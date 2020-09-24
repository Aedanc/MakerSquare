using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine;
using Engine.System.Collision;
using Engine.System.Graphics;
using Engine.System.Input;
using Engine.System.Movement;
using Engine.System.Rule;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using Ultraviolet.Input;
using Game = Engine.Core.Game;

namespace EngineDemo
{
    [Serializable]
    public sealed class SantaBuilder : Entity
    {
        private Entity santaEnt;
        
        private SantaBuilder() {}
        
        public static SantaBuilder CreateNew()
        {
            return new SantaBuilder();
        }

        public SantaBuilder Init(Game game)
        {
            santaEnt = new Entity(true);
            santaEnt.Transform.x = 100;
            santaEnt.Transform.y = 200;
            CreateSantaAudioComponent();
            CreateSantaSpriteComponent();
            CreateSantaCollisionComponent();
           CreateSantaMovementComponent();
            CreateSantaInputComponent();
            CreateRuleComponent();
            return this;
        }

        public Entity Build()
        {
            return santaEnt;
        }

        private void CreateRuleComponent()
        {
            santaEnt.AddComponent(new RuleComponent(santaEnt));
            //TODO Fix missing/wrong code
            //      var engineMessage = new RuleManager.EngineMessage();
            //    engineMessage.entitybase = santaEnt;
            // engineMessage.entityFocus = EntityManager.GetEntityPreload("brick0");
            //     engineMessage.Action = RuleManager.ActionEngine.Collision;
            //   santaEnt.GetComponent<RuleComponent>().WatcherActionEngine(engineMessage, AddHp, "toto", santaEnt, "mabite");
        }
        
        private void CreateSantaAudioComponent()
        {
            //  santa_ent.AddComponent(new Engine.System.Audio.AudioComponent(santa_ent, "NyanCat", false, false, true));
        }

        private void CreateSantaMovementComponent()
        {
            var position = new List<Vector2>();
            position.Add(new Vector2(300, 400));
            position.Add(new Vector2(100, 150));
            santaEnt.AddComponent(MovementComponentBuilder.CreateNew()
                .Init(santaEnt)
                //.SetPosition(new Vector2(300, 100))
                .SetForceAmount(5000)
                // .SetPositions(position)
                //    .SetForceAmount(3000)
                //.SetForceAmount(5000)
                .Build());
        }

        private void CreateSantaSpriteComponent()
        {
            santaEnt.AddComponent(new SpriteComponent(santaEnt, "LittleCircle", true));
        }

        private void CreateSantaCollisionComponent()
        {
            OnCollisionEventHandler print_collide = (sender, other, contact) =>
            {
                Debug.WriteLine("entity controlled:" + sender.Body.Tag + " entity collide:" + other.Body.Tag);
                Debug.WriteLine("res" + contact.FixtureA.Restitution);
                //TODO Fix missing/wrong code
                //  var tmp = new RuleManager.EngineMessage();
                // tmp.Action = RuleManager.ActionEngine.Collision;
                // tmp.entitybase = EntityManager.GetAllEntities().Find(i =>  i.Guid == (Guid)sender.Tag);
                //tmp.entityFocus = EntityManager.GetAllEntities().Find(i => i.Guid == (Guid)other.Tag);
                //   RuleManager.EngineAction.Enqueue(tmp);
                return true;
            }; 
            
            santaEnt.AddComponent(CollisionComponentBuilder.CreateNew()
                .Init(santaEnt, 25)
                .SetBodyType(BodyType.Dynamic)
                .SetCollidesWith(Category.All)
                .SetCategoriesCollision(Category.Cat1)
                //   .SetMass(1f)
                 // .SetInertia(1f)
                .SetLinearVelocity(new Vector2(300, 0))
                .SetRestitution(1f)
              //  .SetIgnoreCCD(true)
                //       .SetForce(100f)
                .AddOnCollisionEventHandler(print_collide)
                .Build());
        }

        private void CreateSantaInputComponent()
        {
            var santaInput = new InputComponent(santaEnt);
            ActionDelegate walk_up_down = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().MoveToUp();
            };
            ActionDelegate walk_down_down = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().MoveToDown();
            };
            ActionDelegate walk_right_down = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().MoveToRight();
            };
            ActionDelegate walk_left_down = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().MoveToLeft();
            };
            
            ActionDelegate walk_up_up = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.UP);
            };
            ActionDelegate  walk_down_up = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.DOWN);
            };
            ActionDelegate walk_left_up = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.LEFT);
            };
            ActionDelegate walk_right_up = (entity, dict) =>
            {
                entity.GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.RIGHT);
            };
            ActionDelegate debug_hitbox = (entity, dict) => { GraphicManager.DebugHitbox(); };

            santaInput.AddAction("walk_up", OnKey.DOWN, Key.Up, null, walk_up_down);
    //        santaInput.AddAction("walk_up", OnKey.UP, Key.Up, null, walk_up_up);
            santaInput.AddAction("walk_down", OnKey.DOWN, Key.Down, null, walk_down_down);
    //        santaInput.AddAction("walk_down", OnKey.UP, Key.Down, null, walk_down_up);
            santaInput.AddAction("walk_left", OnKey.DOWN, Key.Left, null, walk_left_down);
     //       santaInput.AddAction("walk_left", OnKey.UP, Key.Left, null, walk_left_up);
            santaInput.AddAction("walk_right", OnKey.DOWN, Key.Right, null, walk_right_down);
     //       santaInput.AddAction("walk_right", OnKey.UP, Key.Right, null, walk_right_up);           
            santaInput.AddAction("debug_hitbox", OnKey.PRESSED, Key.F1, null, debug_hitbox);

            santaEnt.AddComponent(santaInput);
        }
    }
}