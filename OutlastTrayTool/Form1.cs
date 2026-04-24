using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;
using System.Linq;
using System;

namespace OutlastTrayTool
{
    public partial class Form1 : Form
    {
        private ModManager modManager;
        private Config config;
        private GameManager gameManager;
        private DiscordPresenceLoop presence;
        private ModManagerAPI modManagerAPI;
        private NotifyIcon trayIcon;
        private bool startupLaunch = false;
        private bool realExit = false;
        private ThemeMode currentTheme = ThemeMode.Gothic;
        private FlowLayoutPanel flowLayoutPanel5;
        private Task? presenceTask;

        // Music player
        private MusicPlayer musicPlayer = new MusicPlayer();
        private Panel?      musicPanel;
        private bool        musicPanelVisible = false;
        private bool        _refreshingSongList = false;
        // Direct references to music panel controls — survive theme re-styling
        private Label?   _nowPlayingLabel;
        private ListBox? _songListBox;
        private Button?  _btnPlayPause;
        private Button?  _btnMute;
        private Button?  _btnLoop;
        private Button[] _genreBtns = new Button[4];
        private Color[]  _genreCols = {
            Color.FromArgb(60, 160, 220),   // Electro — blue
            Color.FromArgb(200, 160, 80),   // Classical — gold
            Color.FromArgb(160, 80, 220),   // MurkoffRising — purple
            Color.FromArgb(255, 140, 180)   // Kitty — pink
        };

        public Form1()
        {
            InitializeComponent();

            // Load Aviva's icon from embedded resource
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream("Lathe.assets.LatheAviva_Icon.ico");
                if (stream != null)
                    this.Icon = new Icon(stream);
            }
            catch { }

            // Initialize flowLayoutPanel5 (mod browser panel)
            flowLayoutPanel5 = new FlowLayoutPanel();
            flowLayoutPanel5.AutoScroll = true;
            flowLayoutPanel5.WrapContents = true;

            // Load saved theme preference, default to Gothic
            string savedTheme = LoadThemePreference();
            currentTheme = savedTheme switch
            {
                "Outlast"  => ThemeMode.Outlast,
                "Original" => ThemeMode.Original,
                "Pastel"   => ThemeMode.Pastel,
                _          => ThemeMode.Gothic
            };
            try { Theme.Apply(this, currentTheme); } catch { }

            startupLaunch = Environment.GetCommandLineArgs()
                .Any(arg => arg.Equals("--startup", StringComparison.OrdinalIgnoreCase));

            config = new Config();

            gameManager = new GameManager(config);
            modManager = new ModManager(config);

