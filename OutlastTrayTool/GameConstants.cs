using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlastTrayTool
{
    public static class GameConstants
    {
        public static Dictionary<string, string> phases = new Dictionary<string, string> {
            { "GameStageInfo changed", "Loading trial" },
            { "to StageStarted", "StageStarted" },
            { "Level: /Game/Maps/Lobby/Lobby_Persistent", "Lobby" },
            { "LoadMap: /Game/Maps/Global/MainMenu", "Menu" }
        };

        // can be found from fmodel OPP/Content/Text/Program_Trials.json
        public static Dictionary<string, string> trialNames = new Dictionary<string, string>
        {
            { "FPB_MT01", "Punish the Miscreants" },
            { "CH_Trial", "Vindicate the Guilty" },
            { "PS_Trial", "Kill The Snitch" },
            { "OR_Trial", "Cleanse the Orphans" },
            { "FPB_MT02", "Open the Gates" },
            { "FP_Trial", "Grind the Bad Apples" },
            { "PSB_MT01", "Cancel the Autopsy" },
            { "PSB_MT02", "Sabotage the Lockdown" },
            { "TrialMansion", "WELCOME" },
            { "TrialRelease01", "FAREWELL" },
            { "ORS_MT01", "Feed the Children" },
            { "ORS_MT02", "Foster the Orphans" },
            { "PSO_MT01", "Release the prisoners" },
            { "FPC_MT01", "Drill the Futterman" },
            { "ORI_MT01", "Gather the Children of God" },
            { "CHA_MT01", "Escape the Courthouse" },
            { "CHA_MT02", "Destroy the Evidence" },
            { "TF_Trial", "Pervert the Futterman" },
            { "TFS_MT01", "Crush the Sex Toys" },
            { "TFS_MT02", "Incinerate the Sex Toys" },
            { "PSA_MT01", "Teach the Police Officer" },
            { "CHJ_MT01", "Tilt The Scales of Justice" },
            { "TFW_MT01", "Shutdown the Factory" },
            { "MH_Trial", "Poison the Medicine" },
            { "CHA_MT03", "Fuel the Release" },
            { "MHS_MT01", "Empty the Vault" },
            { "MHS_MT02", "Poison the Cattle" },
            { "PSB_MT03", "Eliminate the Past" },
            { "DT_Trial", "Pleasure the Prosecutor" },
            { "ORS_MT03", "Reunite the Family" },
            { "FPB_MT03", "Deface the Futtermans" },
            { "SR_Trial", "Liquidate the Union" },
            { "Escape_Amelia", "Unknown" },
            { "MHS_MT03", "Stash the Contraband" },
            { "MHS_MT04", "Cook the Informant" },
            { "DTS_MT01", "Kidnap the Mistress" },
            { "DTS_MT02", "Spread the Disease" },
            { "TrialRelease_AE", "ESCAPE" },
            { "TFS_MT03", "Fumigate the Factory" },
            { "FPC_MT02", "Redeem Your Freedom" },
            { "Trial_ComingSoon", "MORE COMING SOON..." },
            { "SRR_MT01", "Get Out the Vote" },
            { "SM_Trial", "Kill the politician" },
            { "CHJ_MT02", "Sentence the Prosecuted" },
            { "DTS_MT03", "Traffick the Product" },
            { "SRR_MT02", "Disrupt the neighborhood" },
            { "SMC_MT01", "Investigate the Minotaur" },
            { "FPI_MT01", "Beguile the Children" },
            { "RE_Trial", "Despoil the Auction" },
            { "TS_Trial", "Silence the Idol" },
            { "PSO_MT02", "Seize the narcotics" },
            { "SMC_MT02", "Fabricate the Scandal" },
            { "SRR_MT03", "Eliminate the Legacy" },
            { "TFW_MT02", "Flatten the Foreman" },
            { "RES_MT01", "Solve The Murder" },
            { "AET_MT01", "Escape the Lies" },
            { "ToyFactory", "Pervert the Futterman" }
        };

        public static Dictionary<string, string> trialMaps = new Dictionary<string, string>
        {
            { "PS", "Police Station" },
            { "OR", "Orphanage" },
            { "FP", "Fun Park" },
            { "CH", "Courthouse" },
            { "TF", "Toy Factory" },
            { "MH", "Docks" },
            { "DT", "Downtown" },
            { "SR", "Suburbs" },
            { "SM", "Shopping Mall" },
            { "RE", "Resort" },
            { "To", "Toy Factory" },
            { "TS", "Television Studio" }
        };

        public static Dictionary<string, string> difficulty = new Dictionary<string, string>
        {
            { "Easy", "Introductory" },
            { "Normal", "Standard" },
            { "Hard", "Intensive" },
            { "Insane", "Psychosurgery" }
        };
    }
}
