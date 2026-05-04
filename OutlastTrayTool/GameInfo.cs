using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace OutlastTrayTool
{
    

    public class GameInfo
    {
        private string gamePhase;
        private string trialMap;
        private string trialName;
        private string difficulty;
        private string largeImage;

        private Dictionary<string, string> trialNames;
        private Dictionary<string, string> trialMaps;
        private Dictionary<string, string> difficulties;
        private Dictionary<string, string> phases;

        private string appId;
        public static DiscordRpcClient client;

        public GameInfo()
        {
            largeImage = "murkoff";
            trialNames = GameConstants.trialNames;
            trialMaps = GameConstants.trialMaps;
            difficulties = GameConstants.difficulty;
            phases = GameConstants.phases;

            appId = "1453685678012366878";
            client = new DiscordRpcClient(appId);
            client.Initialize();
        }

        public void setGamePhase(string phase)
        {
            gamePhase = phases[phase];
        }

        public void setTrial(string trialId)
        {
            if (trialNames.TryGetValue(trialId, out string outTrialName))
            {
                trialName = outTrialName;
            }
            else
            {
                Console.WriteLine("trial not found");
            }
            if (trialMaps.TryGetValue(trialId[..2], out string outTrialMap)) { 
                trialMap = outTrialMap;
                largeImage = trialId.ToLower();
            }
            else
            {
                Console.WriteLine("trial map not found");
            }
        }

        public void setDifficulty(string inputDifficulty)
        {
            // shouldnt ever fail so no tryget
            difficulty = difficulties[inputDifficulty];
        }

        public void updatePresence() {
            if (gamePhase is null)
            {
                Console.WriteLine("No game phase yet");
                return;
            }

            if (gamePhase == "Menu")
            {
                client.SetPresence(new RichPresence()
                {
                    State = "In the main menu",
                    Assets = new Assets() { 
                        LargeImageKey = "murkoff"
                    }
                });

                return;
            }

            if (gamePhase == "Lobby")
            {
                client.SetPresence(new RichPresence()
                {
                    State = "In the lobby",
                    Assets = new Assets()
                    {
                        LargeImageKey = "murkoff"
                    }
                });

                return;
            }

            if (gamePhase == "Loading trial")
            {
                client.SetPresence(new RichPresence()
                {
                    State = trialName,
                    Details = "Loading trial...",
                    Assets = new Assets()
                    {
                        SmallImageKey = "murkoff",
                        LargeImageKey = largeImage
                    }
                });

                return;
            }

            if (gamePhase == "StageStarted")
            {
                client.SetPresence(new RichPresence()
                {
                    State = $"{trialMap} - {difficulty}",
                    Details = trialName,
                    Assets = new Assets()
                    {
                        SmallImageKey = "murkoff",
                        LargeImageKey = largeImage
                    }
                });

                return;
            }
        }

        public void Close()
        {
            client.ClearPresence();
            client.Dispose();
        }
    }
}
