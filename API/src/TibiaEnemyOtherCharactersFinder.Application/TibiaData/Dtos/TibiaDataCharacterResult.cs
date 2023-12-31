using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataCharacterResult
{
    [JsonProperty("character")]
    public CharacterInfo Character { get; set; }

    [JsonProperty("information")]
    public Information Information { get; set; }
}

public class CharacterInfo
{
    [JsonProperty("account_badges")]
    public AccountBadge[] AccountBadges { get; set; }

    [JsonProperty("account_information")]
    public AccountInformation AccountInformation { get; set; }

    [JsonProperty("achievements")]
    public Achievement[] Achievements { get; set; }

    [JsonProperty("character")]
    public CharacterResult Character { get; set; }

    [JsonProperty("deaths")]
    public Death[] Deaths { get; set; }

    [JsonProperty("deaths_truncated")]
    public bool DeathsTruncated { get; set; }

    [JsonProperty("other_characters")]
    public OtherCharacter[] OtherCharacters { get; set; }
}

public class AccountBadge
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class AccountInformation
{
    [JsonProperty("created")]
    public string Created { get; set; }

    [JsonProperty("loyalty_title")]
    public string LoyaltyTitle { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }
}

public class Achievement
{
    [JsonProperty("grade")]
    public long Grade { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("secret")]
    public bool Secret { get; set; }
}

public class CharacterResult
{
    [JsonProperty("account_status")]
    public string AccountStatus { get; set; }

    [JsonProperty("achievement_points")]
    public long AchievementPoints { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("deletion_date")]
    public string DeletionDate { get; set; }

    [JsonProperty("former_names")]
    public string[] FormerNames { get; set; }

    [JsonProperty("former_worlds")]
    public string[] FormerWorlds { get; set; }

    [JsonProperty("guild")]
    public Guild Guild { get; set; }

    [JsonProperty("houses")]
    public House[] Houses { get; set; }

    [JsonProperty("last_login")]
    public string LastLogin { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("married_to")]
    public string MarriedTo { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }

    [JsonProperty("residence")]
    public string Residence { get; set; }

    [JsonProperty("sex")]
    public string Sex { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("traded")]
    public bool Traded { get; set; }

    [JsonProperty("unlocked_titles")]
    public long UnlockedTitles { get; set; }

    [JsonProperty("vocation")]
    public string Vocation { get; set; }

    [JsonProperty("world")]
    public string World { get; set; }
}

public class Guild
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("rank")]
    public string Rank { get; set; }
}

public class House
{
    [JsonProperty("houseid")]
    public long Houseid { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("paid")]
    public string Paid { get; set; }

    [JsonProperty("town")]
    public string Town { get; set; }
}

public class Death
{
    [JsonProperty("assists")]
    public Assist[] Assists { get; set; }

    [JsonProperty("killers")]
    public Assist[] Killers { get; set; }

    [JsonProperty("level")]
    public long Level { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

public class Assist
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("player")]
    public bool Player { get; set; }

    [JsonProperty("summon")]
    public string Summon { get; set; }

    [JsonProperty("traded")]
    public bool Traded { get; set; }
}

public class OtherCharacter
{
    [JsonProperty("deleted")]
    public bool Deleted { get; set; }

    [JsonProperty("main")]
    public bool Main { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("traded")]
    public bool Traded { get; set; }

    [JsonProperty("world")]
    public string World { get; set; }
}

public class Information
{
    [JsonProperty("api")]
    public Api Api { get; set; }

    [JsonProperty("status")]
    public Status Status { get; set; }

    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }
}

public class Api
{
    [JsonProperty("commit")]
    public string Commit { get; set; }

    [JsonProperty("release")]
    public string Release { get; set; }

    [JsonProperty("version")]
    public long Version { get; set; }
}

public class Status
{
    [JsonProperty("error")]
    public long Error { get; set; }

    [JsonProperty("http_code")]
    public long HttpCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}

