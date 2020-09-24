using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;

namespace ForwardLayoutTest.Controller
{
    public class DeepCopy<T>
    {
        public static T DeepCopyMethod(T element)
        {
            var xaml = XamlWriter.Save(element);

            var xamlString = new StringReader(xaml);

            var xmlTextReader = new XmlTextReader(xamlString);

            var deepCopyObject = (T)XamlReader.Load(xmlTextReader);

            return deepCopyObject;
        }
    }
}
