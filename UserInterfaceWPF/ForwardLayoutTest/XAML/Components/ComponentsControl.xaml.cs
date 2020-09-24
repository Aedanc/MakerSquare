using Entities;
using ForwardLayoutTest.Controller;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ForwardLayoutTest.XAML
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class ComponentControl : UserControl
    {
        public ComponentControl()
        {
            InitializeComponent();
        }

        private void ComponentButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Console.WriteLine("sender: " + button.Name);

            // detect component type and create component in view depending on the type
            ContextMenuController.GetInstance().CreateComponent(button.Name);

            Window.GetWindow(this).Close();
        }
    }
}
