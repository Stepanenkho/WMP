using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using WMP;

namespace WMPWindow
{

    public partial class MainWindow : Window
    {
        List<string> LectureList = new List<string>();
        public XMLManager manager;
        public Multimedia m;
        public PlaylistManager p;
        bool isImage = false;
        const string messageErrorFile = "The selected file has an extension that is not recognized by WindowsMediaPlayerV2";
        bool isPlaying = true;
        bool fullscreen = false;
        bool running = false;
        public TimeSpan currentTime;
        TimeSpan timeSpan;
        DispatcherTimer minut = new DispatcherTimer();
        DispatcherTimer timer_hidden;
        System.Timers.Timer timeClick = new System.Timers.Timer(System.Windows.Forms.SystemInformation.DoubleClickTime)
        {
            AutoReset = false
        };

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            VideoList.MouseDoubleClick += new MouseButtonEventHandler(Video_MouseDoubleClick);
            MusicList.MouseDoubleClick += new MouseButtonEventHandler(Music_MouseDoubleClick);
            ImageList.MouseDoubleClick += new MouseButtonEventHandler(Image_MouseDoubleClick);
            PlaylistBox.MouseDoubleClick += new MouseButtonEventHandler(Playlist_MouseDoubleClick);
            SliderMain.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SliderMain_MouseTimer), true);
            SliderVol.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SliderVol_MouseChanged), true);
          }
        
         private void Visibility_move(object sender, MouseEventArgs e)
        {
            if (timer_hidden != null)
            {
                timer_hidden.Tick -= timer_Tick;
            }
            if (fullscreen == true)
            {
                    timer_hidden = new DispatcherTimer();
                    timer_hidden.Interval = new TimeSpan(0, 0, 3);
                    timer_hidden.Tick += new EventHandler(timer_Tick);
                    timer_hidden.Start();
            }
        }

         void timer_Tick(object sender, EventArgs e)
        {
            timer_hidden.Stop();
            DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            LayoutCommand.BeginAnimation(Grid.OpacityProperty, anim);
            LayoutCommand.Visibility = Visibility.Hidden; 
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(3, TimeSpan.FromSeconds(2));
            LayoutCommand.BeginAnimation(Grid.OpacityProperty, anim);
            LayoutCommand.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            LayoutCommand.BeginAnimation(Grid.OpacityProperty, anim);
            LayoutCommand.Visibility = Visibility.Hidden;
        }

        private void Event_MediaOpened(object sender, RoutedEventArgs e)
        {
            SliderMain.Minimum = 0;
           if (isImage == false)
                SliderMain.Maximum = MediaLec.NaturalDuration.TimeSpan.TotalSeconds;
            else
                SliderMain.Maximum = (Double)4.0;
            
        }

        private void SliderMain_MouseTimer(object sender, MouseButtonEventArgs e)
        {
            MediaLec.Position = TimeSpan.FromSeconds(SliderMain.Value);
            currentTime = MediaLec.Position;
        }

        private void SliderVol_MouseChanged(object sender, MouseButtonEventArgs e)
        {
            MediaLec.Volume = SliderVol.Value / 10.0;
        }

        private void SliderMain_Timer(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
                TimeText.Text = string.Format("{0:00}:{1:00}:{2:00}", currentTime.Hours, currentTime.Minutes, currentTime.Seconds);
                if (MediaLec.NaturalDuration.HasTimeSpan == true)
                    FullTimeText.Text = string.Format("{0:00}:{1:00}:{2:00}", MediaLec.NaturalDuration.TimeSpan.Hours, MediaLec.NaturalDuration.TimeSpan.Minutes, MediaLec.NaturalDuration.TimeSpan.Seconds);
                else if (isImage == true)
                    FullTimeText.Text = "00:00:04";
                else
                {
                    FullTimeText.Text = "00:00:00";
                    TimeText.Text = "00:00:00";
                }
        }

        private void Media_FullScreen(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Thickness margin = LayoutMain.Margin;

            if (!timeClick.Enabled)
            {
                timeClick.Enabled = true;
                return;
            }
            if (timeClick.Enabled)
            {
                if (timeClick.Enabled)
                {
                    if (!fullscreen)
                    {
                        GridMain.Children.Remove(Multimedia);
                        GridMain.Children.Remove(PlaylistManager);
                        margin.Left = 0;
                        margin.Right = 0;
                        LayoutMain.Margin = margin;
                        this.WindowStyle = WindowStyle.None;
                        this.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        GridMain.Children.Add(Multimedia);
                        GridMain.Children.Add(PlaylistManager);
                        margin.Left = 250;
                        margin.Right = 250;
                        LayoutMain.Margin = margin;
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        this.WindowState = WindowState.Normal;
                    }
                    MediaLec.Position = currentTime;
                    fullscreen = !fullscreen;
                }
            }
        }
        private void Event_MediaEnded(object sender, EventArgs e)
        {
            MediaLec.Stop();
            LectureList.RemoveAt(0);
            if (LectureList.Count() == 0)
                running = false;
            else
            {
                    PlaySource();
                    return;
            }
            MediaLec.ClearValue(MediaElement.SourceProperty);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
           if (isPlaying == true && running == true)
            {
                MediaLec.Play();
                ButtonPlay.Source = (ImageSource)FindResource("Button_Pause");
                isPlaying = false; 
           }
           else if (isPlaying == false && running == true)
            {
                ButtonPlay.Source = (ImageSource)FindResource("Button_Play");
                MediaLec.Pause();
                isPlaying = true;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MediaLec.Stop();
            MediaLec.ClearValue(MediaElement.SourceProperty);
            isPlaying = false;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            MediaLec.Position = MediaLec.Position - TimeSpan.FromSeconds(10);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MediaLec.Position += TimeSpan.FromSeconds(10);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                if (manager.GetMIMEType(dialog.FileName) != "unknown")
                {

                    if (m.addElement(new Element(System.IO.Path.GetFileNameWithoutExtension(dialog.FileNames[0]), manager.GetMIMEType(dialog.FileName), dialog.FileName)) == true)          
                    {
                        refresh();
                        LectureListCheck(dialog.FileName, true);
                    }
                }
                else
                {
                    MessageBox.Show(messageErrorFile, "Windows Media Player V2", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return ;
                }
            }
        }

        void LectureListCheck(String PathFile, bool now)
        {
            
            if (now == true)
                LectureList.Clear();
            if (PathFile == null || manager.GetMIMEType(PathFile) == "unknown")
            {
                MessageBox.Show(messageErrorFile, "Windows Media Player V2", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            LectureList.Add(PathFile);
            if (now == true)
                PlaySource();
        }

        void PlaySource()
        {
              running = true;
              if (LectureList.Count() >= 1)
              {
                  if (manager.GetMIMEType(LectureList[0]) == "image")
                      isImage = true;
                  else
                      isImage = false;
                  MediaLec.Source = new Uri(LectureList[0]);  
                  SliderVol.Value = MediaLec.Volume * 10;
                  minut.Interval = TimeSpan.FromMilliseconds(1000);
                  minut.Tick += new EventHandler(timeFall);
                  minut.Start();
                  ButtonPlay.Source = (ImageSource)FindResource("Button_Pause");
                  MediaLec.Play();
              }
        }

        void timeFall(object sender, EventArgs e)
        {
            this.timeSpan = MediaLec.Position;
            SliderMain.Value = this.timeSpan.TotalSeconds;
            currentTime = this.timeSpan;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            manager.saveAllElements(m.getAllElements(), p.getContent());
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            manager = new XMLManager();
            m = new Multimedia();
            p = new PlaylistManager();
            manager.loadAllElements();
            m.init(manager.getObjects());
            p.init(manager.getPlaylists());
            fillVideo(null);
            fillMusic(null);
            fillImage(null);
            fillPlaylist();
        }

        void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                refresh();
        }

        void Video_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (VideoList.SelectedItem != null)
                if (m.getElement(VideoList.SelectedItem.ToString()) != null)
                    LectureListCheck(m.getElement(VideoList.SelectedItem.ToString()).getPath(), true);
        }

        void Music_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MusicList.SelectedItem != null)
                if (m.getElement(MusicList.SelectedItem.ToString()) != null)
                    LectureListCheck(m.getElement(MusicList.SelectedItem.ToString()).getPath(), true);
             }

        void Image_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ImageList.SelectedItem != null)
                if (m.getElement(ImageList.SelectedItem.ToString()) != null)
                    LectureListCheck(m.getElement(ImageList.SelectedItem.ToString()).getPath(), true);
        }

        void Playlist_MouseDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (p.getActual() == null)
            {
                if (PlaylistBox.SelectedIndex != -1)
                {
                    List<string> l = p.getPlaylist(PlaylistBox.SelectedItem.ToString()).getContent();
                    if (l.Count() < 1)
                        return;
                    LectureList.Clear();
                    foreach (string s in l)
                        LectureListCheck(m.getElement(s).getPath(), false);
                    PlaySource();
                }
            }
            else
            {
                if (PlaylistBox.SelectedIndex != -1)
                    LectureListCheck(m.getElement(PlaylistBox.SelectedItem.ToString()).getPath(), true);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (p.getActual() == null)
            {
                if (PlaylistName.Text != null && PlaylistName.Text != "")
                {
                    p.addPlaylist(new Playlist(PlaylistName.Text));
                    PlaylistName.Text = "";
                }
            }
            else
            {
                if (VideoList.SelectedIndex != -1 && VideoList.SelectedItem.ToString() != "")
                    p.getActual().addObject(VideoList.SelectedItem.ToString());
                if (MusicList.SelectedIndex != -1 && MusicList.SelectedItem.ToString() != "")
                    p.getActual().addObject(MusicList.SelectedItem.ToString());
                if (ImageList.SelectedIndex != -1 && ImageList.SelectedItem.ToString() != "")
                    p.getActual().addObject(ImageList.SelectedItem.ToString());
            }
            fillPlaylist();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistBox.SelectedIndex != -1)
            {
                if (p.getActual() != null)
                    p.getActual().removeObject(PlaylistBox.SelectedItem.ToString());
                else
                    p.removePlaylist(PlaylistBox.SelectedItem.ToString());
                fillPlaylist();
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (p.getActual() == null)
            {
                if (PlaylistBox.SelectedItem != null)
                {
                    p.setActual(PlaylistBox.SelectedItem.ToString());
                    Enter.Content = "<";
                }
            }
            else
            {
                Enter.Content = ">";
                p.setActual(null);
            }
            fillPlaylist();
        }

        void fillPlaylist()
        {
            PlaylistBox.Items.Clear();
            if (p.getActual() == null)
            {
                PlaylistTitle.Text = "Playlist";
                foreach (Playlist l in p.getContent())
                    PlaylistBox.Items.Add(l.getName());
            }
            else
            {
                PlaylistTitle.Text = p.getActual().getName();
                foreach (string s in p.getActual().getContent())
                    PlaylistBox.Items.Add(s);
            }
        }
        
        void fillVideo(string search)
        {
            List<Element> video = m.getVideo();

            if (video.Count() != 0)
                foreach (Element i in video)
                    if (search == null || i.getName().Contains(search) == true)
                        VideoList.Items.Add(i.getName());
        }

        void fillMusic(string search)
        {
            List<Element> music = m.getMusic();

            if (music.Count() != 0)
                foreach (Element i in music)
                    if (search == null ||i.getName().Contains(search) == true)
                        MusicList.Items.Add(i.getName());
        }

        void fillImage(string search)
        {
            List<Element> image = m.getImage();

            if (image.Count() != 0)
                foreach (Element i in image)
                    if (search == null || i.getName().Contains(search) == true)
                        ImageList.Items.Add(i.getName());
        }

        void refresh()
        {
            string search = Search.Text;

            VideoList.Items.Clear();
            MusicList.Items.Clear();
            ImageList.Items.Clear();
            fillVideo(search);
            fillMusic(search);
            fillImage(search);
        }

        private void MediaLec_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void MediaLec_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (string s in files)
            {
                if (manager.GetMIMEType(s) != "unknown")
                {
                    if (m.addElement(new Element(System.IO.Path.GetFileNameWithoutExtension(s), manager.GetMIMEType(s), s)) == true)
                        refresh();
                    LectureListCheck(s, true);
                }
            }
        }

        private void PlaylistBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void PlaylistBox_Drop(object sender, DragEventArgs e)
        {
            if (p.getActual() == null)
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                if (manager.GetMIMEType(file) != "unknown")
                    if (m.addElement(new Element(System.IO.Path.GetFileNameWithoutExtension(file), manager.GetMIMEType(file), file)) == true)
                        refresh();
                p.getActual().addObject(System.IO.Path.GetFileNameWithoutExtension(file));
                fillPlaylist();
            }
        }
    }
}