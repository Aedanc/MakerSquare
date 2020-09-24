using System;
using System.Collections.Generic;
using Engine;

namespace MakerSquare
{
    namespace SavingSystem
    {
        [Serializable()]
        public class SavedScene
        {
            public SavedScene()
            {
                entities = new List<Entity>();
            }

            public List<Entity> entities;
        }
    }
}
