using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/* 
this script handles starting the discord presence which forever checks through the game
log to see if it's open 
*/

namespace OutlastTrayTool
{
    public class DiscordPresenceLoop
    {
        private Dictionary<string, string> phases;
        private string logPath;
        private GameInfo gameSession;
        private CancellationTokenSource? _cancellationTokenSource;
        private volatile bool _shouldStop = false;
        public DiscordPresenceLoop() {
            phases = GameConstants.phases;
            logPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "OPP/Saved/Logs/OPP.log");
        }

        public void processLine(string line)
        {
            foreach (string phase in phases.Keys)
            {
                if (line.Contains(phase))
                {
                    gameSession.setGamePhase(phase);

                    if (phase == "GameStageInfo changed")
                    {
                        string trialId = line.Split(new[] { "Mission:" }, 2, StringSplitOptions.None)[1]
                            .Split(new[] { ',' }, 2)[0]
                            .Trim();
                        string difficulty = line.Split(new[] { "difficulty:" }, 2, StringSplitOptions.None)[1]
                            .Split(new[] { ',' }, 2)[0]
                            .Trim();

                        gameSession.setTrial(trialId);
                        gameSession.setDifficulty(difficulty); 
                    }

                    gameSession.updatePresence();
                    break;
                }
            }
        }

        public bool gameClientOpen()
        {
            FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(fs);

            string line;
            string lastLine = "";

            while ((line = reader.ReadLine()) != null)
            {
                lastLine = line;
            }

            reader.Close();
            fs.Close();

            return !lastLine.Contains("Log file closed");
        }

        public void StartLoop()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            _shouldStop = false;

            while (!_shouldStop && !token.IsCancellationRequested)
            {
                try
                {
                    gameSession = null;

                    if (gameClientOpen())
                    {
                        gameSession = new GameInfo();

                        FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        StreamReader reader = new StreamReader(fs);
                        string line;

                        while ((line = reader.ReadLine()) != null && !_shouldStop && !token.IsCancellationRequested)
                            processLine(line);

                        while (!_shouldStop && !token.IsCancellationRequested)
                        {
                            line = reader.ReadLine();

                            if (line != null)
                            {
                                if (line.Contains("Log file closed")) break;
                                processLine(line);
                            }
                            else
                            {
                                // Non-blocking sleep — checks cancellation every 100ms
                                for (int i = 0; i < 10 && !_shouldStop && !token.IsCancellationRequested; i++)
                                    System.Threading.Thread.Sleep(100);
                            }
                        }

                        reader.Close();
                        fs.Close();
                    }

                    try { gameSession?.Close(); } catch { }

                    if (!_shouldStop && !token.IsCancellationRequested)
                    {
                        // Non-blocking 30s wait — exits immediately on stop
                        for (int i = 0; i < 300 && !_shouldStop && !token.IsCancellationRequested; i++)
                            System.Threading.Thread.Sleep(100);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception) { /* keep running on non-fatal errors */ }
            }

            try { gameSession?.Close(); } catch { }
        }

        public void Stop()
        {
            _shouldStop = true;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
