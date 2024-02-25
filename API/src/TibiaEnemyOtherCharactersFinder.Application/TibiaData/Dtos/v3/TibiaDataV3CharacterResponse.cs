using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

public class TibiaDataV3CharacterResponse
{
    [JsonProperty("characters")]
    public CharactersInfoResponseV3 Characters { get; set; }

    [JsonProperty("information")]
    public InformationResponseV3 Information { get; set; }

    public CharacterResult MapToCharacterResult()
    {
        return new CharacterResult()
        {
            Name = Characters.Character.Name,
            Traded = Characters.Character.Traded,
            World = Characters.Character.World,
            Vocation = Characters.Character.Vocation,
            Level = Characters.Character.Level,
            LastLogin = Characters.Character.LastLogin,
            FormerNames = Characters.Character.FormerNames,
            FormerWorlds = Characters.Character.FormerWorlds,
            OtherCharacters = Characters.OtherCharacters?.Select(c => c.Name).ToArray()
        };
    }
}

public class CharactersInfoResponseV3
{
    [JsonProperty("account_badges")]
    public AccountBadgeV3[] AccountBadges { get; set; }

    [JsonProperty("account_information")]
    public AccountInformationV3 AccountInformation { get; set; }

    [JsonProperty("achievements")]
    public AchievementV3[] Achievements { get; set; }

    [JsonProperty("character")]
    public CharacterResponseV3 Character { get; set; }

    [JsonProperty("deaths")]
    public DeathV3[] Deaths { get; set; }

    [JsonProperty("other_characters")]
    public OtherCharacterV3[] OtherCharacters { get; set; }
}

public class AccountBadgeV3
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class AccountInformationV3
{
    [JsonProperty("created")]
    public string Created { get; set; }

    [JsonProperty("loyalty_title")]
    public string LoyaltyTitle { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }
}

public class AchievementV3
{
    [JsonProperty("grade")]
    public int Grade { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("secret")]
    public bool Secret { get; set; }
}

public class CharacterResponseV3
{
    [JsonProperty("account_status")]
    public string AccountStatus { get; set; }

    [JsonProperty("achievement_points")]
    public int AchievementPoints { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("deletion_date")]
    public string DeletionDate { get; set; }

    [JsonProperty("former_names")]
    public string[] FormerNames { get; set; }

    [JsonProperty("former_worlds")]
    public string[] FormerWorlds { get; set; }

    [JsonProperty("guild")]
    public GuildV3 Guild { get; set; }

    [JsonProperty("houses")]
    public HouseV3[] Houses { get; set; }

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
    public int UnlockedTitles { get; set; }

    [JsonProperty("vocation")]
    public string Vocation { get; set; }

    [JsonProperty("world")]
    public string World { get; set; }
}

public class GuildV3
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("rank")]
    public string Rank { get; set; }
}

public class HouseV3
{
    [JsonProperty("houseid")]
    public int Houseid { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("paid")]
    public string Paid { get; set; }

    [JsonProperty("town")]
    public string Town { get; set; }
}

public class DeathV3
{
    [JsonProperty("assists")]
    public AssistV3[] Assists { get; set; }

    [JsonProperty("killers")]
    public AssistV3[] Killers { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

public class AssistV3
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

public class OtherCharacterV3
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

