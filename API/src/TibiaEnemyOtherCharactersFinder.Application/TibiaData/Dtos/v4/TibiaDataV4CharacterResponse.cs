using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v4;

public class TibiaDataV4CharacterResponse
{
    [JsonProperty("character")]
    public CharacterInfoResponseV4 Character { get; set; }

    [JsonProperty("information")]
    public InformationResponseV4 Information { get; set; }

    public CharacterResult MapToCharacterResult()
    {
        return new CharacterResult()
        {
            Name = Character.Character.Name,
            Traded = Character.Character.Traded,
            World = Character.Character.World,
            Vocation = Character.Character.Vocation,
            Level = Character.Character.Level,
            LastLogin = Character.Character.LastLogin,
            FormerNames = Character.Character.FormerNames,
            FormerWorlds = Character.Character.FormerWorlds,
            OtherCharacters = Character.OtherCharacters?.Select(c => c.Name).ToArray()
        };
    }
}

public class CharacterInfoResponseV4
{
    [JsonProperty("account_badges")]
    public AccountBadgeV4[] AccountBadges { get; set; }

    [JsonProperty("account_information")]
    public AccountInformationV4 AccountInformation { get; set; }

    [JsonProperty("achievements")]
    public AchievementV4[] Achievements { get; set; }

    [JsonProperty("character")]
    public CharacterResponseV4 Character { get; set; }

    [JsonProperty("deaths")]
    public DeathV4[] Deaths { get; set; }

    [JsonProperty("deaths_truncated")]
    public bool DeathsTruncated { get; set; }

    [JsonProperty("other_characters")]
    public OtherCharacterV4[] OtherCharacters { get; set; }
}

public class AccountBadgeV4
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class AccountInformationV4
{
    [JsonProperty("created")]
    public string Created { get; set; }

    [JsonProperty("loyalty_title")]
    public string LoyaltyTitle { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }
}

public class AchievementV4
{
    [JsonProperty("grade")]
    public long Grade { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("secret")]
    public bool Secret { get; set; }
}

public class CharacterResponseV4
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
    public GuildV4 Guild { get; set; }

    [JsonProperty("houses")]
    public HouseV4[] Houses { get; set; }

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

public class GuildV4
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("rank")]
    public string Rank { get; set; }
}

public class HouseV4
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

public class DeathV4
{
    [JsonProperty("assists")]
    public AssistV4[] Assists { get; set; }

    [JsonProperty("killers")]
    public AssistV4[] Killers { get; set; }

    [JsonProperty("level")]
    public long Level { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

public class AssistV4
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

public class OtherCharacterV4
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

