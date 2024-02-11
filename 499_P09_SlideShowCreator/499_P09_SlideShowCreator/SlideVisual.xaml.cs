using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static _499_P09_SlideShowCreator.MainWindow;

namespace _499_P09_SlideShowCreator
{
    /*
     * Input:
     * SetSlide() -> only on startup
     * StartTransition() -> called after slide duration is over
     * SetNext() -> called by presenter class to load in the next slide
     * 
     * Output:
     * bool IsInTransition -> used to check if user wants to go to next slide the slide visual is still in transition animation [set in StartTransition() to true and in EndOfTransition() to false]
     * EndOfTransition() -> if in automatic mode, used to call the presenter class NextSlide() to load in the next slide on the track and start the next slide's timer
    */
    public partial class SlideVisual : UserControl
    {
        public TrackItem slide;
        public MainWindow.Transition transition;
        public TrackItem nextSlide;
        public DispatcherTimer timer = new DispatcherTimer();
        public bool IsInTransition = false;
        public SlideVisual()
        {
            DataContext = this;
            timer.Tick += EndOfTransition;
            InitializeComponent();
            NextSlide.Visibility = Visibility.Collapsed;
        }
        //only used on start up
        public void SetSlide(TrackItem trackItem) //MainWindow.Slide slide1) 
        {
            this.slide = trackItem;
            //Trace.WriteLine(slide1);
            Slide.Source = new BitmapImage(new Uri(trackItem.ImagePath)); //slide.ImagePath));
        }
        //called after transition is done then calls the presenter window to continue to the next slide (if in automatic mode)
        private void EndOfTransition(object sender, EventArgs e)
        {
            //updates the currents slide's image
            slide = nextSlide;
            Slide.Source = new BitmapImage(new Uri(nextSlide.ImagePath));
            //resets the current slide's position
            TranslateImage(Slide, 0, 0, 0, 0, 0); //resets position of Slide

            //hides the next slide (secondary slide used for animations)
            NextSlide.Visibility = Visibility.Collapsed;
            NextSlide.Source = new BitmapImage(new Uri("EmptySpace.png", UriKind.Relative));
            nextSlide = null;

            //stops the transition timer
            timer.Stop();
            //sets the state to not be in transition (used for manual slide maneuvering)
            IsInTransition = false;
            //signals the presenter that the next slide is loaded
            if (Window1.current.isAutomatic) Window1.current.NextSlide(null, null);

        }
        //sets the next slide on the track
        public void SetNext(TrackItem next, MainWindow.Transition transition)
        {
            nextSlide = next;
            NextSlide.Source = new BitmapImage(new Uri(nextSlide.ImagePath));
            
            this.transition = transition;
        }
        //starts a transition between the current and the next slide based on the current slide's transition type
        private void ImplementTransition(double transitionTime)
        {
            //Create new image
            timer.Interval = TimeSpan.FromSeconds(transitionTime);
            timer.Start();
            switch (transition.TransitionType)
            {
                case (MainWindow.Transition.TransitionTypes.SlideUp):
                    TranslateImage(Slide, 0, 0, 0, -ActualHeight * 1.5, transitionTime);
                    TranslateImage(NextSlide, 0, ActualHeight * 1.5, 0, 0, transitionTime);
                    break;
                case (MainWindow.Transition.TransitionTypes.SlideDown):
                    TranslateImage(Slide, 0, 0, 0, ActualHeight * 1.5, transitionTime);
                    TranslateImage(NextSlide, 0, -ActualHeight * 1.5, 0, 0, transitionTime);
                    break;
                case (MainWindow.Transition.TransitionTypes.SlideLeft):
                    TranslateImage(Slide, 0, 0, -ActualWidth * 1.5, 0, transitionTime);
                    TranslateImage(NextSlide, ActualWidth * 1.5, 0, 0, 0, transitionTime);
                    break;
                case (MainWindow.Transition.TransitionTypes.SlideRight):
                    TranslateImage(Slide, 0, 0, ActualWidth * 1.5, 0, transitionTime);
                    TranslateImage(NextSlide, -ActualWidth * 1.5, 0, 0, 0, transitionTime);
                    break;
                case (MainWindow.Transition.TransitionTypes.CrossFade):
                    FadeImage(Slide,1,0, transitionTime);
                    FadeImage(NextSlide, 0, 1, transitionTime);
                    break;
                default:
                    break;

            }

            
            
        }


        //linearly translates an image between two points over a duration
        private void TranslateImage(Image image, double x1, double y1, double x2, double y2, double duration)
        {
            TranslateTransform transform = new TranslateTransform();
            image.RenderTransform = transform;
            
            DoubleAnimation xAnimation = new DoubleAnimation(x1, x2, TimeSpan.FromSeconds(duration));
            DoubleAnimation yAnimation = new DoubleAnimation(y1, y2, TimeSpan.FromSeconds(duration));
            transform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnimation);

        }

        //animates the image's opacity between two values over a duration
        private void FadeImage(Image image, double from, double to, double duration)
        {
            //image.Opacity = from;
            DoubleAnimation animation = new DoubleAnimation(from, to, TimeSpan.FromSeconds(duration), FillBehavior.Stop);
            //animation.Completed += (s,a) => image.Opacity = 1; 
            image.BeginAnimation(Image.OpacityProperty, animation);
        }
        //How the presenter class activates the transition
        public void StartTransition(double transitionDuration = 1) 
        {
            if (nextSlide == null) return;
            NextSlide.Visibility = Visibility.Visible;
            ImplementTransition(transitionDuration);
            IsInTransition = true;
        }

    }
}
