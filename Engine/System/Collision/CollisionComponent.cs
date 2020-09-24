using System;
using System.Collections.Generic;
using Engine.Components;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using Ultraviolet;
using Ultraviolet.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Engine.System.Collision
{
    [Serializable]
    public class CollisionComponent : Component
    {
        public bool Added = false;
        public bool Initialized { get; private set; }

        public Color[] colorDebug;
        private List<UpdateType> _updates = new List<UpdateType>();
        private HitBoxType _hitBoxType;
        
        public int hitBoxWidth;
        public int hitBoxHeight;
        private float _radius;
        private Category _collidesWith; public Category CollidesWith { set { _collidesWith = value; _updates.Add(UpdateType.COLLIDES_WITH); }}
        private Category _categoryCollision; public Category CategoryCollision {set { _categoryCollision = value; _updates.Add(UpdateType.CATEGORY_COLLISION); }}
        private BodyType _bodyType; public BodyType BodyType {set { _bodyType = value; _updates.Add(UpdateType.BODY_TYPE); }}
        private float _mass; public float Mass{ set { _mass = value; _updates.Add(UpdateType.MASS); }}
        private float _inertia; public float Inertia { set { _inertia = value; _updates.Add(UpdateType.INERTIA); }}
        private float _restitution; public float Restitution { set { _restitution = value; _updates.Add(UpdateType.RESTITUTION); }}
        private bool _ignoreGravity; public bool IgnoreGravity { set { _ignoreGravity = value; _updates.Add(UpdateType.IGNORE_GRAVITY); }}
        private float _forceAmount; public float ForceAmount { set { _forceAmount = value; _updates.Add(UpdateType.FORCE_AMOUNT); }}
        private float _torque; public float Torque { set { _torque = value; _updates.Add(UpdateType.TORQUE); }}
        private float _friction; public float Friction { set { _friction = value; _updates.Add(UpdateType.FRICTION); }}
        private bool _isBullet; public bool IsBullet { set { _isBullet = value; _updates.Add(UpdateType.IS_BULLET); }}
        private bool _ignoreCcd; public bool IgnoreCcd { set { _ignoreCcd = value; _updates.Add(UpdateType.IGNORE_CCD); }}
        private bool _isSensor; public bool IsSensor { set { _isSensor = value; _updates.Add(UpdateType.IS_SENSOR); }}
        private int _fixtureDivide; public int FixtureDivide { set => _fixtureDivide = value; }

        private float _linearX, _linearY;

        private List<OnCollisionEventHandler> _onCollisionEventHandlers;
        private List<BeforeCollisionEventHandler> _beforeCollisionEventHandlers;
        private List<AfterCollisionEventHandler> _afterCollisionEventHandlers;
        private List<OnCollisionEventHandler> _fixtureOnCollisionEventHandlers;

        public enum HitBoxType
        {
            SQUARE,
            CIRLE
        }
        
        public enum UpdateType
        {
            ON_COLLISION_EVENT_HANDLER,
            BEFORE_COLLISION_EVENT_HANDLER,
            AFTER_COLLISION_EVENT_HANDLER,
            FIXTURE_ON_COLLISION_EVENT_HANDLER,
            COLLIDES_WITH,
            CATEGORY_COLLISION,
            BODY_TYPE,
            MASS,
            INERTIA,
            RESTITUTION,
            IGNORE_GRAVITY,
            FORCE_AMOUNT,
            TORQUE,
            FRICTION,
            LINEAR_VELOCITY,
            IS_BULLET,
            IGNORE_CCD,
            IS_SENSOR
        }
        
        [NonSerialized] public Texture2D debugTexture;
        
        [NonSerialized] public Body body;      
        [NonSerialized] public Vector2 force = Vector2.Zero;

        public CollisionComponent(Entity entity, int width, int height) : base(entity)
        {
            _hitBoxType = HitBoxType.SQUARE;
            hitBoxWidth = width;
            hitBoxHeight = height;
            colorDebug = new Color[hitBoxWidth * hitBoxHeight];
            for(var i = 0; i < colorDebug.Length; ++i) colorDebug[i] = Color.Red;
        }

        public CollisionComponent(Entity entity, float radius) : base(entity)
        {
            _hitBoxType = HitBoxType.CIRLE;
            _radius = radius;
            hitBoxWidth = (int) radius * 2;
            hitBoxHeight = (int) radius * 2;
            colorDebug = new Color[hitBoxWidth * hitBoxHeight];
            for(var i = 0; i < colorDebug.Length; ++i) colorDebug[i] = Color.Red;
        }

        private List<Vector2> CreatePointVectors()
        {
            var points = new List<Vector2>();
            var size = (float) hitBoxWidth / _fixtureDivide;
            var initial_point = new Vector2(-(float)hitBoxWidth / 2, (float)hitBoxHeight / 2);
            points.Add(new Vector2(initial_point.X, initial_point.Y));
            points.Add(new Vector2(initial_point.X, -initial_point.Y));              
            for (var i = 0; i != _fixtureDivide; i++)
            {
                initial_point.X += size;
                points.Add(new Vector2(initial_point.X, initial_point.Y));
                points.Add(new Vector2(initial_point.X, -initial_point.Y));
            }
            return points;
        }

        private List<Vertices> CreateVertices()
        {
            var points = CreatePointVectors();
            var vertices = new List<Vertices>();
            for (var i = 0; i < points.Count - 2; i += 2)
            {
                var vertice = new Vertices();
                vertice.Add(points[i]);
                vertice.Add(points[i + 1]);
                vertice.Add(points[i + 2]);
                vertice.Add(points[i + 3]);
                vertices.Add(vertice);
            }
            return vertices;
        }

        private void CreateFixtures()
        {
            var vertices = CreateVertices();
            foreach (var vertice in vertices)
                body.CreatePolygon(vertice, Entity.Transform.depth);
        }

        public void BuildComponent(World world)
        {
            if (_hitBoxType == HitBoxType.CIRLE)
                body = world.CreateCircle(_radius, Entity.Transform.depth, Entity.Transform.ToVector2MX());
            else if (_hitBoxType == HitBoxType.SQUARE && _fixtureDivide == 0)
                body = world.CreateRectangle(hitBoxWidth, hitBoxHeight, Entity.Transform.depth, Entity.Transform.ToVector2MX());
            else if (_hitBoxType == HitBoxType.SQUARE && _fixtureDivide > 1)
            {
                body = world.CreateBody(Entity.Transform.ToVector2MX());
                CreateFixtures();
            }
            else
                throw new Exception("SQUARE HITBOX must be superior to 1");
            body.Tag = Entity.Guid;  
            Initialized = true;
        }

        public void SetLinearVelocity(Vector2 linearVelocity)
        {
            _linearX = linearVelocity.X;
            _linearY = linearVelocity.Y;
            _updates.Add(UpdateType.LINEAR_VELOCITY);
        }


        public void AddOnCollisionHandler(OnCollisionEventHandler collisionEventHandler)
        {
            if (_onCollisionEventHandlers == null)
                _onCollisionEventHandlers = new List<OnCollisionEventHandler>();
            _onCollisionEventHandlers.Add(collisionEventHandler);
            _updates.Add(UpdateType.ON_COLLISION_EVENT_HANDLER);
        }

        public void AddBeforeCollisionEventHandler(BeforeCollisionEventHandler beforeCollisionEventHandler)
        {
            if (_beforeCollisionEventHandlers == null)
                _beforeCollisionEventHandlers = new List<BeforeCollisionEventHandler>();
            _beforeCollisionEventHandlers.Add(beforeCollisionEventHandler);
            _updates.Add(UpdateType.BEFORE_COLLISION_EVENT_HANDLER);
        }

        public void AddAfterCollisionEventHandler(AfterCollisionEventHandler afterCollisionEventHandler)
        {
            if (_afterCollisionEventHandlers == null)
                _afterCollisionEventHandlers = new List<AfterCollisionEventHandler>();
            _afterCollisionEventHandlers.Add(afterCollisionEventHandler);
            _updates.Add(UpdateType.AFTER_COLLISION_EVENT_HANDLER);
        }
        
        public void AddRangeFixtureOnCollisionEventHandler(List<OnCollisionEventHandler> onCollisionEventHandlers)
        {
            if (_fixtureOnCollisionEventHandlers == null)
                _fixtureOnCollisionEventHandlers = new List<OnCollisionEventHandler>();
            _fixtureOnCollisionEventHandlers.AddRange(onCollisionEventHandlers);
            _updates.Add(UpdateType.FIXTURE_ON_COLLISION_EVENT_HANDLER);
        }

        private void SetFixtureOnCollisionEventHandler()
        {
            if (_fixtureOnCollisionEventHandlers.Count != body.FixtureList.Count)
                throw new Exception("ERROR must be the count for all");
            for (var i = 0; i < _fixtureOnCollisionEventHandlers.Count; i++)
                body.FixtureList[i].OnCollision = _fixtureOnCollisionEventHandlers[i];
        }

        public void UpdateComponent()
        {
            foreach (var update in _updates)
            {
                switch (update)
                {
                    case UpdateType.MASS:
                        body.Mass = _mass;
                        break;
                    case UpdateType.INERTIA:
                        body.Inertia = _inertia;
                        break;
                    case UpdateType.BODY_TYPE:
                        body.BodyType = _bodyType;
                        break;
                    case UpdateType.RESTITUTION:
                        body.SetRestitution(_restitution);
                        break;
                    case UpdateType.COLLIDES_WITH:
                        body.SetCollidesWith(_collidesWith);
                        break;
                    case UpdateType.IGNORE_GRAVITY:
                        body.IgnoreGravity = _ignoreGravity;
                        break;
                    case UpdateType.CATEGORY_COLLISION:
                        body.SetCollisionCategories(_categoryCollision);
                        break;
                    case UpdateType.FRICTION:
                        body.SetFriction(_friction);
                        break;
                    case UpdateType.LINEAR_VELOCITY:
                        body.LinearVelocity = new Vector2(_linearX, _linearY);
                        break;
                    case UpdateType.IS_BULLET:
                        body.IsBullet = _isBullet;
                        break;
                    case UpdateType.IGNORE_CCD:
                        body.IgnoreCCD = _ignoreCcd;
                        break;
                    case UpdateType.IS_SENSOR:
                        body.SetIsSensor(_isSensor);
                        break;
                    case UpdateType.ON_COLLISION_EVENT_HANDLER:
                        foreach (var onCollisionEventHandler in _onCollisionEventHandlers)
                            body.OnCollision += onCollisionEventHandler;
                        break;
                    case UpdateType.BEFORE_COLLISION_EVENT_HANDLER:
                        foreach (var beforeCollisionEventHandler in _beforeCollisionEventHandlers)
                            body.FixtureList[0].BeforeCollision += beforeCollisionEventHandler;
                        break;
                    case UpdateType.AFTER_COLLISION_EVENT_HANDLER:
                        foreach (var afterCollisionEventHandler in _afterCollisionEventHandlers)
                            body.FixtureList[0].AfterCollision += afterCollisionEventHandler;
                        break;
                    case UpdateType.FIXTURE_ON_COLLISION_EVENT_HANDLER:
                        SetFixtureOnCollisionEventHandler();
                        break;
                    default:
                        throw new InvalidOperationException("This value doesn't exist");
                }   
            }
            _updates.Clear();
        }
    }
}