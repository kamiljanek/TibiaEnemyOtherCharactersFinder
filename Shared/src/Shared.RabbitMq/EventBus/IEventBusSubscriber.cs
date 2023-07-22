﻿using EkoProTech.Shared.RabbitMQ.EventBus;

namespace Shared.RabbitMQ.EventBus;

public interface IEventBusSubscriber
{
    void Subscribe<T, TResult>(EventBusHandler<T, TResult> handler) where T : class;
}