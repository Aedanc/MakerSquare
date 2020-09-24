using System;
using System.Collections.Generic;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using Ultraviolet;

namespace Engine.System.Collision
{
    public class CollisionManager
    {
        private static World _currentWorld;
        private static List<CollisionComponent> _colisionComponents = new List<CollisionComponent>();

        public static void Initialize(World world)
        {
            // world.Gravity = new Vector2(0, 20f);
            world.ContactManager.VelocityConstraintsMultithreadThreshold = 256;
            world.ContactManager.PositionConstraintsMultithreadThreshold = 256;
            world.ContactManager.CollideMultithreadThreshold = 256;
        }

        public static void  UpdateComponents()
        {
            if (_currentWorld.BodyList != null && _currentWorld.BodyList.Count != 0)
                Debug.WriteLine(_currentWorld.BodyList.Count);
            foreach (var component in _colisionComponents)
            {
                if (!component.Initialized)
                    component.BuildComponent(_currentWorld);
                component.UpdateComponent();
            }
        }

        public static void RemoveCollisionComponent(Entity entity)
        {
            for (int i = 0; i < _colisionComponents.Count; i++)
            {
                if (_colisionComponents[i].Entity == entity)
                    _colisionComponents.RemoveAt(i);                    
            }
        }

        public static void SetCurrentWorld(World world)
        {
            _currentWorld = world;
        }

        private static void AddCollisionComponent(CollisionComponent collisionComponent)
        {
            _colisionComponents.Add(collisionComponent);
        }

        public static void OnUpdateCollision(UltravioletTime time)
        {
            if (_colisionComponents.Count == 0)
                return;
            _currentWorld.Step(Math.Min((float)time.ElapsedTime.TotalSeconds, 1f / 30f));
        }

        public static void UpdatePositionEntities()
        {
            foreach (var component in _colisionComponents)
                component.Entity.Transform = new Position(component.body.Position.X, component.body.Position.Y);            
        }

        public static void RemoveBody(Body body)
        {
            _currentWorld.Remove(body);
        }
        
        public static void FetchCollisionComponent()
        {
            foreach (Entity entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<CollisionComponent>();
                if (comp != null && comp.Added != true)
                {
                    comp.Added = true;
                    AddCollisionComponent(comp);
                }
            }
        }
    }
}