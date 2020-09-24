using System;
using System.Collections.Generic;

namespace Engine.System.Movement
{
    public class MovementManager
    {
        private static List<MovementComponent> _movements = new List<MovementComponent>();

        public static void Initialize() {}

        public static void AddMovementComponent(MovementComponent movementComponent)
        {
            _movements.Add(movementComponent);
        }

        public static void UpdateMovement()
        {
            foreach (var movement in _movements)
            {
                switch (movement.MovementType)
                {
                    case MovementComponent.TypeMovement.PLAYER_CONTROLLED: 
                        movement.ApplyMovement();
                        break;
                    case MovementComponent.TypeMovement.DIRECTION:
                        movement.MoveToDirection();
                        break;
                    case MovementComponent.TypeMovement.POSITION:
                        movement.MoveToPosition();
                        break;
                    default:
                        throw new Exception("Unknow or Invalid TypeMovement\n");
                }
            }
        }

        public static void UpdateCorrectionMovement()
        {
            foreach (var component in _movements)
                component.CorrectPosition();
        }
        
        public static void FetchMovementComponent()
        {
            foreach (var entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<MovementComponent>();
                if (comp != null && comp.Added != true)
                {
                    comp.Added = true;
                    AddMovementComponent(comp);
                }
            }
        }
    }
}