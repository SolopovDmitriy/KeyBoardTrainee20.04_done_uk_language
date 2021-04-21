using KeyBoardTrainee.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace KeyBoardTrainee
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string DefaultLanguage { get; set; } = "uk";// my code -------------------------------------------------------

        private string[] _quests = {
           "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
           "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur",
           "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum"
        };
        private int _indexQuest = -1; //идекс текущей задачи
        private int _indexCurrentLetter = 0; //индекс вводимого символа
        private string _userResult = "";
        private int _countFails = 0; //количество ошибок
        private int _countTotal = 0; //количество нажатых клавиш

        private DispatcherTimer _taskTimer; //таймер одного раунда
        private DateTime _startTime;
        private DateTime _endTime;
        private TimeSpan _elapsedTime; //_endTime - _startTime - прошедшее время

        private List<Border> _buttons;

        public MainWindow()
        {
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru");

            InitializeComponent();//1 - инициализация (создание) всех єлементов на главной форме
        }

        private void UpdateUILocale()
        {
            this.Title = Strings.MainForm;
            Button_End.Content = Strings.Button_End;
            Button_Start.Content = Strings.Button_Start;
            Button_Results.Content = Strings.Button_Results;
            TextBlock_Fails.Text = Strings.TextBlock_Fails;
            TextBlock_Speed.Text = Strings.TextBlock_Speed;
            TextBlock_Total.Text = Strings.TextBlock_Total;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)//2-метод, который выполняется после инициализации компонентов
        {



            //// gsdgh 345 sdgdfg 083-955-4474 sdrgrthgrth  fghdfh 45 345  sgth 099-888-2222 srtgsth  \d

            ////Regex r2 = new Regex(@"\.(\w+)\.(\w+)\.resx", RegexOptions.IgnoreCase);//\.(\w+)- один или более алфавитно-цифрофой символ
            //Regex r2 = new Regex(@"(\d{3})-(\d{3})-\d{4}", RegexOptions.IgnoreCase);
            //Match m2 = r2.Match("gsdgh 083-955-4474 sdrgrthgrth  fghdfh 45 345  sgth 099-888-2222 srtgsth ");//посик по регулярному выражения в item - имя файла, m =.ru.resx

            //Group g1 = m2.Groups[1];//(\w+)=Groups, часть найденной строки, которая в круглых скобках, часть шаблона
            //Group g2 = m2.Groups[2];
            //MessageBox.Show(g1.Value + "=" + g2.Value);





            //UpdateUILocale();

            _buttons = new List<Border>();// создаем пустой список для хранения borders
            _taskTimer = new DispatcherTimer();// создаем таймер
            _taskTimer.Interval = new TimeSpan(1000);//1000 мс - интервал с которым тикает таймер
            _taskTimer.Tick += _taskTimer_Tick;//на каждый тик выполняется метод _taskTimer_Tick, что делать , когда произойдет событие на тик
            Button_End.IsEnabled = false;//неактивная кнопка End

            foreach (StackPanel onePanel in StackButtonPanel.Children.OfType<StackPanel>())
            {
                foreach (Grid item in onePanel.Children.OfType<Grid>())
                {
                    _buttons.Add(item.Children.OfType<Border>().First());
                }
            }

            //string Patch = @"..\..\lang\";
            //string pathTodir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\lang";
            LanguageSwitcher languageSwitcher = new LanguageSwitcher(@"..\..\lang\", "Strings");

            foreach (var item in languageSwitcher.Lang)
            {
                ComboBox_Lang.Items.Add(item);
            }
            // ComboBox_Lang.SelectedIndex = 2;
            ComboBox_Lang.SelectedItem = DefaultLanguage;// my code ----------------------------------------------------------------

        }

        private void _taskTimer_Tick(object sender, EventArgs e)
        {

        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!_taskTimer.IsEnabled)
            {
                _startTime = DateTime.Now;
                Button_End.IsEnabled = true;
                Button_Start.IsEnabled = false;
                _taskTimer.Start();
                Random random = new Random();
                _indexQuest = random.Next(0, _quests.Length);
                RTextBox_QuestText.Document.Blocks.Clear();
                RTextBox_QuestText.Document.Blocks.Add(new Paragraph(new Run(_quests[_indexQuest])));
                RTextBox_QuestText.CaretPosition = RTextBox_QuestText.CaretPosition.DocumentStart;
                _indexCurrentLetter = 0;
                _countFails = 0;
                _countTotal = 0;
            }
        }

        private void Button_End_Click(object sender, RoutedEventArgs e)
        {
            if (_taskTimer.IsEnabled)
            {
                Button_Start.IsEnabled = true;
                Button_End.IsEnabled = false;

                _endTime = DateTime.Now;
                _elapsedTime = _endTime - _startTime;
                RTextBox_UserText.Document.Blocks.Clear();
                
                _taskTimer.Stop();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               
                if (!_taskTimer.IsEnabled) return;

                int keyKode = Convert.ToInt32(e.Key);
                string keySymbol = e.Key.ToString();

                //MessageBox.Show(keyKode.ToString());
                if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.None)
                {
                    keySymbol = keySymbol.ToLower();
                }

                if (keySymbol.Length == 1 || keyKode == 18 || keyKode == 2)
                {
                    var startPosition = RTextBox_QuestText.CaretPosition;
                    var endPosition = RTextBox_QuestText.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                    _userResult = "";

                    if (Keyboard.IsKeyDown(Key.LeftShift) == true)
                    {
                        keySymbol = keySymbol.ToUpper();
                    }
                    if (Keyboard.IsKeyDown(Key.RightShift) == true)
                    {
                        keySymbol = keySymbol.ToUpper();
                    }

                    if (keyKode == 18)
                    {
                        _userResult = " ";
                    }
                    else if (keyKode == 2) //BackSpace
                    {
                        string richText = new TextRange(RTextBox_UserText.Document.ContentStart, RTextBox_UserText.Document.ContentEnd).Text;
                        if (richText.Length > 0)
                        {
                            richText = richText.Substring(0, richText.Length - 3);
                        }

                        RTextBox_UserText.Document.Blocks.Clear();
                        RTextBox_UserText.AppendText(richText);
                    }
                    else
                    {
                        _userResult = keySymbol;
                    }

                    if (_userResult.Length > 0 && _quests[_indexQuest][_indexCurrentLetter] == _userResult[0])
                    {
                        //угадал
                        var textRange = new TextRange(startPosition, endPosition);
                        textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
                        RTextBox_QuestText.CaretPosition = endPosition;

                        _indexCurrentLetter++;
                    }
                    else
                    {
                        _countFails++;
                        //var startPosition = RTextBox_QuestText.CaretPosition.GetPositionAtOffset(_indexCurrentLetter - 1);
                        //var endPosition = RTextBox_QuestText.CaretPosition.GetPositionAtOffset(_indexCurrentLetter);
                        
                        var textRange = new TextRange(startPosition, endPosition);
                        textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Red);
                        //RTextBox_QuestText.CaretPosition = endPosition;

                        TextBlock_Fails.Text = Strings.TextBlock_Fails + _countFails; // my code ----------------------------------------------------------------
                    }

                    RTextBox_UserText.AppendText(_userResult);

                    _countTotal++;
                    TextBlock_Total.Text = Strings.TextBlock_Total + _countTotal; // my code ----------------------------------------------------------------
                }
                else
                {
                    return;
                }


                foreach (Border oneButton in _buttons)
                {
                    if (((TextBlock)oneButton.Child).Text.ToUpper().Equals(keySymbol.ToUpper()))
                    {
                        Brush oldColor = oneButton.Background; //сохраню старый цвет кнопки
                        if (oldColor != Brushes.DimGray)
                        {
                            oneButton.Background = Brushes.DimGray;
                            BlinkBackground(oldColor);
                        }

                        async Task BlinkBackground(Brush brush)
                        {
                            await Task.Delay(250);
                            oneButton.Background = brush; //везврат к исходному состоянию
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Results_Click(object sender, RoutedEventArgs e)
        {
            BestResultsWindow bestResultsWindow = new BestResultsWindow();
            //bestResultsWindow.Show(); //запуск в режиме немодальное окно
            bestResultsWindow.Owner = this;
            bestResultsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            
            var loc = this.PointToScreen(new Point(0, 0));
            bestResultsWindow.Left = loc.X + this.Width;
            bestResultsWindow.Top = loc.Y;


            bestResultsWindow.ShowDialog(); //запуск в режиме модальное окно
        }

        private void ComboBox_Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            Thread.CurrentThread.CurrentUICulture = 
                new System.Globalization.CultureInfo(((ComboBox)sender).SelectedItem.ToString());
            UpdateUILocale();
        }
    }
}


// таймер обратного отсчета
// скорость/эффективность
