using MakerSquare.FrontFacingECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForwardLayoutTest.Classes
{
    public interface IComponent
    {
        //DataFiller dataFiller { get; set; }
        Entities.Entity selectedEntity { get; set; }
        List<FFComponent> engineData { get; set; }

        void FillData();
        void AddSerializedComponent(Entities.Entity entity);
    }
    
    public interface ISerializedComponent
    {
        void SetComponentData(Entities.Entity entity);
    }
}
