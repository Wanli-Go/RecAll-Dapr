﻿using Ddd.Domain.SeedWork;

namespace List.Domain.AggregateModels.ListAggregate;

public interface IListRepository : IRepository<List>
{
    List Add(List list);

    Task<List> GetAsync(int listId, string userIdentityGuid);
}