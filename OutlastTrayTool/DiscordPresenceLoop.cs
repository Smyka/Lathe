using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            while (true)
            {
                gameSession = null;
                if (gameClientOpen())
                {
                    gameSession = new GameInfo();

                    FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(fs);
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        processLine(line);
                    }

                    while (true)
                    {
                        line = reader.ReadLine();

                        if (line != null)
                        {
                            if (line.Contains("Log file closed"))
                            {
                                break;
                            }
                            processLine(line);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }

                }

                if (gameSession != null)
                {
                    gameSession.Close();
                }
                Thread.Sleep(30000);
                
            }
            
        }
    }
}
