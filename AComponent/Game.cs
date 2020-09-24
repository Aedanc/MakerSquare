using Ultraviolet;
using Ultraviolet.OpenGL;

namespace Test
{
    public partial class Game : UltravioletApplication
    {
        public Game() : base("Makers²", "Makers² Engine")
        {

        }

        protected override UltravioletContext OnCreatingUltravioletContext()
        {
            return new OpenGLUltravioletContext(this);
        }


    }
}
