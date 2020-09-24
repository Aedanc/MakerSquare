using MakerSquare.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Logique d'interaction pour AssetSearcher.xaml
    /// </summary>
    public partial class AssetSearcher : UserControl
    {
        EFileType type;
        List<VirtualFile> files;
        public VirtualFile asset = null;

        public AssetSearcher(EFileType type, RoutedEventHandler handler)
        {
            InitializeComponent();

            this.type = type;
            files = Manager.Instance.SearchForFileType(type);
            AddFiles(handler);
        }

        public void AddFiles(RoutedEventHandler handler)
        {
            foreach (var file in files)
            {
                Button fileButton = new Button();

                fileButton.Name = file.FileDisplayName;
                fileButton.Content = file.FileDisplayName;
                fileButton.Click += FileButtonClick;
                fileButton.Click += handler;

                AssetsPanel.Children.Add(fileButton);
            }
        }

        public void FileButtonClick(object sender, RoutedEventArgs e)
        {
            Button obj = sender as Button;

            foreach (var file in files)
            {
                if (file.FileDisplayName.Equals(obj.Name))
                {
                    asset = file;
                    break;
                }
            }
        }
    }
}
