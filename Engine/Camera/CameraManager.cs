using Ultraviolet;

namespace Component 
{
    class CameraManager : Component
    {
        public void CreateViewPort(UltravioletContext context, Rectangle rec)
        {
            Ultraviolet.Graphics.Viewport viewport = new Ultraviolet.Graphics.Viewport(rec);

            context.GetGraphics().SetViewport(viewport);
        }

        public void ChangeMaxDepth(UltravioletContext context, int Maxdepth)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.MaxDepth = Maxdepth;
           context.GetGraphics().SetViewport(viewport);
        }

        public void ChangeMinDepth(UltravioletContext context, int Mindepth)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.MinDepth = Mindepth;
            context.GetGraphics().SetViewport(viewport);
        }

        public void ChangePositionX(UltravioletContext context, int x)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

           viewport.X = x;
            context.GetGraphics().SetViewport(viewport);
        }

        public void ChangePositionY(UltravioletContext context, int y)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.Y = y;
            context.GetGraphics().SetViewport(viewport);
        }

        public void ChangeHeight(UltravioletContext context, int height)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.Height = height;
            context.GetGraphics().SetViewport(viewport);
        }

        public void MoveCameraX(UltravioletContext context, int x)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.X = viewport.X + x;
            context.GetGraphics().SetViewport(viewport);
        }

        public void MoveCameraY(UltravioletContext context, int y)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.Y = viewport.Y + y;
            context.GetGraphics().SetViewport(viewport);
        }

        public void MoveCameraHeight(UltravioletContext context, int height)
        {
            Ultraviolet.Graphics.Viewport viewport = Ultraviolet.GetGraphics().GetViewport();

            viewport.Height = viewport.Height + height;
            context.GetGraphics().SetViewport(viewport);
        }

        public void ClearView(UltravioletContext context, Color color)
        { 
            context.GetGraphics().Clear(color);
        }

        public CameraManager(UltravioletContext context) : base(context)
        {
        }
    }
}
