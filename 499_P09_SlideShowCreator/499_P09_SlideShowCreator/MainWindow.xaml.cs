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
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Runtime.CompilerServices;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Windows.Threading;
using System.Runtime.Remoting.Messaging;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace _499_P09_SlideShowCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // The current window
        public static MainWindow current;

        // Main window constructor
        public MainWindow()
        {
            current = this;
            DataContext = this;

            closeButton = false;
            presentButton = false;
            saveButton = false;
            openButton = true;
            newButton = true;

            this.Title = "";

            directoryPath = "";

            Init();
            InitializeComponent();

            //AutoTime = AutoSeconds.SelectedItem.ToString().Substring(38);
            Auto_Manual = AutoVSManual.SelectedValue.ToString().Substring(38);
        }

        // Path to the save directory
        private string SaveDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Slideshow Save File");

        // Booleans for when buttons are able to be clicked
        public bool presentButton;
        public bool newButton;
        public bool saveButton;
        public bool closeButton;
        public bool openButton;

        // Variable to hold a slideshow object
        private SlideShow slideShow = new SlideShow();
        public SlideShow Slide_Show
        {
            get { return slideShow; }
            set { slideShow = value; }
        }

        // Variable for the current index of the slideshow
        private int currentIndex;
        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (value < 0) return;
                if (value >= track.Count) return;
                if (currentIndex == value) return;
                currentIndex = value;
                CurrentSlide = track[currentIndex].ImagePath;
                OnPropertyChanged();
            }
        }

        // An empty spave image
        private string emptyImage = "EmptySpace.png";

        // Function called to refresh the index of the slideshow
        private void RefreshIndex()
        {
            CurrentSlide = (Track.Count == 0) ? emptyImage : Track[CurrentIndex].ImagePath;
        }

        // Track object to hold the current track
        private ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();
        public ObservableCollection<TrackItem> Track
        {
            get => track;
            set
            {
                if (track == value) return;
                track = value;
                OnPropertyChanged();
            }
        }

        // Variable to hold the directory of images to choose from
        private ObservableCollection<ImageFile> directoryImages = new ObservableCollection<ImageFile>();
        public ObservableCollection<ImageFile> DirectoryImages
        {
            get => directoryImages;
            set
            {
                if (directoryImages == value) return;
                directoryImages = value;
                OnPropertyChanged();
            }
        }

        // Variable to hold the audio collection
        private ObservableCollection<string> audioCollection = new ObservableCollection<string>();
        public ObservableCollection<string> AudioCollection
        {
            get => audioCollection;
            set
            {
                if (audioCollection == value) return;
                audioCollection = value;
                OnPropertyChanged();
            }
        }

        // Variable for the current slide the slideshow is showing
        private string currentSlide = "EmptySpace.png";
        public string CurrentSlide
        {
            get => currentSlide;
            set
            {
                if (currentSlide == value) return;
                currentSlide = value;
                OnPropertyChanged();
            }
        }

        // Variable for the media player that controls the sounds
        private MediaPlayer mediaPlayer = new MediaPlayer();

        // Variable for the current audio playing
        private string currentAudioName;
        public string CurrentAudioName
        {
            get => currentAudioName;
            set
            {
                if (currentAudioName == value) return;
                currentAudioName = value;
                OnPropertyChanged();
            }
        }

        // Variable for the current audios time
        private string currentAudioTime;
        public string CurrentAudioTime
        {
            get => currentAudioName;
            set
            {
                if (currentAudioTime == value) return;
                currentAudioTime = value;
                OnPropertyChanged();
            }
        }

        // Variable for the current audios index
        private int currentAudioIndex = 0;

        // Initialization function used to import the directory of images to choose from
        private void Init()
        {
            ImportDirectory();
        }

        // Function to control the audio timer
        private void AudioTimerTick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source == null) return;
            if (mediaPlayer.NaturalDuration.HasTimeSpan == false) return;
            CurrentAudioTime = "( " + (mediaPlayer.Position.ToString(@"mm\:ss")) + " / " + (mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss")) + " )";
        }

        // Function to open/play the audio
        private void OnAudioOpen(object sender, EventArgs e)
        {
            CurrentAudioName = mediaPlayer.Source.Segments.Last().ToString();
            mediaPlayer.Play();
        }

        // Function to close the audio
        private void OnAudioEnd(object sender, EventArgs e)
        {
            if (audioCollection.Count > currentAudioIndex + 1)
            {
                currentAudioIndex++;
                mediaPlayer.Open(new Uri(audioCollection[currentAudioIndex]));
            }
        }

        // Variable to notify when a property has changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Fucntion to go to the previous slide
        private void PreviousSlide(object sender, RoutedEventArgs e)
        {
            CurrentIndex--;
        }

        // Function to go to the next slide
        private void NextSlide(object sender, RoutedEventArgs e)
        {
            CurrentIndex++;
        }

        // Function to add a slide
        private void AddSlide(object sender, RoutedEventArgs e)
        {
            if (Track.Count > 0) Track.Add(new Transition(Transition.TransitionTypes.SlideUp));
            Track.Add(new Slide(GetIndex(sender, directoryImages)));
            if (Track.Count == 1) RefreshIndex();
        }

        // Function to get the index
        public int GetIndex<T>(object sender, ObservableCollection<T> col) where T : class
        {
            Button button = sender as Button;
            T item = button.DataContext as T;
            return (col.IndexOf(item));
        }

        // Function to select a track
        private void TrackSelect(object sender, RoutedEventArgs e)
        {
            int index = GetIndex(sender, Track);
            if (index % 2 == 0) SlideSelect(index);
            else TransitionSelect(index);
        }

        // Function to select a slide
        private void SlideSelect(int index)
        {
            CurrentIndex = index;
            SlideOptions options = new SlideOptions(Track[CurrentIndex]);
            options.Show();
        }

        // Removed due to removal of DeleteSlidesCheckBox
        /*
        public void CheckDeleteSlidesCheckBox(bool is_checked)
        {
            if (is_checked) DeleteSlidesCheckBox.IsChecked = true;
            else DeleteSlidesCheckBox.IsChecked = false;
        }
        */

        internal void PreviousSlideForTest(object sender, RoutedEventArgs e) //test method
        {
            PreviousSlide(sender, e);
        }

        internal void NextSlideForTest(object sender, RoutedEventArgs e) //test method
        {
            NextSlide(sender, e);
        }

        internal void AddSlideForTest(object sender, RoutedEventArgs e)
        {
            AddSlide(sender, e);
        }

        // Function to select a transition
        private void TransitionSelect(int index)
        {
            Transition transition = Track[index] as Transition;
            //if (DeleteSlidesCheckBox.IsChecked == true) transition.TransitionType = Transition.TransitionTypes.None;
            //else transition.TransitionType++;
            transition.TransitionType++;
        }

        // Function to delete a slide
        public void DeleteSlide(int index)
        {
            int count = Track.Count;

            Track.RemoveAt(index);
            if (index > 0)
            {
                Track.RemoveAt(index - 1);
            }
            if (index == 0 && count > 1)
            {
                Track.RemoveAt(0);
            }
            if (CurrentIndex > 0 && index <= CurrentIndex)
            {
                CurrentIndex -= 2;
            }
            else
            {
                RefreshIndex();
            }
        }

        internal void DeleteSlideForTest(int index)
        {
            DeleteSlide(index);
        }

        // Variable to hold a directory path
        private string directoryPath;
        public string DirectoryPath
        {
            get => directoryPath;
            set
            {
                if (directoryPath == value) return;
                directoryPath = value;
                OnPropertyChanged("DirectoryPath");
            }
        }

        // Function to refresh the directory of images
        public void RefreshDirectory(object sender, RoutedEventArgs e)
        {
            ImportDirectory();
        }

        // Function to import the new directory of images in order to choose from those pictures
        private void ImportDirectory()
        {
            if (directoryPath != "")
            {
                DirectoryImages.Clear();
                string[] filepaths = System.IO.Directory.GetFiles(directoryPath);
                foreach (string path in filepaths)
                {
                    //Trace.WriteLine(path);
                    if (DirectoryImages.Any(p => p.ImagePath == path)) continue;
                    string lowercase = path.ToLower();
                    if (lowercase.EndsWith(".jpg") || lowercase.EndsWith(".jpeg") || lowercase.EndsWith(".png") || lowercase.EndsWith(".jfif"))
                    {
                        DirectoryImages.Add(new ImageFile(path));
                    }
                }
                //Trace.WriteLine(DirectoryImages[0].ImagePath);
            }
        }

        // Class for notifying when a property has changed
        public class ImplementsINotifyPropertyChanged : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Class for an image file
        public class ImageFile : ImplementsINotifyPropertyChanged
        {
            protected string imagePath;

            public string ImagePath
            {
                get => imagePath;
                set
                {
                    if (imagePath == value) return;
                    imagePath = value;
                    OnPropertyChanged();
                }
            }
            public ImageFile(string imagePath)
            {
                this.imagePath = imagePath;
            }
        }

        // Class for a track item
        public class TrackItem : ImplementsINotifyPropertyChanged
        {
            protected string imagePath;
            public string ImagePath
            {
                get => imagePath;
                set
                {
                    if (imagePath == value) return;
                    imagePath = value;
                    OnPropertyChanged();
                }
            }
            public double duration = 1;
        }

        // Class for a slide item
        public class Slide : TrackItem
        {
            public int imageIndex { get; protected set; }

            public Slide(int imageIndex)
            {
                this.imageIndex = imageIndex;
                if (imageIndex >= 0)
                {
                    ImagePath = current.DirectoryImages[imageIndex].ImagePath;
                }
            }
        }

        // Class for a transition item
        public class Transition : TrackItem
        {
            private static string[] iconpaths = { "EmptySpace.png", "upArrow.png", "rightArrow.png", "downArrow.png", "leftArrow.png", "RedoIcon.png" };
            public enum TransitionTypes { None, SlideUp, SlideRight, SlideDown, SlideLeft, CrossFade };
            public TransitionTypes TransitionType
            {
                get => type;
                set
                {
                    if (type == value) return;
                    if ((int)value > 5)
                    {
                        value = (TransitionTypes)(((int)value) % 6);
                    }

                    type = value;
                    ImagePath = iconpaths[((int)type)];
                    OnPropertyChanged();
                }
            }
            private TransitionTypes type;
            public Transition(TransitionTypes transition)
            {
                TransitionType = transition;
            }
        }

        // Function for when the folder button is clicked        
        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPath = folderBrowserDialog.SelectedPath;
                ImportDirectory();
            }
        }

        // Function for when the present button is clicked
        private void PresentButton_Click(object sender, RoutedEventArgs e)
        {
            if (presentButton && Window1.current == null) //why is this checking if there is a present button?
            {
                Window1 window1 = new Window1(Track, AudioCollection, Auto_Manual.Equals("Automatic"));

                window1.Show();
            }
        }

        // Function for when the save button is clicked
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveButton)
            {
                var currentSlideShowDirectory = System.IO.Path.Combine(SaveDirectory, slideShow.SlideShowTitle);

                CreateSaveDirectory(currentSlideShowDirectory);

                SaveJson(currentSlideShowDirectory, slideShow.SlideShowTitle);
            }
        }
        
        // Function for when the save directory is clicked
        public void CreateSaveDirectory(string currentSlideShowDirectory)
        {
            if (!Directory.Exists(currentSlideShowDirectory))
            {
                System.IO.Directory.CreateDirectory(currentSlideShowDirectory);
            }

            foreach (var item in Track)
            {
                var imagePath = item.ImagePath;

                var imageName = System.IO.Path.GetFileName(imagePath);

                var newPath = System.IO.Path.Combine(currentSlideShowDirectory, imageName);

                if (File.Exists(imagePath))
                {
                    try
                    {
                        File.Copy(imagePath, newPath);
                    }
                    catch
                    { }
                }
            }

            for (int i = 0; i < Track.Count; i++)
            {
                if (i % 2 == 0)
                {
                    var imagePath = Track[i].ImagePath;

                    var imageName = System.IO.Path.GetFileName(imagePath);

                    var newPath = System.IO.Path.Combine(currentSlideShowDirectory, imageName);

                    Track[i].ImagePath = newPath;
                }
            }
        }

        // Function to save the json file
        public void SaveJson(string directory, string title)
        {
            var currentSlideShowDirectory = System.IO.Path.Combine(directory, "SlideShowConfig.json");

            SlideShow slideShow = new SlideShow(Track, AudioCollection,Auto_Manual, AutoTime, title);

            WriteToJsonFile<SlideShow>(currentSlideShowDirectory, slideShow);
        }

        // Function to write to a json file
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = Newtonsoft.Json.JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Function to read from a json file
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader.Close();
                }
            }
        }

        // Variable for whether the slideshow is automatic switching or manual
        private string auto_Manual;
        public string Auto_Manual
        {
            get { return auto_Manual; }
            set { auto_Manual = value; }
        }

        // Variable for the automatic switching time
        private string autoTime;
        public string AutoTime
        {
            get { return autoTime; }
            set { autoTime = value; }
        }

        // Function for when the automatic or manual combo box is changed
        private void AutoVSManual_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0].ToString().Length < 38)
            {
                Auto_Manual = e.AddedItems[0].ToString().Substring(36);
            }
            else
            {
                Auto_Manual = e.AddedItems[0].ToString().Substring(38);
            }
        }

        // Function for when the automatic time combo box is changed
        private void AutoSeconds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0].ToString().Length < 38)
            {
                AutoTime = e.AddedItems[0].ToString().Substring(36);
            }
            else
            {
                AutoTime = e.AddedItems[0].ToString().Substring(38);
            }
        }

        // Function for when the open button is clicked
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (openButton)
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

                System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    DirectoryPath = folderBrowserDialog.SelectedPath;

                    SetSlideShowValues();

                    RefreshIndex();

                    this.Title = slideShow.SlideShowTitle;
                }
            }
        }

        // Function to set the slideshow values when a new slideshow is opened
        public void SetSlideShowValues()
        {
            var configPath = System.IO.Path.Combine(DirectoryPath, "SlideShowConfig.json");

            if (File.Exists(configPath))
            {
                SlideShow slideshow = ReadFromJsonFile<SlideShow>(configPath);

                Slide_Show = slideshow;

                Track = Slide_Show.Track;
                AudioCollection = Slide_Show.AudioCollection;
                AutoTime = Slide_Show.AutoTime;
                Auto_Manual = Slide_Show.Auto_Manual;
                
                for (int i = 0; i < Track.Count; i++)
                {
                    if (i % 2 != 0)
                    {
                        if (Track[i].ImagePath.Contains("upArrow"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.SlideUp);
                        }
                        else if (Track[i].ImagePath.Contains("downArrow"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.SlideDown);
                        }
                        else if (Track[i].ImagePath.Contains("leftArrow"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.SlideLeft);
                        }
                        else if (Track[i].ImagePath.Contains("rightArrow"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.SlideRight);
                        }
                        else if (Track[i].ImagePath.Contains("RedoIcon"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.CrossFade);
                        }
                        else if (Track[i].ImagePath.Contains("EmptySpace"))
                        {
                            Track[i] = new Transition(Transition.TransitionTypes.None);
                        }
                    }
                }

                openButton = false;
                newButton = false;
                presentButton = true;
                closeButton = true;
                saveButton = true;
            }
            else
            {
                MessageBox.Show("The slideshow config file could not be found.", "",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Function for when the new button is clicked
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (newButton)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("Enter the name of your new SlideShow:","New SlideShow Name", "", 0, 0);

                var fileExists = System.IO.Directory.Exists(System.IO.Path.Combine(SaveDirectory, input));

                if (fileExists)
                {
                    MessageBox.Show("A slide show with this name already exists.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (input != "")
                    {
                        slideShow.SlideShowTitle = input;

                        var currentSlideShowDirectory = System.IO.Path.Combine(SaveDirectory, slideShow.SlideShowTitle);

                        CreateSaveDirectory(currentSlideShowDirectory);

                        SaveJson(currentSlideShowDirectory, slideShow.SlideShowTitle);

                        this.Title = slideShow.SlideShowTitle;

                        openButton = false;
                        newButton = false;
                        presentButton = true;
                        closeButton = true;
                        saveButton = true;
                    }
                }
            }
        }

        // Function for when the close button is clicked
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (closeButton)
            {
                var result = MessageBox.Show("Would you like to save your changes?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var currentSlideShowDirectory = System.IO.Path.Combine(SaveDirectory, slideShow.SlideShowTitle);

                    CreateSaveDirectory(currentSlideShowDirectory);

                    SaveJson(currentSlideShowDirectory, slideShow.SlideShowTitle);

                    Slide_Show = new SlideShow();
                    Track = Slide_Show.Track;
                    AudioCollection = Slide_Show.AudioCollection;
                    AutoTime = Slide_Show.AutoTime;
                    Auto_Manual = Slide_Show.Auto_Manual;
                    this.Title = Slide_Show.SlideShowTitle;
                }
                else
                {
                    Slide_Show = new SlideShow();
                    Track = Slide_Show.Track;
                    AudioCollection = Slide_Show.AudioCollection;
                    AutoTime = Slide_Show.AutoTime;
                    Auto_Manual = Slide_Show.Auto_Manual;
                }

                RefreshIndex();

                openButton = true;
                newButton = true;
                presentButton = false;
                closeButton = false;
                saveButton = false;
            }
        }

        // Function for when the add audio button is clicked
        private void AddAudioButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3, *.wav, *.aiff)|*.mp3; *.wav; *.aiff|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!AudioCollection.Contains(openFileDialog.FileName)) AudioCollection.Add(openFileDialog.FileName);
            }
        }

        // Function for when the audio preview is clicked
        private void AudioPreview_Click(object sender, RoutedEventArgs e)
        {
            string audioFilepath = (sender as Button).DataContext as string;
            PreviewAudio(audioFilepath);
            
        }

        // Function to play audio for preview
        private void PreviewAudio(string audioFilepath)
        {
            if (mediaPlayer.Source != null)
            {
                if (mediaPlayer.Source.OriginalString == audioFilepath)
                {
                    mediaPlayer.Close();
                    return;
                }
                
            }
            mediaPlayer.Open(new Uri(audioFilepath));
            mediaPlayer.Play();
        }

        // Function for when the audio track is selected
        private void Track_SelectAudio(object sender, RoutedEventArgs e)
        {
            string audioFilepath = (sender as Button).DataContext as string;
            PreviewAudio(audioFilepath);
        }

        //Moves audio higher in the playlist
        private void AudioPreview_MoveUp(object sender, RoutedEventArgs e)
        {
            int index = GetIndex(sender, AudioCollection);
            if (index <= 0) return;
            string tmp = AudioCollection[index - 1];
            AudioCollection[index - 1] = AudioCollection[index];
            AudioCollection[index] = tmp;
        }
        private void AudioPreview_MoveDown(object sender, RoutedEventArgs e)
        {
            int index = GetIndex(sender, AudioCollection);
            if (index >= AudioCollection.Count - 1) return;
            string tmp = AudioCollection[index + 1];
            AudioCollection[index + 1] = AudioCollection[index];
            AudioCollection[index] = tmp;
        }



        private void Audio_Delete_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Close();
            int index = GetIndex(sender, AudioCollection);
            AudioCollection.RemoveAt(index);
        }
    }

    public class AudioNameParser : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string) value;
            string[] tokens = s.Split('\\');

            return (tokens.Last());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
