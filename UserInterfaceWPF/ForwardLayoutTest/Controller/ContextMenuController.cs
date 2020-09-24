using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.XAML;
using ForwardLayoutTest.XAML.Components;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ForwardLayoutTest.Controller
{
    class ContextMenuController
    {
        private static ContextMenuController instance = null;

        private Grid entityMenuGrid;
        private StackPanel menuPanel;
        private Button componentButton;
        private Entity selectedEntity;

        private StackPanel entityDataPanel = new StackPanel();

        // Instantiates singleton
        public ContextMenuController(Grid _entityMenuGrid)
        {
            if (instance == null)
            {
                entityMenuGrid = _entityMenuGrid;
                menuPanel = entityMenuGrid.Children.OfType<StackPanel>().First();

                componentButton = entityMenuGrid.Children.OfType<Button>().First();
                componentButton.Click += ComponentButtonClick;

                instance = this;
            }
        }

        public static ContextMenuController GetInstance()
        {
            return instance;
        }

        // Clears component menu when unselecting entity
        public void ClearMenu()
        {
            entityDataPanel.Children.Clear();
            menuPanel.Children.Remove(entityDataPanel);
            componentButton.Visibility = Visibility.Hidden;

            // clear all components
            for (int i = menuPanel.Children.Count - 1; i >= 0; --i)
            {
                if (menuPanel.Children[i] is UserControl)
                    menuPanel.Children.Remove(menuPanel.Children[i]);
            }

            foreach (var child in menuPanel.Children)
            {
                if (child is UserControl)
                    menuPanel.Children.Remove((UserControl)child);
            }

            selectedEntity = null;
        }

        // Instantiates component menu by adding all of the selected entity components
        public void SelectEntity(Entity entity)
        {
            selectedEntity = entity;
            TextBlock entityName = new TextBlock();

            entityName.FontSize = 20;
            entityName.Text = entity.Name;

            entityDataPanel.Children.Add(entityName);
            menuPanel.Children.Add(entityDataPanel);

            foreach (var component in selectedEntity.GetComponents())
            {
                menuPanel.Children.Add((UserControl)component);
            }

            componentButton.Visibility = Visibility.Visible;
        }

        // "Add component" button click event
        private void ComponentButtonClick(object sender, RoutedEventArgs e)
        {
            ComponentControl userControl = new ComponentControl();

            Window window = new Window
            {
                Title = "Select Component",
                Content = userControl,
                Height = 450,
                Width = 330,
                //SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();
        }

        // Creates a component, adds it to the context menu and to the entity component list
        public void CreateComponent(string componentName)
        {
            IComponent testControl = null;
                        
            if (componentName.Equals("SpriteComponent"))
                testControl = new SpriteComponentControl(selectedEntity);
            else if (componentName.Equals("CollisionComponent"))
                testControl = new CollisionComponentControl(selectedEntity);
            else if (componentName.Equals("MovementComponent"))
                testControl = new MovementComponentControl(selectedEntity);
            else if (componentName.Equals("RuleComponent"))
                testControl = new RuleComponentControl(selectedEntity);
            else if (componentName.Equals("AudioComponent"))
                testControl = new AudioComponentControl(selectedEntity);
            else if (componentName.Equals("InputComponent"))
                testControl = new InputComponentControl(selectedEntity);
            
            if (selectedEntity.GetComponents().Any(x => x.GetType().FullName == testControl.GetType().FullName))
                MessageBox.Show("Impossible d'avoir plusieurs fois le même composant au sein de la même entité.", "Composant en double");
            else if (testControl != null)
            {
                menuPanel.Children.Add((UserControl)testControl);
                selectedEntity.AddComponent(testControl);
            }
        }

        public void DeleteComponent(IComponent testControl)
        {
            if (testControl != null)
            {
                menuPanel.Children.Remove((UserControl)testControl);
                selectedEntity.RemoveComponent(testControl);
            }
        }
    }
}
