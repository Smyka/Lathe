using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NAudio.Wave;

namespace OutlastTrayTool
{
    public class MusicPlayer : IDisposable
    {
        public enum Genre { Electro, Classical, MurkoffRising, Kitty }

        // _lock guards all access to _waveOut/_mp3Reader/_resourceStream.
        // DisposeAudioLocked() holds _lock so OnPlaybackStopped can never run concurrently with a dispose — prevents the use-after-free that causes ExecutionEngineException in NAudio.WinMM.
        private readonly object _lock = new();

        private SynchronizationContext? _syncContext;
        private WaveOutEvent?   _waveOut;
        private Mp3FileReader?  _mp3Reader;
        private Stream?         _resourceStream;

        private List<string> _playlist     = new();
        private int          _currentIndex = 0;
        private Genre        _currentGenre = Genre.Electro;
        private bool         _isPlaying    = false;
        private bool         _isMuted      = false;
        private bool         _loop         = false;
        private float        _volume       = 0.8f;
        private bool         _disposed     = false;

        public event Action<string>? SongChanged;
        public event Action<bool>?   PlayStateChanged;

        public bool   IsPlaying    => _isPlaying;
        public bool   IsMuted      => _isMuted;
        public bool   IsLooping    => _loop;
        public float  Volume       => _volume;
        public Genre  CurrentGenre => _currentGenre;

        public string CurrentSong => _playlist.Count > 0
            ? FriendlyName(_playlist[_currentIndex])
            : "No songs found";

        public List<string> SongNames =>
            _playlist.Select(r => FriendlyName(r)).ToList();

        public MusicPlayer() { }

        public void SetSyncContext(SynchronizationContext ctx) => _syncContext = ctx;

        private void RaiseSongChanged(string song) =>
            _syncContext?.Post(_ => SongChanged?.Invoke(song), null);

        private void RaisePlayStateChanged(bool playing) =>
            _syncContext?.Post(_ => PlayStateChanged?.Invoke(playing), null);

        private static string FriendlyName(string resourceName)
        {
            var parts = resourceName.Split('.');
            if (parts.Length >= 4)
            {
                var nameParts = parts.Skip(3).ToArray();
                var joined = string.Join(".", nameParts);
                return Path.GetFileNameWithoutExtension(joined);
            }
            return Path.GetFileNameWithoutExtension(resourceName);
        }

        public void LoadGenre(Genre genre)
        {
            if (_isPlaying) Stop();
            _currentGenre = genre;
            _currentIndex = 0;
            _playlist     = GetResourcesForGenre(genre);
            RaiseSongChanged(CurrentSong);
        }

