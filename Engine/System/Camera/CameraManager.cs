using Ultraviolet;

namespace Engine.System.Camera
{
    public static class CameraManager
    {
        private static UltravioletContext _context;

        public static void Initialize(UltravioletContext context)
        {
            _context = context;
        }

        public static void CreateViewPort(Rectangle rec)
        {
            Ultraviolet.Graphics.Viewport viewport = new Ultraviolet.Graphics.Viewport(rec);

            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ChangeMaxDepth(int Maxdepth)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.MaxDepth = Maxdepth;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ChangeMinDepth(int Mindepth)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.MinDepth = Mindepth;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ChangePositionX(int x)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.X = x;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ChangePositionY(int y)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.Y = y;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ChangeHeight(int height)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.Height = height;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void MoveCameraX(int x)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.X = viewport.X + x;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void MoveCameraY(int y)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.Y = viewport.Y + y;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void MoveCameraHeight(int height)
        {
            Ultraviolet.Graphics.Viewport viewport = _context.GetGraphics().GetViewport();

            viewport.Height = viewport.Height + height;
            _context.GetGraphics().SetViewport(viewport);
        }

        public static void ClearView(Color color)
        {
            _context.GetGraphics().Clear(color);
        }

    }
}
