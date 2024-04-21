using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SvgBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ObservableCollection<FileInfo> _svgFilesCollection;
        private readonly CollectionViewSource _collectionViewSource;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            DataContext = this;

            _currentFolder = LoadCurrentFolder() ?? Environment.CurrentDirectory; 

            _svgFilesCollection = new ObservableCollection<FileInfo>();
            _collectionViewSource = new CollectionViewSource();
            _collectionViewSource.Source = _svgFilesCollection;
            _collectionViewSource.SortDescriptions
                .Add(new SortDescription(
                    nameof(FileInfo.Name),
                    ListSortDirection.Ascending));

            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private string _currentFolder;
        public string CurrentFolder
        {
            get => _currentFolder;
            private set
            {
                if (_currentFolder == value)
                    return;
                _currentFolder = value;
                //Settings.Default.CurrentFolder = value;
                //Settings.Default.Save();
                SaveCurrentFolder(_currentFolder);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentFolder)));
            }
        }

        public ICollectionView SvgFiles => _collectionViewSource.View;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo folder = new(CurrentFolder);
            LoadFolder(folder);
        }

        private void LoadFolder(DirectoryInfo folder)
        {
            _svgFilesCollection.Clear();
            foreach (FileInfo file in folder.EnumerateFiles("*.svg"))
            {
                _svgFilesCollection.Add(file);
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
            OpenFolderDialog folderDialog = new()
            {
                Title = "Select Folder",
                InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFilesX86)
            };

            if (folderDialog.ShowDialog() == true)
            {
                string folderName = folderDialog.FolderName;
                CurrentFolder = folderName;
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

        private void SaveCurrentFolder (string folder)
        {
            Registry.CurrentUser
                .OpenSubKey("Software", true)
                ?.CreateSubKey("SvgBrowser")
                ?.SetValue("CurrentFolder", folder);
        }

        private string? LoadCurrentFolder ()
        {
            return Registry.CurrentUser
                .OpenSubKey("Software")
                ?.OpenSubKey("SvgBrowser")
                ?.GetValue("CurrentFolder") as string;
        }
    }
}