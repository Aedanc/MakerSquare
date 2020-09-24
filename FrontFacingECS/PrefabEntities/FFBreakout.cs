using MakerSquare.FrontFacingECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakerSquare.FrontFacingECS.PrefabEntities
{
    public class FFBreakoutManagerComponent : FFComponent
    {
        public uint lives;
        
        public FFBreakoutManagerComponent(FFEntity entity) : base(entity)
        {
            entity.EntityTemplate = "Breakout";
        }

        public override string WriteComponentAddition()
        {
            throw new NotImplementedException();
        }
    }

    public class FFBreakoutBallComponent : FFComponent
    {
        public string spritename;        

        public FFBreakoutBallComponent(FFEntity entity) : base(entity)
        {
            entity.EntityTemplate = "Breakout";
        }

        public override string WriteComponentAddition()
        {
            throw new NotImplementedException();
        }
    }

    public class FFBreakoutBatComponent : FFComponent
    {
        public string spritename;        

        public FFBreakoutBatComponent(FFEntity entity) : base(entity)
        {
            entity.EntityTemplate = "Breakout";
        }

        public override string WriteComponentAddition()
        {
            throw new NotImplementedException();
        }
    }

    public class FFBreakoutBrickComponent : FFComponent
    {
        public string spritename;
        public int score;
        
        public FFBreakoutBrickComponent(FFEntity entity) : base(entity)
        {
            entity.EntityTemplate = "Breakout";
        }

        public override string WriteComponentAddition()
        {
            throw new NotImplementedException();
        }
    }

    public class FFBreakoutBoundaryComponent : FFComponent
    {
        public int size_x;
        public int size_y;
        public bool destroys_ball;

        public FFBreakoutBoundaryComponent(FFEntity entity) : base(entity)
        {
            entity.EntityTemplate = "Breakout";
        }

        public override string WriteComponentAddition()
        {
            throw new NotImplementedException();
        }        
    }
}
