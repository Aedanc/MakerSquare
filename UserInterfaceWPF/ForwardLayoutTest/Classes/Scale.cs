using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForwardLayoutTest
{
    [Serializable]
    public class Scale
    {
        int x;
        int y;

        Scale(int _x = 0, int _y = 0)
        {
            x = _x;
            y = _y;
        }
    }
}
