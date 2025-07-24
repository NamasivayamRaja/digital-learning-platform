﻿namespace DigitalLearningPlatform.BuildingBlocks.Common.Domain
{
    public abstract class EntityBase
    {
        public Guid Id { get; protected set; }
        protected EntityBase()
        {
            Id = Guid.NewGuid();
        }

        protected EntityBase(Guid id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EntityBase other) return false;

            if (ReferenceEquals(this, other)) return true;

            if (GetType() != other.GetType()) return false;

            if (Id == Guid.Empty || other.Id == Guid.Empty) return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(EntityBase left, EntityBase right)
        {
            if(left is null && right is null) return true;

            if(left is null || right is null) return false;

            return left.Equals(right);
        }

        public static bool operator !=(EntityBase left, EntityBase right) { return !(left == right); }

    }
}
