using System;
using System.Collections.Generic;
using MakerSquare.FrontFacingECS.PrefabEntities;
using MakerSquare.FrontFacingECS;
using System.Linq;

namespace Codegen.SpecificEntities
{
    public partial class Breakout
    {
        FFBreakoutManagerComponent _manager;
        FFBreakoutBatComponent _bat;
        FFBreakoutBallComponent _ball;
        List<FFBreakoutBrickComponent> _brickList;
        List<FFBreakoutBoundaryComponent> _walls;

        public Breakout(List<FFEntity> entities)
        {
            _brickList = new List<FFBreakoutBrickComponent>();
            _walls = new List<FFBreakoutBoundaryComponent>();
            entities.ForEach(entity =>
            {
                if (entity.Components.Any(comp => comp.GetType() == typeof(FFBreakoutManagerComponent)))
                {
                    _manager = (FFBreakoutManagerComponent)entity.Components.Find(comp => comp.GetType() == typeof(FFBreakoutManagerComponent));
                }
                else if (entity.Components.Any(comp => comp.GetType() == typeof(FFBreakoutBatComponent)))
                {
                    _bat = (FFBreakoutBatComponent)entity.Components.Find(comp => comp.GetType() == typeof(FFBreakoutBatComponent));
                }
                else if (entity.Components.Any(comp => comp.GetType() == typeof(FFBreakoutBallComponent)))
                {
                    _ball = (FFBreakoutBallComponent)entity.Components.Find(comp => comp.GetType() == typeof(FFBreakoutBallComponent));
                }
                else if (entity.Components.Any(comp => comp.GetType() == typeof(FFBreakoutBrickComponent)))
                {
                    _brickList.Add((FFBreakoutBrickComponent)entity.Components.Find(comp => comp.GetType() == typeof(FFBreakoutBrickComponent)));
                }
                else if (entity.Components.Any(comp => comp.GetType() == typeof(FFBreakoutBoundaryComponent)))
                {
                    _walls.Add((FFBreakoutBoundaryComponent)entity.Components.Find(comp => comp.GetType() == typeof(FFBreakoutBoundaryComponent)));
                }
            });                                               
        }

        public List<ReificationData> ReificationData()
        {
            var bricks = new List<ReificationData>();
            for (int i = 0; i < _brickList.Count; i++)
            {
                bricks.Add(new ReificationData("Brick" + i,
                    $"new Brick(BreakoutManager, " +
                    $"new Vector2({_brickList[i].Entity.Transform.x}, " +
                    $"{_brickList[i].Entity.Transform.y}, " +
                    $"{_brickList[i].spritename}, " +
                    $"{_brickList[i].score})"));
            }
            _brickList.ForEach(brick => bricks.Add(new ReificationData()));

            var walls = new List<ReificationData>();
            for (int i = 0; i < _walls.Count; i++)
            {
                walls.Add(new ReificationData("Boundary" + i,
                   $"new InvisibleWall(new Vector2({_walls[i].Entity.Transform.x}, {_walls[i].Entity.Transform.y}), " +
                   $"new Vector2({_walls[i].size_x}, {_walls[i].size_y}), " +
                   $"{_walls[i].destroys_ball})"));
            }

            var breakouts = new List<ReificationData> { new ReificationData("BreakoutManager", "new Breakout(true)"),
                                                new ReificationData("BreakoutBall", "new Ball(true)"),
                                                new ReificationData("BreakoutBat", "new Bat(true)") };

            breakouts.AddRange(bricks);
            breakouts.AddRange(walls);
            return breakouts;
        }
    }
}