        private List<string> GetResourcesForGenre(Genre genre)
        {
            var all = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string[] prefixes = genre switch
            {
                Genre.Electro       => new[] { "Lathe.Resources.Electro." },
                Genre.Classical     => new[] { "Lathe.Resources.Classical." },
                Genre.MurkoffRising => new[] { "Lathe.Resources.MurkoffRising." },
                Genre.Kitty         => new[] { "Lathe.Resources.Kitty." },
                _                   => Array.Empty<string>()
            };

            return all
                .Where(n => prefixes.Any(p => n.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                         && n.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                .OrderBy(n => n)
                .ToList();
        }

        public void Play()
        {
            if (_playlist.Count == 0) return;
            lock (_lock)
            {
                if (_disposed) return;
                try
                {
                    DisposeAudioLocked();

                    _resourceStream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(_playlist[_currentIndex]);
                    if (_resourceStream == null) return;

                    _mp3Reader = new Mp3FileReader(_resourceStream);
                    _waveOut   = new WaveOutEvent();
                    _waveOut.Volume = _isMuted ? 0f : _volume;
                    _waveOut.Init(_mp3Reader);
                    _waveOut.PlaybackStopped += OnPlaybackStopped;
                    _waveOut.Play();
                    _isPlaying = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("MusicPlayer.Play error: " + ex.Message);
                    return;
                }
            }
            RaisePlayStateChanged(true);
            RaiseSongChanged(CurrentSong);
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (!Monitor.TryEnter(_lock, 0))
                return;

            bool shouldAdvance = false;
            try
            {
                if (_disposed || !_isPlaying || _playlist.Count == 0)
                    return;

                // Stale callback from a replaced WaveOutEvent — ignore it
                if (!ReferenceEquals(sender, _waveOut))
                    return;

                if (_loop)
                {
                    try
                    {
                        _mp3Reader?.Seek(0, SeekOrigin.Begin);
                        _waveOut?.Play();
                        return;
                    }
                    catch { }
                }

                _currentIndex = (_currentIndex + 1) % _playlist.Count;
                shouldAdvance = true;
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            // Call PlayNextSong outside the lock — it re-acquires it internally
            if (shouldAdvance)
                PlayNextSong();
        }

        private void PlayNextSong()
        {
            if (_playlist.Count == 0) return;
            lock (_lock)
            {
                if (_disposed) return;
                DisposeAudioLocked();

                _resourceStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(_playlist[_currentIndex]);
                if (_resourceStream == null) return;

                _mp3Reader = new Mp3FileReader(_resourceStream);
                _waveOut   = new WaveOutEvent();
                _waveOut.Volume = _isMuted ? 0f : _volume;
                _waveOut.Init(_mp3Reader);
                _waveOut.PlaybackStopped += OnPlaybackStopped;
                _waveOut.Play();
                _isPlaying = true;
            }
            RaiseSongChanged(CurrentSong);
        }

        public void Pause()
        {
            lock (_lock) { _waveOut?.Pause(); _isPlaying = false; }
            RaisePlayStateChanged(false);
        }

        public void Resume()
        {
            if (_waveOut == null) { Play(); return; }
            lock (_lock) { _waveOut?.Play(); _isPlaying = true; }
            RaisePlayStateChanged(true);
        }

        public void Stop()
        {
            lock (_lock) { _isPlaying = false; DisposeAudioLocked(); }
            RaisePlayStateChanged(false);
        }

        public void Next()
        {
            if (_playlist.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _playlist.Count;
            if (_isPlaying) Play();
            else RaiseSongChanged(CurrentSong);
        }

        public void Previous()
        {
            if (_playlist.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
            if (_isPlaying) Play();
            else RaiseSongChanged(CurrentSong);
        }

        public void SelectSong(int index)
        {
            if (index < 0 || index >= _playlist.Count) return;
            _currentIndex = index;
            if (_isPlaying) Play();
            else RaiseSongChanged(CurrentSong);
        }

        public void ToggleMute()
        {
            _isMuted = !_isMuted;
            lock (_lock)
            {
                if (_waveOut != null)
                    _waveOut.Volume = _isMuted ? 0f : _volume;
            }
        }

        public void ToggleLoop() => _loop = !_loop;

        public void SetVolume(float v)
        {
            _volume = Math.Clamp(v, 0f, 1f);
            lock (_lock)
            {
                if (_waveOut != null && !_isMuted)
                    _waveOut.Volume = _volume;
            }
        }

        public void SaveState(string path)
        {
            try
            {
                File.WriteAllLines(path, new[]
                {
                    _currentGenre.ToString(),
                    _currentIndex.ToString(),
                    _volume.ToString(),
                    _isMuted.ToString(),
                    _loop.ToString()
                });
            }
            catch { }
        }

        public void LoadState(string path)
        {
            try
            {
                if (!File.Exists(path)) return;
                var lines = File.ReadAllLines(path);
                if (lines.Length < 5) return;

                if (Enum.TryParse<Genre>(lines[0], out var g))  LoadGenre(g);
                if (int.TryParse(lines[1], out var idx))         _currentIndex = Math.Max(0, idx);
                if (float.TryParse(lines[2], out var vol))       _volume = vol;
                if (bool.TryParse(lines[3], out var muted))      _isMuted = muted;
                if (bool.TryParse(lines[4], out var loop))       _loop = loop;
            }
            catch { }
        }

        // MUST be called while already holding _lock
        private void DisposeAudioLocked()
        {
            var wo = _waveOut;
            var mr = _mp3Reader;
            var rs = _resourceStream;

            // Null out fields first so any concurrent TryEnter-that-fails path
            // sees null and does nothing
            _waveOut        = null;
            _mp3Reader      = null;
            _resourceStream = null;

            // Unsubscribe BEFORE calling Stop() — ensures the callback
            // is never invoked against a partially torn-down object
            if (wo != null) wo.PlaybackStopped -= OnPlaybackStopped;
            try { wo?.Stop(); }    catch { }
            try { wo?.Dispose(); } catch { }
            try { mr?.Dispose(); } catch { }
            try { rs?.Dispose(); } catch { }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;
                _disposed  = true;
                _isPlaying = false;
                DisposeAudioLocked();
            }
        }
    }
}
