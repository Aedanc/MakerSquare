using System.Collections.Generic;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace Engine.System.Collision
{
    public sealed class CollisionComponentBuilder
    {
        private CollisionComponent _collisionComponent;

        private CollisionComponentBuilder() {}

        public static CollisionComponentBuilder CreateNew()
        {
            return new CollisionComponentBuilder();
        }
        
        public CollisionComponent Build()
        {
            return _collisionComponent;    
        }

        public CollisionComponentBuilder Init(Entity entity, Vector2 size)
        {
            _collisionComponent = new CollisionComponent(entity, (int) size.X, (int) size.Y);
            return this;
        }

        public CollisionComponentBuilder Init(Entity entity, float radius)
        {
            _collisionComponent = new CollisionComponent(entity, radius);
            return this;
        }

        public CollisionComponentBuilder SetCollidesWith(Category category)
        {
            _collisionComponent.CollidesWith = category;
            return this;
        }

        public CollisionComponentBuilder SetBodyType(BodyType bodyType)
        {
            _collisionComponent.BodyType = bodyType;
            return this;
        }

        public CollisionComponentBuilder SetCategoriesCollision(Category category)
        {
            _collisionComponent.CategoryCollision = category;
            return this;
        }

        public CollisionComponentBuilder SetMass(float mass)
        {
            _collisionComponent.Mass = mass;
            return this;
        }

        public CollisionComponentBuilder SetInertia(float inertia)
        {
            _collisionComponent.Inertia = inertia;
            return this;
        }

        public CollisionComponentBuilder SetRestitution(float restitution)
        {
            _collisionComponent.Restitution = restitution;
            return this;
        }

        public CollisionComponentBuilder SetIgnoreGravity(bool ignoreGravity)
        {
            _collisionComponent.IgnoreGravity = ignoreGravity;
            return this;
        }

        public CollisionComponentBuilder SetLinearVelocity(Vector2 linearVelocity)
        {
            _collisionComponent.SetLinearVelocity(linearVelocity);
            return this;
        }

        public CollisionComponentBuilder SetForceAmount(float force)
        {
            _collisionComponent.ForceAmount = force;
            return this;
        }

        public CollisionComponentBuilder SetTorque(float torque)
        {
            _collisionComponent.Torque = torque;
            return this;
        }

        public CollisionComponentBuilder SetFriction(float friction)
        {
            _collisionComponent.Friction = friction;
            return this;
        }

        public CollisionComponentBuilder SetIsBullet(bool enable)
        {
            _collisionComponent.IsBullet = enable;
            return this;
        }

        public CollisionComponentBuilder SetIgnoreCCD(bool enable)
        {
            _collisionComponent.IgnoreCcd = enable;
            return this;
        }

        public CollisionComponentBuilder AddBeforeCollisionEventHandler(BeforeCollisionEventHandler beforeCollisionEventHandler)
        {
            _collisionComponent.AddBeforeCollisionEventHandler(beforeCollisionEventHandler);
            return this;
        }

        public CollisionComponentBuilder AddAfterCollisionEventHandler(AfterCollisionEventHandler afterCollisionEventHandler)
        {
            _collisionComponent.AddAfterCollisionEventHandler(afterCollisionEventHandler);
            return this;
        }

        public CollisionComponentBuilder SetIsSensor(bool enable)
        {
            _collisionComponent.IsSensor = enable;
            return this;
        }

        public CollisionComponentBuilder SetHitBoxDivide(int divider)
        {
            _collisionComponent.FixtureDivide = divider;
            return this;
        }

        public CollisionComponentBuilder SetFixtureOnCollisionEventHandlers(List<OnCollisionEventHandler> onCollisionEventHandlers)
        {
            _collisionComponent.AddRangeFixtureOnCollisionEventHandler(onCollisionEventHandlers);
            return this;
        }
        

        public CollisionComponentBuilder AddOnCollisionEventHandler(OnCollisionEventHandler collisionEventHandler)
        {
            _collisionComponent.AddOnCollisionHandler(collisionEventHandler);
            return this;
        }
    }
}