namespace Pereprodai.Shared.Domain;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { } // EF Core

    public override bool Equals(object? obj)
    {
        if(obj is not Entity other) return false;
        if(ReferenceEquals(this, other)) return true;
        if(other.GetType() != GetType()) return false;
        return Id == other.Id;
    }

    public bool Equals(Entity? other) => Equals((object?)other);

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
