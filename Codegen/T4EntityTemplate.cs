using System.Collections.Generic;

namespace Codegen
{
    public partial class EntityTemplate
    {
        private string _className;
        private string _x_pos;
        private string _y_pos;
        private string _depth;
        private List<string> _components;

        public EntityTemplate(MakerSquare.FrontFacingECS.FFEntity entity, int nth)
        {
            this._className = entity.Name + nth;
            this._x_pos = entity.Transform.x.ToString();
            this._y_pos = entity.Transform.y.ToString();
            this._depth = entity.Transform.depth.ToString();

            _components = new List<string>();
            entity.Components.ForEach(c => _components.Add(c.WriteComponentAddition()));
        }

        public ReificationData ReificationData(MakerSquare.FrontFacingECS.FFEntity entity, string name)
        {
            ReificationData data = new ReificationData();
            data.reif_str = string.Format("new {0}(true)", name);
            data.varname = name;
            return data;
        }
    }
}
