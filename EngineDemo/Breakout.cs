using System;
using System.Collections.Generic;
using Engine;
using Engine.System.Rule;

namespace EngineDemo
{
    [Serializable]
    public class Breakout: Entity
    {
        public int nbBall;
        public int score;
        public List<Brick> wall;
        public int nbBrick;
        bool activeWatcher;
        Ball ball;

        public Breakout(int nbball, int scorebase, Ball Ball)
        {
            Name = "Breakout";
            nbBall = nbball;
            score = scorebase;
            nbBrick = 0;
            wall = new List<Brick>();
            AddComponent(new RuleComponent(this));
            ball = Ball;
            activeWatcher = false;
            GetComponent<RuleComponent>().WatcherVarEntity("nbBall", 0, "Breakout", QuitGame, "none", this, "none");
        }

        public void QuitGame(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            //Quit the game or new scene : print score etc
        }
        
        public void BuildWall(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            //Console.WriteLine("We are gonna build the wall !");
        }

        //watcher nb brick, if 0 -> new game, save of the score
        //level of the ball ?(add this in the ball)
        // 
    }
}
