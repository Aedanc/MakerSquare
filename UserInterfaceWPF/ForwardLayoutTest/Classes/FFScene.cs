using System;
using System.Collections.Generic;
using Entities;
using MakerSquare.FrontFacingECS;

namespace ForwardLayoutTest
{
    [Serializable]
    public class FFScene
    {
        public FFScene()
        {
            entities = new List<FFEntity>();
        }

        public FFScene(List<FFEntity> entities_)
        {
            entities = entities_;
        }

        public List<FFEntity> entities;
    }
}

