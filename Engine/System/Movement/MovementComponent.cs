using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine.Components;
using Engine.System.Collision;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Engine.System.Movement
{
    [Serializable]
    public class MovementComponent : Component
    {
        public bool Added = false;
        [NonSerialized] private List<Vector2> _positions;
        [NonSerialized] private Vector2 _direction; public Vector2 Direction { set { _direction = value; _directionReached = false;  _typeMovement = TypeMovement.DIRECTION; }}
       
        private bool _constantForce;
        private bool _directionReached;
        private bool _positionReached;
        public float forceAmount;
        private bool _moveRight, _moveLeft, _moveUp, _moveDown;
        [NonSerialized] private Vector2 _force = Vector2.Zero;
        [NonSerialized] private Vector2 _lastBodyPosition;
        [NonSerialized] private Vector2 _positionToReached;
        
        public enum DirectionMovement
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }
        
        private TypeMovement _typeMovement = TypeMovement.PLAYER_CONTROLLED; public TypeMovement MovementType => _typeMovement;

        public enum TypeMovement
        {
            PLAYER_CONTROLLED,
            DIRECTION,
            POSITION
        }
        
        public MovementComponent(Entity entity, bool constantForce) : base(entity)
        {
            _constantForce = constantForce;
            _lastBodyPosition = new Vector2(Entity.Transform.x, Entity.Transform.y);
        }

        public void MoveToPosition()
        {
            if (_positionReached)
                MovementVector();
        }

        public void CorrectPosition()
        {
            _lastBodyPosition = Entity.Transform.ToVector2MX();
            if (_typeMovement != TypeMovement.POSITION)
                return;
            var body = Entity.GetComponent<CollisionComponent>().body;
            var transform = body.GetTransform();
            if (!(_lastBodyPosition.X < transform.p.X && transform.p.X <= _positionToReached.X) && _force.X > 0)
            {
                transform.p = _positionToReached;
                if (_positions.Count > 0)
                    _positionReached = true;
            }
            if (!(_lastBodyPosition.Y < transform.p.Y && transform.p.Y <= _positionToReached.Y) && _force.Y > 0)
            {
                transform.p = _positionToReached;
                if (_positions.Count > 0)
                    _positionReached = true;
            }
            if (!(_lastBodyPosition.X > transform.p.X && transform.p.X >= _positionToReached.X) && _force.X < 0)
            {
                transform.p = _positionToReached;
                if (_positions.Count > 0)
                    _positionReached = true;
            }
            if (!(_lastBodyPosition.Y > transform.p.Y && transform.p.Y >= _positionToReached.Y) && _force.Y < 0)
            {
                transform.p = _positionToReached;
                if (_positions.Count > 0)
                    _positionReached = true;
            }
            body.SetTransform(transform.p, body.Rotation);
        }
        
        public void MoveToDirection()
        {
            if (_directionReached)
                return;
            _force = new Vector2(_direction.X * forceAmount, _direction.Y * forceAmount);
            Entity.GetComponent<CollisionComponent>().body.ApplyForce(_force);
            _directionReached = true;
        }

        public void StopMoveToDirection(DirectionMovement direction)
        {
            switch (direction)
            {
                case DirectionMovement.UP:
                    _moveUp = false;
                    break;
                case DirectionMovement.DOWN:
                    _moveDown = false;
                    break;
                case DirectionMovement.LEFT:
                    _moveLeft = false;
                    break;
                case DirectionMovement.RIGHT:
                    _moveRight = false;
                    break;
            }
        }

        public void MoveToUp()
        {
            _moveUp = true;
            _force += new Vector2(0, -forceAmount);
        }

        public void MoveToDown()
        {
            _moveDown = true;
            _force += new Vector2(0, forceAmount);
        }

        public void MoveToLeft()
        {
            _moveLeft = true;
            _force += new Vector2(-forceAmount, 0);
        }

        public void MoveToRight()
        {
            _moveRight = true;
            _force += new Vector2(forceAmount, 0);
        }

        public void ApplyMovement()
        {
            if (_moveUp == false && _moveDown == false && _moveLeft == false && _moveRight == false)
                Entity.GetComponent<CollisionComponent>().body.ResetDynamics();
            else
                Entity.GetComponent<CollisionComponent>().body.ApplyForce(_force);            
            if (!_constantForce)
                _force = Vector2.Zero;
        }

        public void AddPosition(Vector2 position)
        {
            if (_positions == null)
                _positions = new List<Vector2>();
            _positions.Add(position);
            _typeMovement = TypeMovement.POSITION;
            _positionReached = true;
        }

        public void AddPositions(List<Vector2> positions)
        {
            if (_positions == null)
                _positions = new List<Vector2>();
            _positions.AddRange(positions);
            _typeMovement = TypeMovement.POSITION;
            _positionReached = true;
        }

        private void MovementVector()
        {
            if (_positions.Count == 0 && _force != Vector2.Zero)
            {
                Entity.GetComponent<CollisionComponent>().body.ResetDynamics();
                _positionReached = false;
                _force = Vector2.Zero;
            }
            else if (_positions.Count > 0)
            {
                Entity.GetComponent<CollisionComponent>().body.ResetDynamics();
                _force = new Vector2((_positions[0].X - Entity.Transform.x) * forceAmount, (_positions[0].Y - Entity.Transform.y) * forceAmount);
                Entity.GetComponent<CollisionComponent>().body.ApplyForce(_force);
                Debug.WriteLine(_force.ToString());
                _positionToReached = _positions[0];
                _positionReached = false;
                _positions.RemoveAt(0);
            }
        }
    }
}