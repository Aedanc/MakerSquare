using ForwardLayoutTest;
using ForwardLayoutTest.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MakerSquare.FrontFacingECS;
using System.Runtime.Serialization;

namespace Entities
{
    public struct Position
    {
        public Position(double x_, double y_)
        {
            x = x_;
            y = y_;
            depth = 0;
        }

        public Position(double x_, double y_, uint depth_)
        {
            x = x_;
            y = y_;
            depth = depth_;
        }

        public double x;
        public double y;
        public uint depth;
    };

    [Serializable]
    public class Entity : FFEntity
    {        
        [NonSerialized]
        protected List<IComponent> interfaceComponents = new List<IComponent>();
        public List<ISerializedComponent> _serializedComponents = new List<ISerializedComponent>();
        //protected List<Entity> Children = new List<Entity>();
        //protected Position Transform;        

        //public string Name { get; set; }
        public Guid Guid { get; }

        [NonSerialized]
        public Image originalImage;
        [NonSerialized]
        public BitmapImage bitmap;
        public Scale scale;
        public float rotation;
        public bool bSelected = false;
        [NonSerialized]
        public Border outlineBorder = null;
        [NonSerialized]
        public CanvasImage canvas_image_data = new CanvasImage();

        public int entity_id = -1;

        /////   CTORS

        public Entity(string _name) : base(_name)
        {
            Guid = new Guid();
            //Transform = new Position(0, 0, 0);
        }

        public Entity(Position pos, string _name) : base(_name)
        {
            Guid = new Guid();
            Transform = pos;
        }

        public Entity(string imagePath, string _name) : base(_name)
        {
            Guid = new Guid();
            //Transform = new Position(0, 0, 0);

            bitmap = new BitmapImage(new Uri(imagePath));
            originalImage = new Image { Source = bitmap };
            Canvas.SetLeft(originalImage, 0);
            Canvas.SetTop(originalImage, 0);
        }

        public Entity(Entity _img, string _name) : base(_name)
        {
            if (_img != null)
            {
                Guid = _img.Guid;
                Transform = _img.Transform;

                originalImage = _img.originalImage;
                bitmap = _img.bitmap;
            }
        }

        /////   IMAGE METHODS 

        public void SetImage(string imagePath)
        {
            double x = Canvas.GetLeft(originalImage);
            double y = Canvas.GetTop(originalImage);
            string imgName = originalImage.Name;

            CanvasController.GetInstance().GetCanvas().Children.Remove(originalImage);
            RemoveOutlineBorder();

            bitmap = new BitmapImage(new Uri(imagePath));
            originalImage = new Image { Source = bitmap };
            originalImage.Name = imgName;
            originalImage.Width = bitmap.PixelWidth;
            originalImage.Height = bitmap.PixelHeight;

            Canvas.SetLeft(originalImage, x);
            Canvas.SetTop(originalImage, y);

            CanvasController.GetInstance().GetCanvas().Children.Add(originalImage);
            //CreateOutlineBorder();
        }

        public bool Selected()
        {
            return bSelected;
        }

        public void Select()
        {
            if (!bSelected)
            {
                bSelected = true;

                CreateOutlineBorder();
                Panel.SetZIndex(originalImage, 1);
                Panel.SetZIndex(outlineBorder, 1);

                CanvasController.instance.SetUnselectedEntityZIndex(0);
            }
        }

        public void CreateOutlineBorder()
        {
            outlineBorder = new Border();

            outlineBorder.BorderThickness = new Thickness(5);
            outlineBorder.BorderBrush = new SolidColorBrush(Colors.Orange);
            outlineBorder.Height = originalImage.Height;
            outlineBorder.Width = originalImage.Width;

            CanvasController.GetInstance().GetCanvas().Children.Add(outlineBorder);
            Canvas.SetLeft(outlineBorder, Transform.x);
            Canvas.SetTop(outlineBorder, Transform.y);
        }

        public void RemoveOutlineBorder()
        {
            CanvasController.GetInstance().GetCanvas().Children.Remove(outlineBorder);
        }

        public void Unselect()
        {
            bSelected = false;

            RemoveOutlineBorder();
            outlineBorder = null;
        }

        public void SetCanvasLeft(Image draggedImage, double xValue)
        {
            double left = Canvas.GetLeft(draggedImage) + xValue;
            Canvas canvas = CanvasController.instance.GetCanvas();

            if (left + draggedImage.ActualWidth > canvas.ActualWidth)
                left = canvas.ActualWidth - draggedImage.ActualWidth;

            Canvas.SetLeft(draggedImage, left);
            Transform.x = (int)left;

            if (bSelected)
                Canvas.SetLeft(outlineBorder, left);
        }

        public void SetCanvasTop(Image draggedImage, double yValue)
        {
            double top = Canvas.GetTop(draggedImage) + yValue;
            Canvas canvas = CanvasController.instance.GetCanvas();

            if (top + draggedImage.ActualHeight > canvas.ActualHeight)
                top = canvas.ActualHeight - draggedImage.ActualHeight;

            Canvas.SetTop(draggedImage, top);
            Transform.y = (int)top;

            if (bSelected)
                Canvas.SetTop(outlineBorder, top);
        }

        public void DeleteFromCanvas()
        {
            Unselect();
            CanvasController.GetInstance().GetCanvas().Children.Remove(originalImage);
        }

        public Point GetPosition()
        {
            return new Point(Canvas.GetLeft(originalImage), Canvas.GetTop(originalImage));
        }

        /////   COMPONENT METHODS

        public List<IComponent> GetComponents()
        {
            return interfaceComponents;
        }

        public void AddComponent(IComponent component)
        {
            interfaceComponents.Add(component);
        }

        public void ClearComponents()
        {
            Components.Clear();
        }

        public void RemoveComponent(IComponent component)
        {
            interfaceComponents.Remove(component);
        }

        [OnSerializing]
        private void SerializeData(StreamingContext context)
        {            
            foreach (var component in interfaceComponents)            
                component.AddSerializedComponent(this);            
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            _serializedComponents.Clear();
        }
        
        public void DeserializeData()
        {
            interfaceComponents = new List<IComponent>();
            canvas_image_data = new CanvasImage();
            outlineBorder = null;

            var bitmap = new BitmapImage(new Uri("pack://application:,,,/Src/letter-e124.png"));
            var entityImage = new Image { Source = bitmap };

            entityImage.Width = bitmap.PixelWidth;
            entityImage.Height = bitmap.PixelHeight;
            originalImage = entityImage;
            SetImage("pack://application:,,,/Src/letter-e124.png");

            foreach (var component in _serializedComponents)
                component.SetComponentData(this);
            _serializedComponents.Clear();
        }
    }
}