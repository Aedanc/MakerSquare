using FS = MakerSquare.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Entities;

namespace ForwardLayoutTest.XAML
{

    /// <summary>
    /// Interaction logic for FileObj.xaml
    /// </summary>
    public partial class FileObj : UserControl
    {

        #region Properties

        public string Filetype
        {
            get { return (string)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FILETYPE.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileTypeProperty =
            DependencyProperty.Register("Filetype", typeof(string), typeof(FileObj), new PropertyMetadata("Type de fichier inconnu"));


        public string Displayname
        {
            get { return (string)GetValue(DisplaynameProperty); }
            set { SetValue(DisplaynameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Displayname.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplaynameProperty =
            DependencyProperty.Register("Displayname", typeof(string), typeof(FileObj), new PropertyMetadata("File name that is way too long and is due to pose problems"));

        public string Filename
        {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string), typeof(FileObj), new PropertyMetadata("Fichier inconnu"));

        public bool InSelection
        {
            get { return (bool)GetValue(InSelectionProperty); }
            set
            {
                SetValue(InSelectionProperty, value);
                if (value == true)
                    Panel.Background = Blue;
                else
                    Panel.Background = Brushes.Transparent;

            }
        }

        // Using a DependencyProperty as the backing store for InSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InSelectionProperty =
            DependencyProperty.Register("InSelection", typeof(bool), typeof(FileObj), new PropertyMetadata(false));

        public static bool GetInSelection(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (bool)element.GetValue(InSelectionProperty);
        }
        public static void SetInSelection(UIElement element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(InSelectionProperty, value);
        }

        #endregion

        private Brush Blue;
        private FS.VirtualFile file;
        private FS.VirtualDirectory dir = null;
        private FileExplorer explorer;
        public bool isClicked = false;
        public Dictionary<string, string> MimeTo = new Dictionary<string, string>
        {
            { "audio", "MusicFileImage" },
            { "application/x-font", "FontFileImage"},
            { "image", "PictureFileImage" }
        };

        private void LoadInImage(string image_path)
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(image_path);
            myBitmapImage.EndInit();
            this.Image.Source = ((Image)this.FindResource("BackgroundFileImage")).Source;
            this.Image.Source = myBitmapImage;            
        }

        protected override void OnInitialized(EventArgs e)
        {
            InitializeComponent();
            base.OnInitialized(e);

            if (dir != null)
                InitDirectory();
            else
                InitFile();
        }

        private void InitDirectory()
        {
            this.Image.Source = ((Image)this.FindResource("FolderImage")).Source;
            this.MouseDoubleClick += DirectoryClick;
            this.FilenameTextBlock.Width = 160;
        }

        private void DirectoryClick(object sender, MouseButtonEventArgs e)
        {
            explorer.OpenDir(dir);
        }

        private void InitFile()
        {
            this.FilenameTextBlock.Width = 180;
            var Mime = new MimeSharp.Mime();
            try
            {
                if (!File.Exists(file.RealFilePath))
                    throw new Exception("Non-existant file");
                Filetype = Mime.Lookup(file.RealFilePath);
                foreach (KeyValuePair<string, string> pair in MimeTo)
                    if (Filetype.StartsWith(pair.Key))
                    {
                        this.Image.Source = ((Image)this.FindResource(pair.Value)).Source;
                        if (pair.Key == "image")
                            Panel.MouseMove += ImageMouseMove;                        
                        return;
                    }
            }
            catch (Exception)
            {
                this.Image.Source = ((Image)this.FindResource("UnknownFileImage")).Source;
            }
        }

        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData(typeof(FS.VirtualFile), file);
                data.SetData(typeof(Image), sender);
                data.SetData(typeof(ImageSource), new Image { Source = new BitmapImage(new Uri(file.RealFilePath)) }.Source);
                data.SetData(typeof(object), this);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }


        private void SetupBrush()
        {
            BrushConverter bc = new BrushConverter();
            Blue = (Brush)bc.ConvertFrom("#7faef9");
        }

        public FileObj()
        {
            SetupBrush();
        }

        public FileObj(FS.VirtualDirectory dir, FileExplorer explorer)
        {
            SetupBrush();
            this.dir = dir;
            this.explorer = explorer;
            this.Displayname = dir.name;
        }

        public FileObj(FS.VirtualFile file, FileExplorer explorer)
        {
            SetupBrush();
            this.file = file;
            this.explorer = explorer;
            this.Displayname = file.FileDisplayName;
        }

        private void Panel_MouseDown(object sender, MouseButtonEventArgs e)
        {
        //    if (e.ClickCount == 1)
        //        InSelection = !InSelection;
        //    if (e.ClickCount >= 2)
        //    {
        //        //try
        //        //{ System.Diagnostics.Process.Start(Filename); }
        //        //catch (Exception)
        //        //{
        //        //    MessageBoxResult result = MessageBox.Show("Ce fichier n'existe pas, et sera supprimé de votre liste de fichiers.",
        //        //                              "Fichier inexistant",
        //        //                              MessageBoxButton.OK,
        //        //                              MessageBoxImage.Error);
        //        //    //TODO remove non-existing file
        //        //}
        //    }
        }
    }
}
