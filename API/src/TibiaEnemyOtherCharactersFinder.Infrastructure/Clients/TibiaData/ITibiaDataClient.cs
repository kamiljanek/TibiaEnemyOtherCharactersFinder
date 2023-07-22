﻿using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

public interface ITibiaDataClient
{
    public Task<List<string>> FetchWorldsNames();
    public Task<string> FetchCharactersOnline(string name);
    Task<TibiaDataCharacterInformationResult> FetchCharacter(string name);
}