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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace _499_P09_SlideShowCreator
{
    /// <summary>
    /// Interaction logic for SlideOptions.xaml
    /// </summary>
    public partial class SlideOptions : Window
    {
        private static readonly Regex validDurationExpression = new Regex("[^0-9.]+");
        public MainWindow.TrackItem slide;
        public double duration
        {
            get => slide.duration;
            set => slide.duration = value;
        }
        public SlideOptions(MainWindow.TrackItem slide)
        {
            this.slide = slide;
            DataContext = this;
            InitializeComponent();
        }

        private void DeleteSlideButton_Click(object sender, RoutedEventArgs e)
        {
            int index = MainWindow.current.Track.IndexOf(slide);
            MainWindow.current.DeleteSlide(index);
            
            Close();
        }

        private void ValidateDurationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = validDurationExpression.IsMatch(e.Text);
        }
    }
}
