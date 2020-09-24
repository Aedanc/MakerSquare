using System.Collections.Generic;
using Engine.System.Movement;
using Microsoft.Xna.Framework;

namespace Engine.System.Collision
{
    public sealed class MovementComponentBuilder
    {
        private MovementComponent _movementComponent;

        private MovementComponentBuilder() {}

        public static MovementComponentBuilder CreateNew()
        {
            return new MovementComponentBuilder();
        }
        
        public MovementComponent Build()
        {
            return _movementComponent;    
        }

        public MovementComponentBuilder Init(Entity entity, bool constantForce = false)
        {
            _movementComponent = new MovementComponent(entity, constantForce);
            return this;
        }

        public MovementComponentBuilder SetPosition(Vector2 position)
        {
            _movementComponent.AddPosition(position);
            return this;
        }

        public MovementComponentBuilder SetDirection(Vector2 direction)
        {
            _movementComponent.Direction = direction;
            return this;
        }

        public MovementComponentBuilder SetPositions(List<Vector2> positions)
        {
            _movementComponent.AddPositions(positions); 
            return this;
        }

        public MovementComponentBuilder SetForceAmount(float forceAmount)
        {
            _movementComponent.forceAmount = forceAmount;
            return this;
        }

    }
}