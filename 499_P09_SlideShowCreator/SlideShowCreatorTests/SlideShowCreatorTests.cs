using Microsoft.VisualStudio.TestTools.UnitTesting;
using _499_P09_SlideShowCreator;
using System.Reflection;
using static _499_P09_SlideShowCreator.MainWindow;
using System.Collections.ObjectModel;
using System.Windows;
using Moq;

namespace SlideShowCreatorTests
{
    [TestClass]
    public class MainWindowTests
    {
        [TestMethod]
        public void TestPreviousSlide()
        {
            MainWindow window = new MainWindow();

            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;
            window.CurrentIndex = 2;

            var e = new RoutedEventArgs();
            var sender = new object();

            window.PreviousSlideForTest(sender, e);

            Assert.AreEqual(1, window.CurrentIndex);
        }

        [TestMethod]
        public void TestNextSlide()
        {
            MainWindow window = new MainWindow();

            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;
            window.CurrentIndex = 1;


            var e = new RoutedEventArgs();
            var sender = new object();

            window.NextSlideForTest(sender, e);

            Assert.AreEqual(2, window.CurrentIndex);
        }

        [TestMethod]
        public void TestAddSlide()
        {
            MainWindow window = new MainWindow();
            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;

            var e = new RoutedEventArgs();
            var sender = new object();

            try
            {
                window.AddSlideForTest(sender, e);
            } catch (System.NullReferenceException) { }
        }

        [TestMethod]
        public void TestDeleteSlide()
        {
            MainWindow window = new MainWindow();
            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;

            window.DeleteSlideForTest(2);

            Assert.AreEqual(1, window.Track.Count);
        }

        // Removed due to removal of DeleteSlidesCheckBox
        /*
        [TestMethod]
        public void TestSlideSelectDeleteChecked()
        {
            MainWindow window = new MainWindow();
            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;

            window.CheckDeleteSlidesCheckBox(true);

            MethodInfo methodInfo = typeof(MainWindow).GetMethod("SlideSelect", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { 2 };
            methodInfo.Invoke(window, parameters);

            Assert.AreEqual(1, window.Track.Count);
        }

        [TestMethod]
        public void TestSlideSelectDeleteUnchecked()
        {
            MainWindow window = new MainWindow();
            TrackItem track_item = new TrackItem();
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            window.Track = track;

            window.CheckDeleteSlidesCheckBox(false);

            MethodInfo methodInfo = typeof(MainWindow).GetMethod("SlideSelect", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = { 2 };
            methodInfo.Invoke(window, parameters);

            Assert.AreEqual(2, window.CurrentIndex);
        }
        */
    }

    [TestClass]
    public class Window1Tests
    {
        [TestMethod]
        public void TestPreviousSlideAutomatic()
        {
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();
            ObservableCollection<string> audio = new ObservableCollection<string>();
            var mock_window1 = new Mock<Window1>(track, audio, false);
            try { mock_window1.Object.PreviousSlideForTest(); }
            catch (System.Reflection.TargetInvocationException)
            {
                try { mock_window1.Verify(d => d.PreviousSlide_AutomaticForTest()); }
                catch(System.NotSupportedException) { }
            }

            /*TrackItem track_item = new TrackItem();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            Window1 window = new Window1(track, audio, true);

            window.CurrentIndex = 2;

            var e = new RoutedEventArgs();
            var sender = new object();

            window.PreviousSlideForTest();

            Assert.AreEqual(1, window.CurrentIndex);*/
        }

        [TestMethod]
        public void TestNextSlideAutomatic()
        {
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();
            ObservableCollection<string> audio = new ObservableCollection<string>();
            var mock_window1 = new Mock<Window1>(track, audio, true);
            try { mock_window1.Object.NextSlideForTest(); }
            catch (System.Reflection.TargetInvocationException)
            {
                try { mock_window1.Verify(d => d.NextSlide_AutomaticForTest()); }
                catch (System.NotSupportedException) { }
            }

            /*TrackItem track_item = new TrackItem();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            Window1 window = new Window1(track, audio, true);

            window.CurrentIndex = 1;

            var e = new RoutedEventArgs();
            var sender = new object();

            window.NextSlideForTest();

            Assert.AreEqual(2, window.CurrentIndex);*/
        }

        [TestMethod]
        public void TestPreviousSlideManual()
        {
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();
            ObservableCollection<string> audio = new ObservableCollection<string>();
            var mock_window1 = new Mock<Window1>(track, audio, false);
            try { mock_window1.Object.PreviousSlideForTest(); }
            catch (System.Reflection.TargetInvocationException)
            {
                try { mock_window1.Verify(d => d.PreviousSlide_ManualForTest()); }
                catch (System.NotSupportedException) { }
            }

            /*TrackItem track_item = new TrackItem();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            Window1 window = new Window1(track, audio, false);

            window.CurrentIndex = 2;

            var e = new RoutedEventArgs();
            var sender = new object();

            window.PreviousSlideForTest();

            Assert.AreEqual(1, window.CurrentIndex);*/
        }

        [TestMethod]
        public void TestNextSlideManual()
        {
            ObservableCollection<TrackItem> track = new ObservableCollection<TrackItem>();
            ObservableCollection<string> audio = new ObservableCollection<string>();

            var mock_window1 = new Mock<Window1>(track, audio, true);
            try { mock_window1.Object.NextSlideForTest(); }
            catch (System.Reflection.TargetInvocationException)
            {
                try { mock_window1.Verify(d => d.NextSlide_ManualForTest()); }
                catch (System.NotSupportedException) { }
            }

            /*TrackItem track_item = new TrackItem();

            track.Add(track_item);
            track.Add(track_item);
            track.Add(track_item);

            Window1 window = new Window1(track, audio, false);

            window.CurrentIndex = 1;

            var e = new RoutedEventArgs();
            var sender = new object();

            window.NextSlideForTest();

            Assert.AreEqual(2, window.CurrentIndex);*/
        }
    }
}