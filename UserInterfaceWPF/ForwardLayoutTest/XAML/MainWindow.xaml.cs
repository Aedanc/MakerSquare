using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Entities;
using FS = MakerSquare.FileSystem;
using ForwardLayoutTest.Controller;
using ForwardLayoutTest.XAML;
using MakerSquare.Compiler;
using MakerSquare.FileSystem;
using Codegen;
using System.Diagnostics;
using System.IO;
using MakerSquare.FrontFacingECS;
using System.Windows.Media.Imaging;

namespace ForwardLayoutTest
{
    public partial class MainWindow : Window
    {
        CanvasController canvasController;
        ContextMenuController contextMenuController;
        PrefabController prefabController;

        Random _random = new Random();
        public EntitiesController entitiesController { get; }
        SceneController sceneController;

        public MainWindow()
        {
            InitializeComponent();

            entitiesController = new EntitiesController();
            contextMenuController = new ContextMenuController(entityMenuGrid);
            canvasController = new CanvasController(mainWindowCanvas, entitiesController, canvasScrollViewer);
            sceneController = new SceneController(this);
            PrefabController.Init(this);
            prefabController = PrefabController.Instance;

            this.PreviewKeyDown += new KeyEventHandler(MainWindowKeyDownHandler);
            this.PreviewKeyDown += new KeyEventHandler(MainWindowDebugKey);
            this.PreviewKeyUp += new KeyEventHandler(MainWindowKeyUpHandler);

            mainWindowCanvas.MouseLeftButtonUp += canvasController.CanvasMouseLeftButtonUp;
            mainWindowCanvas.MouseLeftButtonDown += canvasController.CanvasMouseLeftButtonDown;
            mainWindowCanvas.MouseMove += canvasController.CanvasMouseMove;
            mainWindowCanvas.MouseWheel += canvasController.CanvasMouseWheel;
            mainWindowCanvas.MouseLeave += canvasController.CanvasMouseLeave;

            Search = new SearchBar { SearchText = "", DataListEntities = dataListEntities, mainWindow = this };
            this.DataContext = Search;
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

        public List<Entity> dataListEntities = new List<Entity>();

        public void AddEntityToList(Entity newEntity)
        {
            newEntity.Name = ChangeNameIfNecesary(new StringBuilder(newEntity.Name), newEntity.Name, 1);
            dataListEntities.Add(newEntity);
            listEntities.ItemsSource = null;
            listEntities.ItemsSource = dataListEntities;
        }

        public void RemoveEntitiesFromList(List<Entity> entities)
        {
            dataListEntities.RemoveAll(x => entities.Contains(x));
            listEntities.ItemsSource = null;
            listEntities.ItemsSource = dataListEntities;
        }

        public void EntitiesRefresh()
        {
            listEntities.ItemsSource = null;
            listEntities.ItemsSource = Search.DisplayDataListEntities;
        }

        Point previousMousePos = default(Point);
        Point actualMousePos = new Point();
        private SearchBar Search;

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
                ImageSource imgsource = e.Data.GetData(typeof(ImageSource)) as ImageSource;
                BitmapSource bitmapSource = (BitmapSource)imgsource;

                Point mousePos = Mouse.GetPosition(null);
                FS.VirtualFile file = e.Data.GetData(typeof(FS.VirtualFile)) as FS.VirtualFile;

                if (canvas != null)
                {                
                    if (imgsource != null)
                    {
                        if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                        {
                            Image img = new Image();
                            var position = e.GetPosition(canvas);
                            var offset = position - mousePos;
                            img.Source = imgsource;

                            img.Width = bitmapSource.PixelWidth;
                            img.Height = bitmapSource.PixelHeight;

                            Entity canvasImage = new Entity(imgsource.ToString(), "Image" + entitiesController.CountCanvasEntities())
                            {
                                originalImage = img,
                                entity_id = entitiesController.generator.NewID()
                            };
                            canvasImage.originalImage.Name = "_" + canvasImage.entity_id.ToString();
                            img.Source = imgsource;

                            canvasImage.canvas_image_data.position.x = offset.X - imgsource.Width / 2;
                            canvasImage.canvas_image_data.position.y = offset.Y - imgsource.Height / 2;

                            canvasImage.canvas_image_data.source_hashcode = img.Source.GetHashCode();
                            canvasImage.canvas_image_data.image_id = canvasImage.originalImage.Name;

                            canvasImage.Name = "Image" + canvasImage.entity_id.ToString();

                            SpriteComponentControl spriteComponent = new SpriteComponentControl(canvasImage);
                            spriteComponent.ChangeSpriteName(file.FileDisplayName);
                            canvasImage.AddComponent(spriteComponent);

                            entitiesController.AddCanvasEntity(canvasImage.Name, canvasImage);

                            Canvas.SetLeft(img, offset.X - imgsource.Width / 2);
                            Canvas.SetTop(img, offset.Y - imgsource.Height / 2);
                            canvasImage.Transform.x = (int)(offset.X - imgsource.Width / 2);
                            canvasImage.Transform.y = (int)(offset.Y - imgsource.Height / 2);

                            ((Canvas)sender).Children.Add(img);

                            canvasController.InitializeCanvasSizeIfNecessary(canvasController.GetCanvas());

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

        private void MainWindowKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                entitiesController.UnselectAll();
            else if ((e.Key == Key.Delete/* || e.Key == Key.Back*/) && FocusOnSearchBar == false)
            {
                bool mouseIsDown = Mouse.LeftButton == MouseButtonState.Pressed;

                if (!mouseIsDown && entitiesController.GetSelectedEntities() != null)
                {
                    ContextMenuController.GetInstance().ClearMenu();
                    RemoveEntitiesFromList(entitiesController.GetSelectedEntities());
                    entitiesController.DeleteSelectedEntities();
                }
            }
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                canvasController.bMultiSelect = true;
        }

        private void MainWindowKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                canvasController.bMultiSelect = false;
        }

        private void MainWindowDebugKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                entitiesController.SaveEntitiesData();
                entitiesController.EntitiesComponentsLog();
            }
        }

