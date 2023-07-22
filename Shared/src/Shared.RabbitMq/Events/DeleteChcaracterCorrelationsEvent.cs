﻿namespace Shared.RabbitMQ.Events;

/// <summary>
/// Event to delete all correlations
/// </summary>
/// <param name="CharacterId"></param>
public record DeleteChcaracterCorrelationsEvent(int CharacterId);
