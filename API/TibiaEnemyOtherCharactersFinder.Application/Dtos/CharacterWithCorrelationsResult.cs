﻿namespace TibiaEnemyOtherCharactersFinder.Application.Dtos
{
    public class CharacterWithCorrelationsResult
    {
        public int CorrelationId { get; set; }
        public string OtherCharacterName { get; set; }
        public short NumberOfMatches { get; set; }
    }
}