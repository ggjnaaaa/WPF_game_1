using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using System.Reflection;

namespace WPF_game_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        int tenthsOfSecondsElapsed;
        int matchesFound;

        TextBlock lastTextBlockClicked;
        bool findingMatch = false;

        // весь список эмодзи
        List<string> defaultList = new List<string>()
        {
                "😊", "🤣", "❤️", "😍", "😒", "👌", "😘", "💕", "🤨", "🙌", "🤷‍♀️", "😎", "🎶", "😢"
        };
        // список для эмодзи, которые будут использоваться в одной игре
        List<string> listForPlay = new List<string>();
        // нужен для проверки во время заполнения списка listForPlay чтобы не было повторок
        int[] indices = new int[8];
        // считает сколько пар эмодзи уже были записаны в listForPlay
        int count = 0;
        // лучший результат
        string best;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Array.Clear(indices, 0, indices.Length);
            count = 0;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(.1);
            timer.Tick += Timer_Tick;

            foreach (TextBlock textBlock in mainGrid.Children.OfType<TextBlock>())
            {
                textBlock.Visibility = Visibility.Visible;
            }

            Random rand = new Random();

            for (int i = 0; i < 8; i++)
            {
            start:
                int ind = rand.Next(defaultList.Count);
                for (int j = 0; j < indices.Length; j++)
                    if (indices[j] == ind) goto start;
                indices[count] = ind;
                count++;
                listForPlay.Add(defaultList[ind]);
                listForPlay.Add(defaultList[ind]);
            }

            StreamReader sr = new StreamReader(@"..\..\BestTime.txt");
            best = sr.ReadLine();

            SetUpGame();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tenthsOfSecondsElapsed++;
            time.Text = (tenthsOfSecondsElapsed / 10F).ToString("0.0");
            if (matchesFound == 8)
            {
                timer.Stop();
                double b = Convert.ToDouble(best);
                double t = Convert.ToDouble((tenthsOfSecondsElapsed / 10F).ToString("0.0"));

                if (b > t)
                {
                    File.WriteAllText(@"..\..\BestTime.txt", String.Empty);

                    StreamWriter sw = new StreamWriter(@"..\..\BestTime.txt");
                    sw.WriteLine(timer);
                    sw.Close();
                }
            }
        }

        private void SetUpGame()
        {
            Random rand = new Random();
            foreach (TextBlock textBlock in mainGrid.Children.OfType<TextBlock>())
            {
                if (listForPlay.Count > 0)
                {
                    int ind = rand.Next(listForPlay.Count);
                    string text = listForPlay[ind];
                    textBlock.Text = text;
                    listForPlay.RemoveAt(ind);
                }
            }

            timer.Start();
            tenthsOfSecondsElapsed = 0;
            matchesFound = 0;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (findingMatch == false)
            {
                textBlock.Visibility = Visibility.Hidden;
                lastTextBlockClicked = textBlock;
                findingMatch = true;
            }
            else if (textBlock.Text == lastTextBlockClicked.Text)
            {
                textBlock.Visibility = Visibility.Hidden;
                findingMatch = false;
                matchesFound++;
            }
            else
            {
                lastTextBlockClicked.Visibility = Visibility.Visible;
                findingMatch = false;
            }
        }
    }
}
