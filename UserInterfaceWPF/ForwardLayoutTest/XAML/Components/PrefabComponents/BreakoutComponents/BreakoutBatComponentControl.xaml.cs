using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.Controller;
using MakerSquare.FileSystem;
using MakerSquare.FrontFacingECS;
using MakerSquare.FrontFacingECS.PrefabEntities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForwardLayoutTest.XAML.Components
{

    [Serializable]
    public class BreakoutBatSerializedData : ISerializedComponent
    {
        public string bat_sprite;        

        public void SetComponentData(Entity entity)
        {
            var breakout = new BreakoutBatComponentControl(entity);
            entity.AddComponent(breakout);
        }
    }

    public partial class BreakoutBatComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public string spriteName;
        AssetSearcher assetSearcher;
        Window window;

        public BreakoutBatComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
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
            spriteName = name;
            TextFileName.Text = spriteName;
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

        public void AddSerializedComponent(Entity entity)
        {
            var data = new BreakoutBatSerializedData();
            data.bat_sprite = spriteName;
            entity._serializedComponents.Add(data);
        }

        public void FillData()
        {
            var data = new FFBreakoutBatComponent(selectedEntity);
            data.spritename = spriteName;
            engineData.Add(data);
        }

        public void DeleteComponent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir supprimer les composants et entités en rapport avec le préfabriqué de Casse-Briques ?", "Suppression", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                PrefabController.Instance.DeletePrefab(PrefabEnum.BREAKOUT);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
