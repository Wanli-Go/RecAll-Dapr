using Ddd.Domain.SeedWork;
using List.Domain.Events;
using List.Domain.Exceptions;

namespace List.Domain.AggregateModels.ListAggregate;

public class List : Entity, IAggregateRoot // Write Only
{
    private string _name;
    private int _typeId;
    private string _userIdentityGuid;
    private bool _isDeleted;

    private void _checkDeleted()
    {
        if (_isDeleted)
        {
            ThrowDeletedException();
        }
    }
    private void ThrowDeletedException() =>
        throw new ListDomainException("列表已删除。");

    public bool IsDeleted => _isDeleted;
    public string UserIdentityGuid => _userIdentityGuid;

    public ListType Type { get; private set; }
    private List() { }

    // New
    public List(string name, int typeId, string userIdentityGuid) : this()
    {
        _name = name;
        _typeId = typeId;
        _userIdentityGuid = userIdentityGuid;

        var listCreatedDomainEvent = new ListCreatedDomainEvent(this);
        AddDomainEvent(listCreatedDomainEvent);
    }

    public void SetDeleted()
    {
        _checkDeleted();
        _isDeleted = true;
    }

    public void SetName(string name)
    {
        _checkDeleted();
        _name = name;
    }
}