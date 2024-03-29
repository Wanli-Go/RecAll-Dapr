﻿using Ddd.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ddd.Infrastructure;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
    {
        var domainEntities = context.ChangeTracker.Entries<Entity>().Where(p =>
            p.Entity.DomainEvents != null && p.Entity.DomainEvents.Any());
        var domainEvents = domainEntities.SelectMany(p => p.Entity.DomainEvents).ToList();

        domainEntities.ToList().ForEach(domainEntity => domainEntity.Entity.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}