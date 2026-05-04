using Newtonsoft.Json;

namespace OutlastTrayTool
{
    public static class GameConstants
    {
        private const string GameConstantsUrl =
            "https://raw.githubusercontent.com/Smyka/Lathe/refs/heads/master/api/gameConstants.json";

        public static Dictionary<string, string> phases { get; private set; } = new();
        public static Dictionary<string, string> trialNames { get; private set; } = new();
        public static Dictionary<string, string> trialMaps { get; private set; } = new();
        public static Dictionary<string, string> difficulty { get; private set; } = new();

        public static async Task LoadAsync()
        {
            using HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            string json = await client.GetStringAsync(GameConstantsUrl);

            GameConstantsData data = JsonConvert.DeserializeObject<GameConstantsData>(json)!;

            phases = data.phases ?? new();
            trialNames = data.trialNames ?? new();
            trialMaps = data.trialMaps ?? new();
            difficulty = data.difficulty ?? new();
        }
        public static async Task<bool> TryLoadWithRetryAsync()
        {
            DateTime deadline = DateTime.UtcNow.AddMinutes(1);

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    await LoadAsync();
                    return true;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            return false;
        }


        private class GameConstantsData
        {
            public Dictionary<string, string>? phases { get; set; }
            public Dictionary<string, string>? trialNames { get; set; }
            public Dictionary<string, string>? trialMaps { get; set; }
            public Dictionary<string, string>? difficulty { get; set; }
        }
    }
}
