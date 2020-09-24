using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.Controller;
using MakerSquare.FrontFacingECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForwardLayoutTest.XAML
{
    [Serializable]
    public class CollisionComponentSerializedData : ISerializedComponent
    {
        public int x, y;

        public void SetComponentData(Entity entity)
        {
            var collision = new CollisionComponentControl(entity);
            collision.SetXY(x, y);
            entity.AddComponent(collision);
        }
    }

    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class CollisionComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public int x { get; private set; }
        public int y { get; private set; }
        private bool bTrigger;

        public CollisionComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void SetXY(int x_, int y_)
        {
            x = x_;
            y = y_;
            VectorXTextBox.Text = x.ToString();
            VectorYTextBox.Text = y.ToString();
        }

        public void FillData()
        {
            x = Int32.Parse(VectorXTextBox.Text);
            y = Int32.Parse(VectorYTextBox.Text);
            //bTrigger = TriggerCheckBox.IsChecked.Value;

            engineData.Add(new FFCollision(selectedEntity, new MakerSquare.FrontFacingECS.Tuple<int, int>(x, y)/*, bTrigger*/));
        }
        public void deleteComponent(object sender, RoutedEventArgs e)
        {
            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new CollisionComponentSerializedData();            
            data.x = Int32.Parse(VectorXTextBox.Text);
            data.y = Int32.Parse(VectorYTextBox.Text);
            entity._serializedComponents.Add(data);
        }
    }
}
