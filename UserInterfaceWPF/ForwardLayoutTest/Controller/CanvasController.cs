using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Entities;
using ForwardLayoutTest.Controller;

namespace ForwardLayoutTest
{
    class CanvasController
    {
        public static CanvasController instance = null;

        public bool bMultiSelect = false;

        private Canvas mainWindowCanvas;
        private ScrollViewer canvasScroller;
        private EntitiesController entitiesController;

        private Dimensions _canvas_default_size = new Dimensions();
        private Image canvasDraggedImage = null;
        private Entity draggedEntity = null;
        private Point mousePosition;

        private Rectangle selectionBox;
        private Point selectBoxAnchor = new Point();
        private bool bIsDragging = false;

        private double maxXScale = 5;
        private double maxYScale = 5;
        private double minXScale = 0.5;
        private double minYScale = 0.5;
        private double zoomValue = 0.15;

        public CanvasController(Canvas canvas, EntitiesController _entitiesController, ScrollViewer _canvasScroller)
        {
            if (instance == null)
            {
                instance = this;
                
                mainWindowCanvas = canvas;
                entitiesController = _entitiesController;
                canvasScroller = _canvasScroller;

                selectionBox = (Rectangle)mainWindowCanvas.FindName("SelectBox");
            }
        }

        public static CanvasController GetInstance()
        {
            return instance;
        }

        public Canvas GetCanvas()
        {
            return mainWindowCanvas;
        }

        public Dimensions GetCanvasDimensions()
        {
            return _canvas_default_size;
        }

        public ScrollViewer GetScrollViewe()
        {
            return canvasScroller;
        }

        public void SetCanvas(Canvas newCanvas)
        {
            mainWindowCanvas = newCanvas;
        }

        // --------------- Canvas Methods --------------- //

        public void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;
            
            if (image != null && mainWindowCanvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(mainWindowCanvas);
                canvasDraggedImage = image;
                draggedEntity = entitiesController.FindCanvasEntityByStringId(canvasDraggedImage.Name);

                Console.WriteLine("find: " + canvasDraggedImage.Name);

                if (draggedEntity != null && !draggedEntity.Selected() && bMultiSelect == false)
                {
                    entitiesController.UnselectAll();
                    ContextMenuController.GetInstance().ClearMenu();
                }

                draggedEntity.Select();
                ContextMenuController.GetInstance().ClearMenu();
                ContextMenuController.GetInstance().SelectEntity(draggedEntity);
            }
            else
            {
                entitiesController.UnselectAll();
                ContextMenuController.GetInstance().ClearMenu();
            }

            if (canvasDraggedImage == null)
            {
                selectBoxAnchor = e.GetPosition(mainWindowCanvas);
                bIsDragging = true;
            }
        }

        public void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectionBox.Visibility == Visibility.Visible)
                entitiesController.SelectEntitiesInZone(selectionBox);
            ResetRect();

            if (canvasDraggedImage != null)
            {
                mainWindowCanvas.ReleaseMouseCapture();

                Entity entity = entitiesController.FindCanvasEntityByStringId(canvasDraggedImage.Name);
                if (entity != null)
                {
                    entity.canvas_image_data.update(entity.originalImage, canvasDraggedImage);
                }
                canvasDraggedImage = null;
                draggedEntity = null;
            }
        }

        public void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (bIsDragging)
                HandleSelectBox(e);

            if (canvasDraggedImage != null)
            {
                var position = e.GetPosition(mainWindowCanvas);
                var offset = position - mousePosition;
                var upper_left_corner_limit = canvasDraggedImage.TranslatePoint(new Point(0, 0), mainWindowCanvas);
                var selectedEntities = entitiesController.GetSelectedEntities();

                mousePosition = position;
                
                foreach (Entity selectedEntity in selectedEntities)
                {
                    if (upper_left_corner_limit.X > 0)
                        selectedEntity.SetCanvasLeft(selectedEntity.originalImage, offset.X);
                    else
                        selectedEntity.SetCanvasLeft(selectedEntity.originalImage, (offset.X > 0 ? offset.X : 0));
                    if (upper_left_corner_limit.Y > 0)
                        selectedEntity.SetCanvasTop(selectedEntity.originalImage, offset.Y);
                    else
                        selectedEntity.SetCanvasTop(selectedEntity.originalImage, (offset.Y > 0 ? offset.Y : 0));
                    //UpdateCanvasSize(mainWindowCanvas, selectedEntity.originalImage);
                }
            }
        }

        public void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            ResetRect();
        }

        private void ResetRect()
        {
            bIsDragging = false;
            selectionBox.Visibility = Visibility.Collapsed;
        }

        private void HandleSelectBox(MouseEventArgs e)
        {
            double x = e.GetPosition(mainWindowCanvas).X;
            double y = e.GetPosition(mainWindowCanvas).Y;

            selectionBox.SetValue(Canvas.LeftProperty, Math.Min(x, selectBoxAnchor.X));
            selectionBox.SetValue(Canvas.TopProperty, Math.Min(y, selectBoxAnchor.Y));

            selectionBox.Width = Math.Abs(x - selectBoxAnchor.X);
            selectionBox.Height = Math.Abs(y - selectBoxAnchor.Y);

            if (selectionBox.Visibility != Visibility.Visible)
                selectionBox.Visibility = Visibility.Visible;
        }

        public void InitializeCanvasSizeIfNecessary(Canvas canvas)
        {
            if (Double.IsNaN(canvas.Width))
            {
                canvas.Width = canvas.ActualWidth;
            }
            if (Double.IsNaN(canvas.Height))
            {
                canvas.Height = canvas.ActualHeight;
            }
            if (canvas == mainWindowCanvas && (_canvas_default_size.height == 0 || _canvas_default_size.width == 0))
            {
                _canvas_default_size.width = canvas.Width;
                _canvas_default_size.height = canvas.Height;
            }
        }

        //TODO don't remind all images and resize depending of the current one 
        public void UpdateCanvasSize(Canvas canvas, Image image)
        {
            InitializeCanvasSizeIfNecessary(canvas);
            Dimensions dimensions = new Dimensions(canvas.Width, canvas.Height);
            Position image_end_position = new Position(Canvas.GetLeft(image) + image.Width, Canvas.GetTop(image) + image.Height);

            if (image_end_position.y > canvas.Height)
            {
                canvas.Height = image_end_position.y;
            }
            if (image_end_position.x > canvas.Width)
            {
                canvas.Width = image_end_position.x;
            }
        }

        public void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) && entitiesController.CountCanvasEntities() > 0)
                return;

            ScaleTransform scaleTr = mainWindowCanvas.LayoutTransform as ScaleTransform;

            if (e.Delta >= 5 && scaleTr.ScaleY < maxYScale && scaleTr.ScaleX < maxXScale)
            {
                scaleTr.ScaleY += zoomValue;
                scaleTr.ScaleX += zoomValue;
            }
            else if (e.Delta <= -5 && scaleTr.ScaleY > minYScale && scaleTr.ScaleX > minXScale)
            {
                scaleTr.ScaleY -= zoomValue;
                scaleTr.ScaleX -= zoomValue;
            }
        }

        public void RemoveSelectedImages()
        {
            draggedEntity = null;
            canvasDraggedImage = null;
        }

        public void SetUnselectedEntityZIndex(int zIndex)
        {
            foreach (Entity entity in entitiesController.GetCanvasEntities())
            {
                if (!entity.bSelected && Panel.GetZIndex(entity.originalImage) != zIndex)
                    Panel.SetZIndex(entity.originalImage, zIndex);
            }
        }
    }
}
