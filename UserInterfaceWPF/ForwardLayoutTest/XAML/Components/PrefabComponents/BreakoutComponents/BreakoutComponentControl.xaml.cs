using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.Controller;
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
    public class BreakoutComponentSerializedData : ISerializedComponent
    {
        public uint lives;            

        public void SetComponentData(Entity entity)
        {
            var breakout = new BreakoutComponentControl(entity);            
            breakout.LivesTextBox.Text = lives.ToString();
            entity.AddComponent(breakout);            
        }
    }

    public partial class BreakoutComponentControl : UserControl, IComponent
    {

        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public BreakoutComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new BreakoutComponentSerializedData();
            data.lives = UInt32.Parse(LivesTextBox.Text);            
            entity._serializedComponents.Add(data);
        }

        public void FillData()
        {
            var data = new FFBreakoutManagerComponent(selectedEntity);
            data.lives = UInt32.Parse(LivesTextBox.Text);            
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
