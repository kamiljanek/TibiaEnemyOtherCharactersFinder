namespace WorldScanSeeder
{
    public class WorldApiResult
    {
        public WorldsApi worlds { get; set; }
        public InformationApi information { get; set; }
    }

    public class WorldsApi
    {
        public WorldApi world { get; set; }
    }

    public class InformationApi
    {
        public int api_version { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class WorldApi
    {
        public string name { get; set; }
        public string status { get; set; }
        public int players_online { get; set; }
        public int record_players { get; set; }
        public DateTime record_date { get; set; }
        public string creation_date { get; set; }
        public string location { get; set; }
        public string pvp_type { get; set; }
        public bool premium_only { get; set; }
        public string transfer_type { get; set; }
        public List<string> world_quest_titles { get; set; }
        public bool battleye_protected { get; set; }
        public string battleye_date { get; set; }
        public string game_world_type { get; set; }
        public string tournament_world_type { get; set; }
        public List<OnlinePlayerApi> online_players { get; set; }
    }

    public class OnlinePlayerApi
    {
        public string name { get; set; }
        public int level { get; set; }
        public string vocation { get; set; }
    }
}