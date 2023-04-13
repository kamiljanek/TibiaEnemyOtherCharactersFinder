using System.ComponentModel.DataAnnotations;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaDataApi;

public class TibiaDataApiSection
{
    public const string SectionName = "TibiaDataApi";

    [Required]
    public string BaseAddress { get; init; }
    [Required]
    public string ApiVersion { get; init; }
    [Required]
    public string Timeout { get; init; }
}