namespace Fitmaniac.Domain.Exceptions;

public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entity, object key)
        : base($"Entity '{entity}' with key '{key}' was not found.") { }
}