        private void ListEntities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox obj = sender as ListBox;
            if (obj.SelectedIndex != -1)
            {
                entitiesController.UnselectAll();
                contextMenuController.ClearMenu();
                dataListEntities[obj.SelectedIndex].Select();
                contextMenuController.SelectEntity(dataListEntities[obj.SelectedIndex]);
            }
        }

        private void FileExplorer_Loaded(object sender, RoutedEventArgs e)
        {
		}
		
        bool FocusOnSearchBar = false;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusOnSearchBar = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusOnSearchBar = false;
        }

        private void MenuItem_Ouvrir(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Ouvrir une scène MakerSquare";
            dialog.Filter = "Scène Makers² (*.mk2scene)|*.mk2scene;";
            dialog.InitialDirectory = Path.Combine(FS.Manager.Instance.ProjectDir, FS.Manager.Instance.EditorScenesDirName);            
            if (dialog.ShowDialog() == true)
            {
                if (!dialog.FileName.StartsWith(dialog.InitialDirectory))
                {
                    MessageBox.Show("Merci de ne pas sortir des dossiers prévus à cet effet !", "Mauvais dossier",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else
                {
                    sceneController.LoadScene(dialog.FileName);
                }
            }                            
        }

        private void MenuItem_Sauvegarder(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Sauvegarder une scène MakerSquare";
            dialog.Filter = "Scène Makers² (*.mk2scene)|*.mk2scene;";
            dialog.InitialDirectory = Path.Combine(FS.Manager.Instance.ProjectDir, FS.Manager.Instance.EditorScenesDirName);
            if (!Directory.Exists(dialog.InitialDirectory))
                Directory.CreateDirectory(dialog.InitialDirectory);
            if (dialog.ShowDialog() == true)
            {
                if (!dialog.FileName.StartsWith(dialog.InitialDirectory))
                {
                    MessageBox.Show("Merci de sauvegarder votre scène dans le dossier \"EditorScenes\" !", "Mauvais dossier",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else
                {
                    sceneController.SaveSceneAs(dialog.FileName);
                }
            }
        }

        private void MenuItem_Quitter(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Compiler(object sender, MouseButtonEventArgs e)
        {
            var dialog = new BuildWindow(this);
            dialog.Owner = GetWindow(this);
            dialog.ShowDialog();
        }

        private void MenuItem_BreakoutPrefab(object sender, MouseButtonEventArgs e)
        {
            if (!BreakoutMenuEntry.IsChecked)            
                prefabController.CreatePrefab(PrefabEnum.BREAKOUT);                
            else
                MessageBox.Show("Tu ne peux pas avoir de préfabriqués de casse-briques en double.\n" +
                    "Si tu souhaites supprimer ton préfabriqué de casse-briques, supprime simplement un des composants de casse-briques, les autres se supprimeront d'eux-même.", "Duplication impossible !");
        }
    }
}