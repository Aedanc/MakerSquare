using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using FS = MakerSquare.FileSystem;

namespace ForwardLayoutTest.XAML
{
    /// <summary>
    /// Interaction logic for FileExplorer.xaml
    /// </summary>
    public partial class FileExplorer : UserControl
    {
        FS.Manager file_explorer;
        FS.VirtualDirectory current_dir;

        private Dictionary<string, Action<string, string>> _dict_to_AddFunc = new Dictionary<string, Action<string, string>>();
        public FileExplorer()
        {
            var user_docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MK2Project";
            FS.Manager.Initialize(user_docs);
            file_explorer = FS.Manager.Instance;
            InitializeComponent();
            OpenDir(file_explorer.Root);

            _dict_to_AddFunc.Add(file_explorer.SpriteDirName, new Action<string, string>(file_explorer.AddSprite));
            _dict_to_AddFunc.Add(file_explorer.SongDirName, new Action<string, string>(file_explorer.AddSong));
            _dict_to_AddFunc.Add(file_explorer.FontDirName, new Action<string, string>(file_explorer.AddFont));
        }

        public void OpenDir(FS.VirtualDirectory dir)
        {
            ClearExplorer();
            foreach (var children in dir.directories)
                Panel.Children.Add(new FileObj(children, this));
            foreach (var children in dir.files)
                Panel.Children.Add(new FileObj(children, this));
            if (dir.parent == null)            
                this.BackButton.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                this.BackButton.Visibility = System.Windows.Visibility.Visible;
                this.BackButton.Click -= OpenParent;
                this.BackButton.Click += OpenParent;
            }
            current_dir = dir;
        }

        private void OpenParent(object sender, EventArgs e)
        {                        
            OpenDir(current_dir.parent);
        }

        public void ClearExplorer()
        {
            Panel.Children.Clear();
        }

        public void AddFile(string real_path, string display_name, string filter_name)
        {
            _dict_to_AddFunc[filter_name](real_path, display_name);            
            OpenDir(current_dir);
        }

        private void Panel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;
            //Panel.ItemHeight += e.Delta * ScrollMultiplier;
            ////Clamping function
            //Panel.ItemHeight = (Panel.ItemHeight <= MinItemHeight) ? MinItemHeight : ((Panel.ItemHeight >= MaxItemHeight) ? MaxItemHeight : Panel.ItemHeight);
            //Panel.ItemWidth = Panel.ItemHeight * HWRatio;
        }

        private void Scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                Panel_MouseWheel(sender, e);
                e.Handled = true;
            }

        }

        private void OnClickAddFile(object sender, MouseButtonEventArgs e)
        {
            var dialog = new FileAddingDialog(this);
            dialog.Owner = MainWindow.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}
