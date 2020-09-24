using Entities;
using ForwardLayoutTest.Classes;
using System.Windows;
using System.Windows.Controls;
using MakerSquare.FrontFacingECS;
using MakerSquare.FileSystem;
using ForwardLayoutTest.Controller;
using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace ForwardLayoutTest.XAML
{
    [Serializable]
    public class SpriteComponentSerializedData : ISerializedComponent
    {        
        public string image_path;
        public string image_name;

        public void SetComponentData(Entity entity)
        {
            var component = new SpriteComponentControl(entity);
            component.ChangeSpriteName(image_name);

            entity.AddComponent(component);
            
            var bitmap = new BitmapImage(new Uri(image_path));
            var originalImage = new Image { Source = bitmap };

            originalImage.Width = bitmap.PixelWidth;
            originalImage.Height = bitmap.PixelHeight; 
            entity.SetImage(image_path);
        }
    }

    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    ///     
    public partial class SpriteComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public string fileName;
        AssetSearcher assetSearcher;
        Window window;

        public SpriteComponentControl(Entity _selectedEntity)
        {
            InitializeComponent();
            selectedEntity = _selectedEntity;
            engineData = new List<FFComponent>();
        }

        public void SpriteFileButtonClick(object sender, RoutedEventArgs e)
        {
            Button obj = sender as Button;
            ChangeSpriteName(obj.Name);
            window.Close();
            selectedEntity.SetImage(assetSearcher.asset.RealFilePath);
        }

        public void ChangeSpriteName(string name)
        {
            fileName = name;
            TextFileName.Text = fileName;
            TextFileName.Visibility = Visibility.Visible;
        }

        private void SearchSpriteFileClick(object sender, RoutedEventArgs e)
        {
            assetSearcher = new AssetSearcher(EFileType.SPRITE, SpriteFileButtonClick);

            window = new Window
            {
                Title = "Select Asset",
                Content = assetSearcher,
                Height = 450,
                Width = 330,
            };
            window.ShowDialog();
        }

        public void FillData()
        {
            engineData.Add(new FFSpriteComponent(selectedEntity, fileName));
        }

        public void deleteComponent(object sender, RoutedEventArgs e) {
            
            selectedEntity.SetImage("pack://application:,,,/Src/letter-e124.png");

            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new SpriteComponentSerializedData();
            
            data.image_path = Manager.Instance.SearchForFileType(EFileType.SPRITE).Find(x => x.FileDisplayName == fileName).RealFilePath;
            data.image_name = fileName;
            entity._serializedComponents.Add(data);
        }
    }
}
