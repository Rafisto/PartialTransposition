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
using NAudio.Wave;
using System.IO;
using System.Windows.Threading;
using NAudio.Utils;

namespace PartialTransposition
{
    public class Audio : WaveOut
    {
        public string path;
        public TimeSpan duration;
        public bool isPlaying;

        public Audio(string path)
        {
            this.path = path;
            this.duration = FileDuration(this.path);
            LoadAudio(path);
        }
        public void LoadAudio(string path)
        {
            FileStream fileStream = File.OpenRead(path);
            var importer = new RawSourceWaveStream(fileStream, new WaveFormat(44100, 1));
            this.Init(importer);
        }
        private TimeSpan FileDuration(string path)
        {
            return new AudioFileReader(path).TotalTime;
        }
        public void ChangePlayState()
        {
            isPlaying = !isPlaying;
            if (isPlaying) Play();
            else Pause();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;
        Audio audio;
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void LoadAUDIO(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".wav";
            fileDialog.Filter = "Wave audio (.wav)|*.wav";
            if (fileDialog.ShowDialog() == true)
            {
                string filepath = fileDialog.FileName;
                LabelFilename.Content = filepath;
                ButtonLoadAUDIO.Content = "-->";
                ButtonLoadAUDIO.Cursor = Cursors.No;

                audio = new Audio(filepath);
                SetupAudioSlider();
            }
        }

        public void SetupAudioSlider()
        {
            if (audio == null) return;
            SliderAudio.Minimum = 0;
            SliderAudio.Maximum = audio.duration.TotalSeconds;
        }

        public bool isPlaying = false;
        private void ButtonAudioPlayState(object sender, RoutedEventArgs e)
        {
            if (audio == null) return;
            audio.ChangePlayState();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (audio == null) return;
            TimeSpan currentPosition = audio.GetPositionTimeSpan();
            TimeSpan audioLength = audio.duration;
            LabelAudioTime.Content = currentPosition.ToString("mm\\:ss") + "/" + audioLength.ToString("mm\\:ss");
            SliderAudio.Value = currentPosition.TotalSeconds;
        }

    }
}
