using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;
using static _499_P09_SlideShowCreator.MainWindow;
using System.Windows.Threading;
using System.Diagnostics;

namespace _499_P09_SlideShowCreator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        // Current window
        public static Window1 current;
        private DispatcherTimer slideTimer;
        public readonly bool isAutomatic;
        // Constructor for a new window1
        public Window1(ObservableCollection<TrackItem> _track, ObservableCollection<string> _audioCollection, bool isAutomatic)
        {
            InitializeComponent();
            current = this;
            DataContext = this;
            this.isAutomatic = isAutomatic;
            Track = _track;

            SlideVisuals.SetSlide(track[0]);
            //SlideVisuals.SetNext(track[2] as Slide, track[1] as Transition);
            //CurrentSlide = Track[0].ImagePath;
            AudioCollection = _audioCollection;
            slideTimer = new DispatcherTimer();
            slideTimer.Tick += SlideTimer_Tick;
            slideTimer.Interval = TimeSpan.FromSeconds(track[0].duration);

            slideTimer.Start();
            InitAudio();
        }

        // Variable for when a property is changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Variable for a track item
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

        //// Variable for the current slide
        //private string currentSlide;
        //public string CurrentSlide
        //{
        //    get => currentSlide;
        //    set
        //    {
        //        if (currentSlide == value) return;
        //        currentSlide = value;
        //        OnPropertyChanged();
        //    }
        //}

        // Variable for the current index
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
                //CurrentSlide = track[currentIndex].ImagePath;
                
                OnPropertyChanged();
            }
        }

        //called when the current slide duration is over
        public void SlideTimer_Tick(object sender, EventArgs e) 
        {
            slideTimer.Stop();
            if (CurrentIndex < Track.Count - 1 && isAutomatic)
            {
                SlideVisuals.SetNext(track[currentIndex + 2], track[currentIndex + 1] as Transition);
                SlideVisuals.StartTransition(); //starts transition and at the end of transition, if isAutomatic = true, NextSlide is called
            }
        }
        
        // Function to go to the previous slide
        public void PreviousSlide(object sender, RoutedEventArgs e)
        {
            if (SlideVisuals.IsInTransition) return;

            if (isAutomatic)
            {
                PreviousSlide_Automatic(); //should never be used
            }
            else
            {
                PreviousSlide_Manual();
            }
        }
        public void PreviousSlide_Manual()
        {
           
            if (currentIndex >= 2)
            {
                SlideVisuals.SetNext(track[currentIndex - 2], track[currentIndex - 1] as Transition);
                SlideVisuals.StartTransition();
                CurrentIndex -= 2;
            }
        }
        public void PreviousSlide_Automatic()
        {
            slideTimer.Stop();
            if (currentIndex >= 2)
            {
                slideTimer.Interval = TimeSpan.FromSeconds(track[currentIndex].duration);
                slideTimer.Start();
                SlideVisuals.SetNext(track[currentIndex - 2], track[currentIndex - 1] as Transition);
                CurrentIndex -= 2;
            }
        }
        // Function to go to the next slide
        public void NextSlide(object sender, RoutedEventArgs e)
        {
            if (SlideVisuals.IsInTransition) return;

            if (isAutomatic)
            {
                NextSlide_Automatic();
            }
            else
            {
                NextSlide_Manual();
            }

            
            
            //Trace.WriteLine(CurrentIndex);
            
        }
        private void NextSlide_Manual()
        {
            //Trace.WriteLine("NEXT");
            
            if (currentIndex < Track.Count - 1)
            {
                SlideVisuals.SetNext(track[currentIndex + 2], track[currentIndex + 1] as Transition);
                SlideVisuals.StartTransition();
                CurrentIndex += 2;
            }
        }
       
        private void NextSlide_Automatic() //called after the current slide's transition is finished (through NextSlide(...) )
        {
            slideTimer.Stop();
            if (currentIndex < Track.Count - 1)
            {
                slideTimer.Interval = TimeSpan.FromSeconds(track[currentIndex + 2].duration); // this call for the next slide in the track to be played with its transition
                //Trace.WriteLine("Index ( " + (currentIndex + 2) + " ): Time ( " + track[currentIndex + 2].duration + " )");
                slideTimer.Start();
                
                CurrentIndex += 2;
            }
        }
        // Variable for the current audio name
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

        // Variable for the current audio time
        private string currentAudioTime;
        public string CurrentAudioTime
        {
            get => currentAudioTime;
            set
            {
                if (currentAudioTime == value) return;
                currentAudioTime = value;
                OnPropertyChanged();
            }
        }

        // Variable for the current audio index
        private int currentAudioIndex = 0;

        // Variable for the media player to play the audio
        private MediaPlayer mediaPlayer = new MediaPlayer();

        // Variabale to hold the collection of audio
        private ObservableCollection<string> audioCollection;
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

        // Function to initialize the audio
        private void InitAudio()
        {
            mediaPlayer.MediaOpened += OnAudioOpen;
            mediaPlayer.MediaEnded += OnAudioEnd;
            if (AudioCollection.Count == 0)
            {
                //Trace.WriteLine("NO AUDIO COLLECTION " + AudioCollection.Count);
                return;
            }
            mediaPlayer.Open(new Uri(AudioCollection[0]));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += AudioTimerTick;
            timer.Start();
        }

        // Function to tack the audio timer
        private void AudioTimerTick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source == null) return;
            if (mediaPlayer.NaturalDuration.HasTimeSpan == false) return;
            CurrentAudioTime = "( " + (mediaPlayer.Position.ToString(@"mm\:ss")) + " / " + (mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss")) + " )";
        }

        // Function to open the audio
        private void OnAudioOpen(object sender, EventArgs e)
        {
            Trace.WriteLine("Opening Audio File");
            CurrentAudioName = mediaPlayer.Source.Segments.Last().ToString();
            mediaPlayer.Play();
        }

        // Function for when the audio ends
        private void OnAudioEnd(object sender, EventArgs e)
        {
            Trace.WriteLine("Audio Clip Ending");
            if (audioCollection.Count > currentAudioIndex + 1)
            {
                Trace.WriteLine("Moving to Next Audio File in Collection");
                currentAudioIndex++;
                mediaPlayer.Open(new Uri(audioCollection[currentAudioIndex]));
            }
            else
            {
                Trace.WriteLine("Restarting Collection");
                currentAudioIndex = 0;
                mediaPlayer.Open(new Uri(audioCollection[0]));
            }
        }

        // Function for when the presentation is closed
        protected override void OnClosed(EventArgs e)
        {
            mediaPlayer.Stop();
            current = null;
            base.OnClosed(e);
        }

        internal void PreviousSlide_AutomaticForTest() { }

        internal void PreviousSlide_ManualForTest() { }

        internal void NextSlide_ManualForTest() { }

        internal void NextSlide_AutomaticForTest() { } 

        internal void PreviousSlideForTest()
        {
            if (isAutomatic)
            {
                PreviousSlide_AutomaticForTest(); 
            }
            else
            {
                PreviousSlide_ManualForTest();
            }
        }

        internal void NextSlideForTest() //test method
        {
            if (isAutomatic)
            {
                NextSlide_AutomaticForTest();
            }
            else
            {
                NextSlide_ManualForTest();
            }
        }
    }
}
