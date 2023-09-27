using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace RabbitMqSubscriber.Dtos;

public class CombinedCharacterCorrelation
{
    public CharacterCorrelation FirstCombinedCorrelation { get; set; }
    public CharacterCorrelation SecondCombinedCorrelation { get; set; }
}