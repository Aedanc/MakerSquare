using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForwardLayoutTest.Classes
{
    public class DataFiller
    {
        public void FillData(object component, object engineData)
        {
            var componentFields = component.GetType().GetFields()
                .Where(prop => Attribute.IsDefined(prop, typeof(ComponentData)));
            var engineFields = engineData.GetType().GetFields();
            int i = 0;

            Console.WriteLine("Component fields");
            foreach (var field in componentFields)
            {
                Console.WriteLine("field: " + field.Name + " = " + field.GetValue(component));
                if (field.FieldType.Equals(engineFields[i].FieldType))
                    engineFields[i].SetValue(engineData, field.GetValue(component));
                ++i;
            }
            Console.WriteLine("=====");
            Console.WriteLine("Engine fields");
            foreach (var field in engineFields)
            {
                Console.WriteLine("field: " + field.Name + " = " + field.GetValue(engineData));
            }
        }
    }
}