            modManager.SetRefreshUiAction(() =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(LoadCheckboxes));
                }
                else
                {
                    LoadCheckboxes();
                }
            });

            modManager.RefreshMods();
            modManager.StartDownloadWatcher();
            modManagerAPI = new ModManagerAPI();

            LoadPresence();
            LoadTrayIcon();
            LoadFOVSlider();
            LoadScreenPercentageSlider();
            LoadFogComboBox();
            LoadCheckboxes();
            LoadStartupComboBox();
            LoadPresenceComboBox();
            if (startupLaunch)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }

            panel8.Visible = true;
            buttonTheme.Text = "Toggle Interface Theme";
            gameManager.ChangeFOV(120);

            BuildMusicPanel();

            this.Load += (s, e) =>
            {
                musicPlayer.SetSyncContext(SynchronizationContext.Current!);
                string musicCfg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "music.cfg");
                musicPlayer.LoadState(musicCfg);
                RefreshSongList();
            };
        }

        private void LoadTrayIcon()
        {
            trayIcon = new NotifyIcon();
            try { trayIcon.Icon = this.Icon; } catch { }
            trayIcon.Text = "Lathe";
            trayIcon.Visible = true;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            });
            menu.Items.Add("Minimize to Tray", null, (s, e) =>
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            });
            menu.Items.Add("Exit", null, (s, e) =>
            {
                realExit = true;
                DoExit();
            });
            trayIcon.ContextMenuStrip = menu;
            trayIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            };
        }

        private void ToggleTheme()
        {
            // Save current size before theme apply — font changes can resize the form
            var savedSize     = this.Size;
            var savedMinSize  = this.MinimumSize;

            try { Theme.Apply(this, currentTheme); }
            catch (Exception ex)
            {
                MessageBox.Show("Theme failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Restore size — theme engine must not change window dimensions
            this.MinimumSize = savedMinSize;
            this.Size        = savedSize;

            // Restore music panel custom colors — theme engine overwrites them
            RestoreMusicPanelColors();
        }

        private void RestoreMusicPanelColors()
        {
            if (musicPanel == null) return;
            musicPanel.BackColor = Color.FromArgb(18, 10, 18);
            if (_nowPlayingLabel != null)
            {
                _nowPlayingLabel.BackColor = Color.Transparent;
                _nowPlayingLabel.ForeColor = Color.FromArgb(140, 200, 140);
                _nowPlayingLabel.Font = new Font("Courier New", 8f, FontStyle.Italic);
            }
            if (_songListBox != null)
            {
                _songListBox.BackColor = Color.FromArgb(12, 6, 12);
                _songListBox.ForeColor = Color.FromArgb(200, 195, 190);
                _songListBox.Font = new Font("Courier New", 8.5f);
            }
            for (int i = 0; i < _genreBtns.Length; i++)
            {
                if (_genreBtns[i] == null) continue;
                _genreBtns[i].ForeColor = _genreCols[i];
                _genreBtns[i].BackColor = Color.FromArgb(30, 20, 30);
                _genreBtns[i].FlatAppearance.BorderColor = _genreCols[i];
                _genreBtns[i].Font = new Font("Palatino Linotype", 8.5f, FontStyle.Bold);
            }
            if (_btnPlayPause != null) { _btnPlayPause.ForeColor = Color.FromArgb(180, 140, 60); _btnPlayPause.BackColor = Color.FromArgb(25, 15, 25); _btnPlayPause.Font = new Font("Segoe UI Symbol", 10f); }
            if (_btnMute != null)      { _btnMute.ForeColor = musicPlayer.IsMuted ? Color.FromArgb(140, 60, 60) : Color.FromArgb(100, 180, 220); _btnMute.BackColor = Color.FromArgb(25, 15, 25); _btnMute.Font = new Font("Segoe UI Symbol", 10f); }
            if (_btnLoop != null)      { _btnLoop.ForeColor = musicPlayer.IsLooping ? Color.FromArgb(180, 140, 60) : Color.FromArgb(120, 120, 120); _btnLoop.BackColor = Color.FromArgb(25, 15, 25); _btnLoop.Font = new Font("Segoe UI Symbol", 10f); }
        }

        // ── Music button — opens genre picker dropdown ─────────────────
        private void buttonMusic_Click(object sender, EventArgs e)
        {
            if (musicPanel == null) return;
            musicPanelVisible = !musicPanelVisible;
            musicPanel.Visible = musicPanelVisible;
            musicPanel.BringToFront();
        }

        // ── Build the music sidebar panel ─────────────────────────────
        private void BuildMusicPanel()
        {
            musicPanel = new Panel
            {
                Width       = 300,
                Height      = 380,
                BackColor   = Color.FromArgb(18, 10, 18),
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };
            musicPanel.Location = new Point(42, this.ClientSize.Height - 390);
            this.Controls.Add(musicPanel);
            this.Resize += (s, e) =>
                musicPanel.Location = new Point(42, this.ClientSize.Height - 390);

            // ── Title ──────────────────────────────────────────── y=8
            var genreLabel = new Label
            {
                Text      = "🎵  Interface Music",
                ForeColor = Color.FromArgb(180, 140, 60),
                Font      = new Font("Palatino Linotype", 11f, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(10, 8)
            };
            musicPanel.Controls.Add(genreLabel);

            // ── Genre buttons ─────────────────────────────────── y=34
            string[] genres = { "Electro", "Classical", "MR", "Kitty" };
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                MusicPlayer.Genre g = idx switch
                {
                    0 => MusicPlayer.Genre.Electro,
                    1 => MusicPlayer.Genre.Classical,
                    2 => MusicPlayer.Genre.MurkoffRising,
                    _ => MusicPlayer.Genre.Kitty
                };
                var btn = new Button
                {
                    Text      = genres[i],
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(30, 20, 30),
                    ForeColor = _genreCols[i],
                    Font      = new Font("Palatino Linotype", 8.5f, FontStyle.Bold),
                    Size      = new Size(66, 32),
                    Location  = new Point(10 + idx * 70, 40)
                };
                btn.FlatAppearance.BorderColor = _genreCols[i];
                btn.Click += (s, e) =>
                {
                    musicPlayer.LoadGenre(g);
                    musicPlayer.Play();
                    RefreshSongList();
                };
                _genreBtns[i] = btn;
                musicPanel.Controls.Add(btn);
            }

            // ── Divider ───────────────────────────────────────── y=66
            musicPanel.Controls.Add(new Label { BackColor = Color.FromArgb(60, 180, 140, 60), Size = new Size(278, 1), Location = new Point(10, 100) });

            // ── Song list ─────────────────────────────────────── y=72
            _songListBox = new ListBox
            {
                BackColor   = Color.FromArgb(12, 6, 12),
                ForeColor   = Color.FromArgb(200, 195, 190),
                Font        = new Font("Courier New", 8.5f),
                BorderStyle = BorderStyle.None,
                Size        = new Size(278, 100),
                Location    = new Point(10, 78),
                Name        = "songList"
            };
            _songListBox.SelectedIndexChanged += (s, e) =>
            {
                if (_refreshingSongList) return;
                if (_songListBox.SelectedIndex != -1)
    {
        musicPlayer.SelectSong(_songListBox.SelectedIndex);
        
        if (_nowPlayingLabel != null)
        {
            _nowPlayingLabel.Text = "Now Playing: " + musicPlayer.CurrentSong;
        }
    }
            };
            musicPanel.Controls.Add(_songListBox);

            // ── Now playing ───────────────────────────────────── y=178
            _nowPlayingLabel = new Label
            {
                Text      = "♪  " + musicPlayer.CurrentSong,
                ForeColor = Color.FromArgb(140, 200, 140),
                Font      = new Font("Courier New", 8f, FontStyle.Italic),
                AutoSize  = false,
                Size      = new Size(278, 18),
                Location  = new Point(10, 188),
                Name      = "nowPlaying"
            };
            musicPanel.Controls.Add(_nowPlayingLabel);

            // Wire SongChanged to always use field reference — never breaks
            musicPlayer.SongChanged += name =>
            {
                void Update()
                {
                    if (_nowPlayingLabel != null)
                        _nowPlayingLabel.Text = "♪  " + musicPlayer.CurrentSong;
                    RefreshSongList();
                }
                if (musicPanel != null && musicPanel.InvokeRequired)
                    musicPanel.Invoke((Action)Update);
                else
                    Update();
            };

            // ── Volume ────────────────────────────────────────── y=202
            musicPanel.Controls.Add(new Label
            {
                Text      = "VOL",
                ForeColor = Color.FromArgb(140, 130, 120),
                Font      = new Font("Courier New", 7.5f),
                AutoSize  = true,
                Location  = new Point(10, 212)
            });
            var volSlider = new TrackBar
            {
                Minimum   = 0, Maximum = 100,
                Value     = (int)(musicPlayer.Volume * 100),
                TickStyle = TickStyle.None,
                Size      = new Size(218, 14),
                Location  = new Point(40, 206),
                BackColor = Color.FromArgb(18, 10, 18)
            };
            volSlider.Scroll += (s, e) => musicPlayer.SetVolume(volSlider.Value / 100f);
            musicPanel.Controls.Add(volSlider);

            // ── Controls ──────────────────────────────────────── y=234
            int cy = 280, bw = 52, bs = 56;
            Button MkBtn(string t, int x, Color c) => new Button
            {
                Text = t, FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(25, 15, 25), ForeColor = c,
                Font = new Font("Segoe UI Symbol", 10f),
                Size = new Size(bw, 36), Location = new Point(x, cy)
            };

            var btnPrev = MkBtn("⏮", 10, Color.FromArgb(160, 150, 140));
            btnPrev.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnPrev.Click += (s, e) => musicPlayer.Previous();
            musicPanel.Controls.Add(btnPrev);

            _btnPlayPause = MkBtn("▶", 10 + bs, Color.FromArgb(180, 140, 60));
            _btnPlayPause.FlatAppearance.BorderColor = Color.FromArgb(140, 100, 40);
            _btnPlayPause.Click += (s, e) =>
            {
                if (musicPlayer.IsPlaying) { musicPlayer.Pause(); _btnPlayPause.Text = "▶"; }
                else                        { musicPlayer.Play();  _btnPlayPause.Text = "⏸"; }
            };
            musicPlayer.PlayStateChanged += playing =>
            {
                void Update() { if (_btnPlayPause != null) _btnPlayPause.Text = playing ? "⏸" : "▶"; }
                if (_btnPlayPause != null && _btnPlayPause.InvokeRequired)
                    _btnPlayPause.Invoke((Action)Update);
                else Update();
            };
            musicPanel.Controls.Add(_btnPlayPause);

            var btnNext = MkBtn("⏭", 10 + bs * 2, Color.FromArgb(160, 150, 140));
            btnNext.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btnNext.Click += (s, e) => musicPlayer.Next();
            musicPanel.Controls.Add(btnNext);

            _btnMute = MkBtn("🔊", 10 + bs * 3, Color.FromArgb(100, 180, 220));
            _btnMute.FlatAppearance.BorderColor = Color.FromArgb(40, 80, 100);
            _btnMute.Click += (s, e) =>
            {
                musicPlayer.ToggleMute();
                _btnMute.Text      = musicPlayer.IsMuted ? "🔇" : "🔊";
                _btnMute.ForeColor = musicPlayer.IsMuted ? Color.FromArgb(140, 60, 60) : Color.FromArgb(100, 180, 220);
            };
            musicPanel.Controls.Add(_btnMute);

            _btnLoop = MkBtn("🔁", 10 + bs * 4, Color.FromArgb(120, 120, 120));
            _btnLoop.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            _btnLoop.Click += (s, e) =>
            {
                musicPlayer.ToggleLoop();
                _btnLoop.ForeColor = musicPlayer.IsLooping ? Color.FromArgb(180, 140, 60) : Color.FromArgb(120, 120, 120);
                _btnLoop.FlatAppearance.BorderColor = musicPlayer.IsLooping ? Color.FromArgb(140, 100, 40) : Color.FromArgb(60, 60, 60);
            };
            musicPanel.Controls.Add(_btnLoop);

            // ── Divider ───────────────────────────────────────── y=278
            musicPanel.Controls.Add(new Label { BackColor = Color.FromArgb(40, 180, 140, 60), Size = new Size(278, 1), Location = new Point(10, 278) });

            // ── Close button ──────────────────────────────────── y=284
            var btnClose = new Button
            {
                Text = "✕  Close Player", FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(20, 10, 10), ForeColor = Color.FromArgb(120, 80, 80),
                Font = new Font("Palatino Linotype", 9f),
                Size = new Size(278, 50), Location = new Point(10, 320)
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(60, 30, 30);
            btnClose.Click += (s, e) => { musicPanelVisible = false; musicPanel.Visible = false; };
            musicPanel.Controls.Add(btnClose);

            RefreshSongList();
        }

        private void RefreshSongList()
        {
            if (_songListBox == null) return;
            _refreshingSongList = true;
            _songListBox.Items.Clear();
            foreach (var name in musicPlayer.SongNames)
                _songListBox.Items.Add(name);
            if (_songListBox.Items.Count > 0)
            {
                int idx = musicPlayer.SongNames.IndexOf(musicPlayer.CurrentSong);
                _songListBox.SelectedIndex = idx >= 0 ? idx : 0;
            }
            _refreshingSongList = false;
        }

        private void buttonTheme_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();
            menu.RenderMode = ToolStripRenderMode.Professional;
            menu.Renderer   = Theme.GetMenuRenderer(currentTheme);
            menu.BackColor  = Theme.GetMenuBackground(currentTheme);
            menu.ForeColor  = Theme.GetMenuForeground(currentTheme);
            menu.Font       = Theme.GetMenuFont(currentTheme);

            var gothicItem = new ToolStripMenuItem("⚰  Gothic  (Dark Ritual)");
            gothicItem.ForeColor = Color.FromArgb(200, 140, 60);
            gothicItem.Font      = new Font("Palatino Linotype", 10f, FontStyle.Bold);
            gothicItem.Click    += (s, ev) => { currentTheme = ThemeMode.Gothic;   SaveThemePreference("Gothic");   ToggleTheme(); };

            var outlastItem = new ToolStripMenuItem("◈  Outlast Trials");
            outlastItem.ForeColor = Color.FromArgb(80, 200, 80);
            outlastItem.Font      = new Font("Roboto Mono", 10f, FontStyle.Bold);
            outlastItem.Click    += (s, ev) => { currentTheme = ThemeMode.Outlast; SaveThemePreference("Outlast");  ToggleTheme(); };

            var originalItem = new ToolStripMenuItem("☐  Original  (Default)");
            originalItem.ForeColor = Color.FromArgb(0, 120, 215);
            originalItem.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            originalItem.Click    += (s, ev) => { currentTheme = ThemeMode.Original; SaveThemePreference("Original"); ToggleTheme(); };

            var pastelItem = new ToolStripMenuItem("♡  Pastel  (Sweet Dreams)");
            pastelItem.ForeColor = Color.FromArgb(220, 100, 140);
            pastelItem.Font      = new Font("Comic Sans MS", 10f, FontStyle.Bold);
            pastelItem.Click    += (s, ev) => { currentTheme = ThemeMode.Pastel; SaveThemePreference("Pastel"); ToggleTheme(); };

            menu.Items.Add(gothicItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(outlastItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(originalItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(pastelItem);

            Button btn = (Button)sender;
            menu.Show(btn, new Point(0, btn.Height));
        }

        private void label14_Click(object sender, EventArgs e) { }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user clicked X button — do a real exit, don't hide to tray
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = false; // allow the close
                DoExit();
                return;
            }

            // Any other close reason (system shutdown etc) — also exit cleanly
            DoExit();
        }

        private void DoExit()
        {
            try { musicPlayer.SaveState(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "music.cfg")); } catch { }
            try { musicPlayer.Dispose(); } catch { }
            try { presence?.Stop(); } catch { }
            try { modManager?.StopDownloadWatcher(); } catch { }
            try { trayIcon.Visible = false; trayIcon.Dispose(); } catch { }
            System.Environment.Exit(0);
        }

        private void SaveThemePreference(string theme)
        {
            try
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "theme.cfg");
                File.WriteAllText(path, theme);
            }
            catch { }
        }

        private string LoadThemePreference()
        {
            try
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "theme.cfg");
                if (File.Exists(path))
                    return File.ReadAllText(path).Trim();
            }
            catch { }
            return "Gothic";
        }

        private void CleanupBeforeExit()
        {
            try
            {
                // Stop Discord presence loop if running
                if (presence != null)
                {
                    presence.Stop();
                    
                    // Wait for the presence task to complete (max 5 seconds)
                    if (presenceTask != null && !presenceTask.IsCompleted)
                    {
                        presenceTask.Wait(TimeSpan.FromSeconds(5));
                    }
                }

                // Stop mod download watcher
                if (modManager != null)
                {
                    modManager.StopDownloadWatcher();
                }

                // Dispose of tray icon
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }
                
                System.Diagnostics.Debug.WriteLine("Cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        private void LoadPresence()
        {
            if (config.LoadConfig()["presence"] == "Enabled")
            {
                presence = new DiscordPresenceLoop();
                Task.Run(() => presence.StartLoop());
            }
        }

        private void LoadCheckboxes()
        {
            try
            {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(label4);

            JObject configObj = JObject.Parse(File.ReadAllText(config.configPath));
            JObject modMap = (JObject)configObj["modMap"];
            var modGroups = modMap.Properties()
                .Where(mod => mod.Name != "undefined")
                .ToList();

            var undefinedGroup = modMap.Properties()
                .FirstOrDefault(mod => mod.Name == "undefined");

            if (undefinedGroup != null)
                modGroups.Add(undefinedGroup);

            foreach (JProperty modGroup in modGroups)
            {
                if (modGroup.Value is not JObject modObj)
                    continue;

                string modName = modObj["name"]?.ToString() ?? modGroup.Name;

                if (modGroup.Name == "undefined")
                    modName = "Other";

                JArray files = modObj["files"] as JArray;

                if (files == null || files.Count == 0)
                    continue;

                Label modNameLabel = new Label();
                modNameLabel.Text = modName;
                modNameLabel.AutoSize = true;
                modNameLabel.Font = new Font("Figtree", 11f, FontStyle.Bold);
                modNameLabel.ForeColor = Color.Honeydew;
                modNameLabel.Margin = new Padding(3, 12, 3, 3);
                flowLayoutPanel1.Controls.Add(modNameLabel);

                foreach (JToken fileToken in files)
                {
                    string fileName = fileToken.ToString();

                    bool isEnabled = modManager.enabledMods.Contains(fileName);
                    bool isDisabled = modManager.disabledMods.Contains(fileName);

                    if (!isEnabled && !isDisabled)
                        continue;

                    CheckBox chk = new CheckBox();
                    chk.Text = fileName;
                    chk.Name = "chk_" + fileName;
                    chk.AutoSize = true;
                    chk.Checked = isEnabled;
                    chk.Font = new Font("Figtree", 10f);
                    chk.Margin = new Padding(20, 2, 3, 2);

                    chk.CheckedChanged += (s, ev) =>
                    {
                        CheckBox senderChk = (CheckBox)s;
                        if (senderChk.Checked)
                            modManager.EnableMod(senderChk.Text);
                        else
                            modManager.DisableMod(senderChk.Text);

                        modManager.RefreshMods();
                        LoadCheckboxes();
                    };

                    flowLayoutPanel1.Controls.Add(chk);
                }
            }
            } catch { }
        }

        private void LoadFOVSlider()
        {
            try
            {
                textBox2.Text = config.LoadConfig()["FOV"].ToString();
                trackBar1.Value = config.LoadConfig()["FOV"];
            }
            catch { }
        }

        private void LoadScreenPercentageSlider()
        {
            try
            {
                textBox1.Text = config.LoadConfig()["screenPercentage"].ToString();
                trackBar2.Value = config.LoadConfig()["screenPercentage"];
            }
            catch { }
        }

        private void LoadFogComboBox()
        {
            try
            {
                var val = config.LoadConfig()["fog"]?.ToString();
                if (val != null) comboBox1.SelectedItem = val;
            }
            catch { }
        }

        private void LoadPresenceComboBox()
        {
            try
            {
                var val = config.LoadConfig()["presence"]?.ToString();
                if (val != null) comboBox2.SelectedItem = val;
            }
            catch { }
        }

        private void LoadStartupComboBox()
        {
            try
            {
                var val = config.LoadConfig()["startup"]?.ToString();
                if (val != null) comboBox3.SelectedItem = val;
            }
            catch { }
        }

        private async void LoadModBrowser()
        {
            dynamic json = await ModManagerAPI.GetModPageAsync();

            for (int i = 0; i < 20; i++)
            {
                string name = json.data.mods.nodes[i].name;
                string uploader = json.data.mods.nodes[i].uploader.name;
                string thumbnail = json.data.mods.nodes[i].thumbnailUrl;
                string summary = json.data.mods.nodes[i].summary;
                int downloads = json.data.mods.nodes[i].downloads;
                int endorsements = json.data.mods.nodes[i].endorsements;
                int modId = json.data.mods.nodes[i].modId;
                string modUrl = $"https://www.nexusmods.com/theoutlasttrials/mods/{modId}";
                DateTime updatedAt = json.data.mods.nodes[i].updatedAt;

                Panel panel = new Panel();
                panel.Size = new Size(295, 268);
                panel.Margin = new Padding(5);
                panel.BorderStyle = BorderStyle.FixedSingle;

                FlowLayoutPanel detailsPanel = new FlowLayoutPanel();
                detailsPanel.Size = new Size(200, 30);
                detailsPanel.Location = new Point(3, 210);

                PictureBox thumbnailBox = new PictureBox();
                thumbnailBox.Size = new Size(295, 166);
                thumbnailBox.Location = new Point(0, 0);
                thumbnailBox.SizeMode = PictureBoxSizeMode.Zoom;

                PictureBox downloadIconBox = new PictureBox();
                downloadIconBox.Size = new Size(12, 12);
                downloadIconBox.SizeMode = PictureBoxSizeMode.Zoom;
                downloadIconBox.ImageLocation = @"assets/downloadIcon.png";
                downloadIconBox.LoadAsync();

                PictureBox likeIconBox = new PictureBox();
                likeIconBox.Size = new Size(12, 12);
                likeIconBox.SizeMode = PictureBoxSizeMode.Zoom;
                likeIconBox.ImageLocation = @"assets/likeIcon.png";
                likeIconBox.LoadAsync();

                Label nameLabel = new Label();
                nameLabel.Text = name;
                nameLabel.Font = new Font("Segoe UI", 12f);
                nameLabel.ForeColor = Color.Honeydew;
                nameLabel.Location = new Point(3, 170);
                nameLabel.Size = new Size(274, 22);

                Label uploaderLabel = new Label();
                uploaderLabel.Text = uploader;
                uploaderLabel.Font = new Font("Segoe UI", 9f);
                uploaderLabel.ForeColor = Color.LightGray;
                uploaderLabel.Location = new Point(3, 192);
                uploaderLabel.Size = new Size(274, 22);

                Label downloadLabel = new Label();
                downloadLabel.Text = downloads.ToString();
                downloadLabel.Font = new Font("Segoe UI", 8f);
                downloadLabel.ForeColor = Color.LightGray;
                downloadLabel.AutoSize = true;
                downloadLabel.Margin = new Padding(0, 2, 5, 3);

                Label likeLabel = new Label();
                likeLabel.Text = endorsements.ToString();
                likeLabel.Font = new Font("Segoe UI", 8f);
                likeLabel.ForeColor = Color.LightGray;
                likeLabel.AutoSize = true;
                likeLabel.Margin = new Padding(0, 2, 5, 3);

                Label updatedAtLabel = new Label();
                updatedAtLabel.Text = updatedAt.ToShortDateString();
                updatedAtLabel.Font = new Font("Segoe UI", 8f);
                updatedAtLabel.ForeColor = Color.LightGray;
                updatedAtLabel.AutoSize = true;
                updatedAtLabel.Margin = new Padding(0, 2, 5, 3);

                Button viewOnNexusButton = new Button();
                viewOnNexusButton.Text = "View on Nexus";
                viewOnNexusButton.Location = new Point(5, 233);
                viewOnNexusButton.FlatStyle = FlatStyle.Flat;
                viewOnNexusButton.Size = new Size(100, 27);
                viewOnNexusButton.BackColor = Color.FromArgb(47, 54, 61);
                viewOnNexusButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                viewOnNexusButton.ForeColor = Color.Honeydew;
                viewOnNexusButton.Click += (s, e) =>
                {
                    Process.Start(new ProcessStartInfo { FileName = modUrl, UseShellExecute = true });
                };

                Button detailsButton = new Button();
                detailsButton.Text = "Details";
                detailsButton.Location = new Point(110, 233);
                detailsButton.FlatStyle = FlatStyle.Flat;
                detailsButton.Size = new Size(70, 27);
                detailsButton.BackColor = Color.FromArgb(47, 54, 61);
                detailsButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                detailsButton.ForeColor = Color.Honeydew;
                detailsButton.Click += (s, e) =>
                {
                    Debug.Write(summary);
                    toolTip1.Show(summary, detailsButton);
                };

                Button installButton = new Button();
                installButton.Text = "Install";
                installButton.Location = new Point(185, 233);
                installButton.FlatStyle = FlatStyle.Flat;
                installButton.Size = new Size(102, 27);
                installButton.BackColor = Color.FromArgb(47, 54, 61);
                installButton.FlatAppearance.BorderColor = Color.FromArgb(113, 123, 133);
                installButton.ForeColor = Color.Honeydew;

                await ModManagerAPI.LoadImageAsync(thumbnail, thumbnailBox);

                detailsPanel.Controls.Add(downloadIconBox);
                detailsPanel.Controls.Add(downloadLabel);
                detailsPanel.Controls.Add(likeIconBox);
                detailsPanel.Controls.Add(likeLabel);
                detailsPanel.Controls.Add(updatedAtLabel);

                panel.Controls.Add(detailsPanel);
                panel.Controls.Add(nameLabel);
                panel.Controls.Add(uploaderLabel);
                panel.Controls.Add(thumbnailBox);
                panel.Controls.Add(viewOnNexusButton);
                panel.Controls.Add(detailsButton);
                panel.Controls.Add(installButton);
                viewOnNexusButton.BringToFront();
                detailsButton.BringToFront();
                installButton.BringToFront();

                flowLayoutPanel5.Controls.Add(panel);
            }
        }

        private void HideTabPanels()
        {
            panel2.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel9.Visible = false;
        }

        private void ResetMenuButtonColorsAndBorders()
        {
            foreach (Control c in panel1.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(29, 33, 37);
                    btn.FlatAppearance.BorderColor = Color.FromArgb(29, 33, 37);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pak Files (*.pak)|*.pak|Zip Files (*.zip)|*.zip|Rar Files (*.rar)|*.rar|7z Files (*.7z)|*.7z";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            modManager.AddMod(dialog.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pak Files (*.pak)|*.pak|Zip Files (*.zip)|*.zip|Rar Files (*.rar)|*.rar|7z Files (*.7z)|*.7z";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            modManager.AddMod(dialog.FileName);
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(250, 120);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                gameManager.ChangeFOV(Convert.ToInt32(textBox2.Text));
                MessageBox.Show($"Changed FOV to {textBox2.Text}");
            }
            catch (FormatException) { trackBar1.Value = 0; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try { trackBar1.Value = Convert.ToInt32(textBox2.Text); }
            catch (FormatException) { trackBar1.Value = 0; }
            catch (ArgumentOutOfRangeException) { trackBar1.Value = 150; }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = $"{trackBar1.Value}";
        }

        private void flowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void flowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                modManager.AddMod(file);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel2.Visible = true;
            button4.BackColor = Color.FromArgb(47, 54, 61);
            button4.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel4.Visible = true;
            button5.BackColor = Color.FromArgb(47, 54, 61);
            button5.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            // Also update the theme button label to be correct
            buttonTheme.Text = "Toggle Interface Theme";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HideTabPanels();
            ResetMenuButtonColorsAndBorders();
            panel9.Visible = true;
            button6.BackColor = Color.FromArgb(47, 54, 61);
            button6.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = $"{trackBar2.Value}";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                gameManager.ChangeScreenPercentage(Convert.ToInt32(textBox1.Text));
                MessageBox.Show($"Changed screen percentage to {textBox1.Text}");
            }
            catch (FormatException) { trackBar2.Value = 0; }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            gameManager.ChangeFogToggle(comboBox1.SelectedItem.ToString());
            MessageBox.Show($"Changed fog to {comboBox1.SelectedItem}");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            gameManager.UninstallReshade();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Go to nexusmods.com or click the button below\r\n2. Download mods while leaving this tool running and it will \r\nautomatically add the mod to the mod manager\r\n3. If you need to manually add mods, you can drag and drop\r\nthem or click the open button\r\n");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            gameManager.InstallReshade();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.nexusmods.com/games/theoutlasttrials/mods",
                UseShellExecute = true
            });
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem?.ToString() == "Enabled")
            {
                StartupManager.EnableStartup();
                config.ChangeProperty("startup", "Enabled");
            }
            else
            {
                StartupManager.DisableStartup();
                config.ChangeProperty("startup", "Disabled");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem?.ToString() == "Enabled")
            {
                if (config.LoadConfig()["startup"] == "Disabled")
                {
                    MessageBox.Show("Discord presence has been enabled for this session\nHowever, it will not properly work after restarts unless you enable\n launch on startup");
                }
                presence = new DiscordPresenceLoop();
                Task.Run(() => presence.StartLoop());
                config.ChangeProperty("presence", "Enabled");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click_1(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void pictureBox6_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }
        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e) { }
        private void panel8_Paint(object sender, PaintEventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
    }

    public enum ThemeMode { Gothic, Outlast, Original, Pastel }

    // ── Gothic renderer (dark crimson) ────────────────────────────────
    public class GothicMenuRenderer : ToolStripProfessionalRenderer
    {
        public GothicMenuRenderer() : base(new GothicColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var col = e.Item.Selected ? Color.FromArgb(80, 100, 0, 0) : Color.FromArgb(18, 10, 18);
            using (var b = new System.Drawing.SolidBrush(col))
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            if (e.Item.Selected)
                using (var p = new System.Drawing.Pen(Color.FromArgb(140, 0, 0), 1f))
                    e.Graphics.DrawRectangle(p, new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1));
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var p = new System.Drawing.Pen(Color.FromArgb(80, 140, 0, 0), 1f))
                e.Graphics.DrawLine(p, 10, y, e.Item.Width - 10, y);
        }
    }
    public class GothicColorTable : ProfessionalColorTable
    {
        public override Color MenuItemBorder              => Color.FromArgb(140, 0, 0);
        public override Color MenuBorder                  => Color.FromArgb(100, 0, 0);
        public override Color ToolStripDropDownBackground => Color.FromArgb(18, 10, 18);
        public override Color ImageMarginGradientBegin    => Color.FromArgb(18, 10, 18);
        public override Color ImageMarginGradientMiddle   => Color.FromArgb(18, 10, 18);
        public override Color ImageMarginGradientEnd      => Color.FromArgb(18, 10, 18);
    }

    // ── Outlast Trials renderer (deep green) ──────────────────────────
    public class OutlastMenuRenderer : ToolStripProfessionalRenderer
    {
        public OutlastMenuRenderer() : base(new OutlastColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var col = e.Item.Selected ? Color.FromArgb(60, 0, 80, 0) : Color.FromArgb(8, 14, 8);
            using (var b = new System.Drawing.SolidBrush(col))
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            if (e.Item.Selected)
                using (var p = new System.Drawing.Pen(Color.FromArgb(0, 140, 0), 1f))
                    e.Graphics.DrawRectangle(p, new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1));
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var p = new System.Drawing.Pen(Color.FromArgb(60, 0, 120, 0), 1f))
                e.Graphics.DrawLine(p, 10, y, e.Item.Width - 10, y);
        }
    }
    public class OutlastColorTable : ProfessionalColorTable
    {
        public override Color MenuItemBorder              => Color.FromArgb(0, 140, 0);
        public override Color MenuBorder                  => Color.FromArgb(0, 100, 0);
        public override Color ToolStripDropDownBackground => Color.FromArgb(8, 14, 8);
        public override Color ImageMarginGradientBegin    => Color.FromArgb(8, 14, 8);
        public override Color ImageMarginGradientMiddle   => Color.FromArgb(8, 14, 8);
        public override Color ImageMarginGradientEnd      => Color.FromArgb(8, 14, 8);
    }

    // ── Original renderer (clean Windows style) ───────────────────────
    public class OriginalMenuRenderer : ToolStripProfessionalRenderer
    {
        public OriginalMenuRenderer() : base(new OriginalColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var col = e.Item.Selected ? Color.FromArgb(40, 0, 120, 215) : Color.White;
            using (var b = new System.Drawing.SolidBrush(col))
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            if (e.Item.Selected)
                using (var p = new System.Drawing.Pen(Color.FromArgb(0, 120, 215), 1f))
                    e.Graphics.DrawRectangle(p, new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1));
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var p = new System.Drawing.Pen(Color.FromArgb(200, 200, 200), 1f))
                e.Graphics.DrawLine(p, 10, y, e.Item.Width - 10, y);
        }
    }
    public class OriginalColorTable : ProfessionalColorTable
    {
        public override Color MenuItemBorder              => Color.FromArgb(0, 120, 215);
        public override Color MenuBorder                  => Color.FromArgb(180, 180, 180);
        public override Color ToolStripDropDownBackground => Color.White;
        public override Color ImageMarginGradientBegin    => Color.White;
        public override Color ImageMarginGradientMiddle   => Color.White;
        public override Color ImageMarginGradientEnd      => Color.White;
    }

    public class PastelMenuRenderer : ToolStripProfessionalRenderer
    {
        public PastelMenuRenderer() : base(new PastelColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var col = e.Item.Selected ? Color.FromArgb(255, 200, 220) : Color.FromArgb(255, 228, 238);
            using (var b = new System.Drawing.SolidBrush(col))
                e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            if (e.Item.Selected)
                using (var p = new System.Drawing.Pen(Color.FromArgb(220, 100, 140), 1f))
                    e.Graphics.DrawRectangle(p, new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1));
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (var p = new System.Drawing.Pen(Color.FromArgb(255, 180, 210), 1f))
                e.Graphics.DrawLine(p, 10, y, e.Item.Width - 10, y);
        }
    }
    public class PastelColorTable : ProfessionalColorTable
    {
        public override Color MenuItemBorder              => Color.FromArgb(220, 100, 140);
        public override Color MenuBorder                  => Color.FromArgb(255, 150, 180);
        public override Color ToolStripDropDownBackground => Color.FromArgb(255, 228, 238);
        public override Color ImageMarginGradientBegin    => Color.FromArgb(255, 218, 232);
        public override Color ImageMarginGradientMiddle   => Color.FromArgb(255, 218, 232);
        public override Color ImageMarginGradientEnd      => Color.FromArgb(255, 218, 232);
    }
}
