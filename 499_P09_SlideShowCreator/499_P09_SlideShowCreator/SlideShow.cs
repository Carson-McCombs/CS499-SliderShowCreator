using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static _499_P09_SlideShowCreator.MainWindow;

namespace _499_P09_SlideShowCreator
{
    public class SlideShow
    {
        public SlideShow()
        {
            SlideShowTitle = "";
            Auto_Manual = "Manual";
            AutoTime = "1";
            Track = new ObservableCollection<TrackItem>();
            AudioCollection = new ObservableCollection<string>();
        }

        public SlideShow(ObservableCollection<TrackItem> track, ObservableCollection<string> audioCollection,string auto_manual, string autoTime, string slideShowTitle)
        {
            Track = track;
            AudioCollection = audioCollection;
            Auto_Manual = auto_manual;
            AutoTime = autoTime;
            SlideShowTitle = slideShowTitle;
        }

        public ObservableCollection<TrackItem> Track { get; set; }

        public ObservableCollection<string> AudioCollection { get; set; }

        public string SlideShowTitle { get; set; }

        public string Auto_Manual { get; set; }

        public string AutoTime { get; set; }
    }
}
