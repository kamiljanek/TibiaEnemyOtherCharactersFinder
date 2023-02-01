﻿using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public interface IRepository
{
    Task<List<World>> GetAvailableWorldsAsync();

    Task<List<World>> GetWorldsAsNoTrackingAsync();

    Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId);

    Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync();

    Task<List<short>> GetAvailableWorldIdsFromWorldScansAsync();

    Task SoftDeleteWorldScanAsync(WorldScan worldScan);

    Task AddCharactersActionsAsync(List<CharacterAction> characterActions);

    Task AddAsync<T>(T entity) where T : class, IEntity;

    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IEntity;

    Task UpdateWorldAsync(World entity);
}