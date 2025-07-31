namespace Recruitment.Core.Common;

public abstract class AggregateRoot<T>(T id)
{
    public T Id { get; init; } = id;

    private readonly List<IDomainEvent> _domainEvents = new();
        
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    protected AggregateRoot() : this(default(T)!)
    {
    }
}
