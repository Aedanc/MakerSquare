using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ForwardLayoutTest
{
    public partial class MainWindow : Window
    {

        CanvasController canvasController;

        Random _random = new Random();
        EntitiesController entitiesController = new EntitiesController();

        public MainWindow()
        {
            InitializeComponent();

            canvasController = new CanvasController(mainWindowCanvas, entitiesController, canvasScrollViewer);

            mainWindowCanvas.MouseLeftButtonUp += canvasController.CanvasMouseLeftButtonUp;
            mainWindowCanvas.MouseLeftButtonDown += canvasController.CanvasMouseLeftButtonDown;
            mainWindowCanvas.MouseMove += canvasController.CanvasMouseMove;
            mainWindowCanvas.MouseWheel += canvasController.CanvasMouseWheel;
        }


        private void ImportImage(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter =
                "Image Files (*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if ((bool)dialog.ShowDialog())
            {
                var new_entity = new ImageEntity(dialog.FileName);
                MainWindowStackPanelEntities.Children.Add(new_entity.original_image);
                new_entity.original_image.MouseMove += ImageMouseMove;
                new_entity.original_image.Name = "image_" + (entitiesController.CountCanvasEntities() + 1);
                new_entity.Name = dialog.SafeFileName;
                entitiesController.AddPrefabsEntity(new_entity.original_image.Name, new_entity);
            }
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();

                data.SetData(typeof(Image), sender);
                data.SetData(typeof(ImageSource), ((Image)sender).Source);
                data.SetData(typeof(object), this);
                data.SetData(typeof(DependencyObject), ((Image)sender).Parent);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }

        Regex hasAlreadyBeRenamed = new Regex("[(][0-9]+[)]$");

        private string ChangeNameIfNecesary(StringBuilder BuilderName, String name, int v)
        {
            foreach (Entity entity in dataListEntities)
            {
                if (entity.Name == name && hasAlreadyBeRenamed.IsMatch(name))
                {
                    BuilderName.Replace("(" + v + ")", "(" + (v + 1) + ")");
                    name = ChangeNameIfNecesary(BuilderName, BuilderName.ToString(), v + 1);
                }
                else if (entity.Name == name)
                {
                    return name + " (1)";
                }
            }
            return name;
        }

        private List<Entity> dataListEntities = new List<Entity>();

        private void AddEntityToList(Entity newEntity)
        {
            newEntity.Name = ChangeNameIfNecesary(new StringBuilder(newEntity.Name), newEntity.Name, 1);
            dataListEntities.Add(newEntity);
            listEntities.ItemsSource = null;
            listEntities.ItemsSource = dataListEntities;
        }

        Point previousMousePos = default(Point);
        Point actualMousePos = new Point();

        private void ScrollViewerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                Mouse.OverrideCursor = Cursors.ScrollAll;
        }

        private void ScrollViewerMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                previousMousePos = default(Point);
        }

        private void ScrollViewerMouseMove(object sender, MouseEventArgs e)
        {
            ScrollViewer sViewer = sender as ScrollViewer;
            double hOff = 0;
            double vOff = 0;

            if (sViewer == null)
                return;

            if (Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                actualMousePos = Mouse.GetPosition(sViewer);

                if (!previousMousePos.Equals(default(Point)))
                {
                    hOff = actualMousePos.X - previousMousePos.X;
                    vOff = actualMousePos.Y - previousMousePos.Y;

                    if (vOff < 5 && vOff > -5 && hOff < 5 && hOff > -5)
                    {
                        sViewer.ScrollToHorizontalOffset(sViewer.HorizontalOffset + hOff * -1);
                        sViewer.ScrollToVerticalOffset(sViewer.VerticalOffset + vOff * -1);
                    }
                }

                previousMousePos = actualMousePos;
            }
        }

        private void CanvasDrop(object sender, DragEventArgs e)
        {
            if (e.Handled == false)
            {
                Canvas canvas = (Canvas)sender;
                UIElement parent = (UIElement)e.Data.GetData(typeof(DependencyObject));
                ImageSource imgsource = e.Data.GetData(typeof(ImageSource)) as ImageSource;
                Point mousePos = Mouse.GetPosition(null);

                if (canvas != null)
                {
                    // Get the panel that the element currently belongs to,
                    // then remove it from that panel and add it the Children of
                    // the panel that its been dropped on.

                    if (parent != null && imgsource != null)
                    {
                        if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                        {
                            Image img = new Image();
                            var position = e.GetPosition(canvas);
                            var offset = position - mousePos;
                            ImageEntity associated_entity = entitiesController.FindImageEntityByOriginalImageSourceHashCode(imgsource.GetHashCode());
                            img.Source = imgsource;
                            ImageEntity canvasImage = new ImageEntity(associated_entity)
                            {
                                original_image = img,
                                entity_id = entitiesController.generator.NewID()
                            };
                            canvasImage.original_image.Name = "_" + canvasImage.entity_id.ToString();
                            img.Source = imgsource;

                            associated_entity.canvas_image_data.position.x = offset.X - imgsource.Width / 2;
                            associated_entity.canvas_image_data.position.y = offset.Y - imgsource.Height / 2;

                            associated_entity.canvas_image_data.source_hashcode = img.Source.GetHashCode();
                            associated_entity.canvas_image_data.image_id = canvasImage.original_image.Name;

                            canvasImage.canvas_image_data.position.x = offset.X - imgsource.Width / 2;
                            canvasImage.canvas_image_data.position.y = offset.Y - imgsource.Height / 2;

                            canvasImage.canvas_image_data.source_hashcode = img.Source.GetHashCode();
                            canvasImage.canvas_image_data.image_id = canvasImage.original_image.Name;

                            canvasImage.Name = "Image " + entitiesController.CountCanvasEntities();

                            entitiesController.AddCanvasEntity("Image " + entitiesController.CountCanvasEntities(), canvasImage);

                            Canvas.SetLeft(img, offset.X - imgsource.Width / 2);
                            Canvas.SetTop(img, offset.Y - imgsource.Height / 2);
                            ((Canvas)sender).Children.Add(img);

                            img.Width = imgsource.Width;
                            img.Height = imgsource.Height;

                            canvasController.UpdateCanvasSize(mainWindowCanvas, img);

                            // set the value to return to the DoDragDrop call
                            e.Effects = DragDropEffects.Copy;
                            AddEntityToList(canvasImage);
                        }
                    }
                }
            }
        }

        private void WindowMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
    }
}