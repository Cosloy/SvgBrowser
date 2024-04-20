using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SvgBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ObservableCollection<FileInfo> _svgFiles = new ObservableCollection<FileInfo>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo(@"c:\users\tcvan\desktop\icons");
            LoadFolder(folder);
        }

        public ObservableCollection<FileInfo> SvgFiles => _svgFiles;

        private void LoadFolder (DirectoryInfo folder)
        {
            _svgFiles.Clear();
            foreach (var file in folder.EnumerateFiles("*.svg"))
            {
                _svgFiles.Add(file);
            }
        }

        public Color IconBackground
        {
            get => Color.FromRgb(
                (byte)_backgroundBrightness, 
                (byte)_backgroundBrightness, 
                (byte)_backgroundBrightness);
        }

        private double _backgroundBrightness = 127;
        public double BackgroundBrightness
        {
            get => _backgroundBrightness;
            set
            {
                if (value == _backgroundBrightness)
                    return;
                _backgroundBrightness = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundBrightness)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconBackground)));
            }
        }

        private double _imageSize = 120;
        public double ImageSize
        {
            get => _imageSize;
            set
            {
                if (_imageSize == value)
                    return;
                _imageSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSize)));
            }
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Folder",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                DirectoryInfo folderInfo = new(folderName);
                LoadFolder(folderInfo);
            }
        }

        private void OnImageClick(object sender, RoutedEventArgs e)
        {
            FileInfo item = (FileInfo)((Button)sender).DataContext;
            Clipboard.SetText(item.FullName);
            MessageBox.Show(
                $"{item.FullName} has been copied to clipboard.", 
                "Filename Copied To Clipboard",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}