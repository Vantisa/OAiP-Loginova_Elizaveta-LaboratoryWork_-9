using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SortingApp
{
    public partial class MainWindow : Window
    {
        private Context _context;
        private int[] _currentArray;
        private StringBuilder _buffer;
        private int _bufferThreshold = 20;
        private int _lineCount;
        private bool _useDetailedOutput = true;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            _buffer = new StringBuilder();

            sliderArraySize.ValueChanged += SliderArraySize_ValueChanged;
        }

        private void SliderArraySize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtArraySizeValue.Text = ((int)sliderArraySize.Value).ToString();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = (int)sliderArraySize.Value;

                if (size <= 0 || size > 150)
                {
                    MessageBox.Show("Размер массива должен быть от 1 до 150", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _useDetailedOutput = size <= 30;

                Random rand = new Random();
                _currentArray = new int[size];
                for (int i = 0; i < size; i++)
                {
                    _currentArray[i] = rand.Next(1, 1000);
                }

                Context.Array = (int[])_currentArray.Clone();
                DisplayOriginalArray();
                ClearStepByStep();
                txtSortedArray.Text = "";
                txtStatistics.Text = "";

                if (!_useDetailedOutput)
                {
                    AppendStep($"Массив из {size} элементов !!!\n");
                    AppendStep($"Детальный пошаговый вывод отключен для ускорения работы.\n");
                    AppendStep($"Будут показаны только основные этапы сортировки.\n\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации массива: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Выберите файл с данными"
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(ofd.FileName);
                    string[] parts = content.Split(new char[] { ' ', ',', ';', '\t', '\n', '\r' },
                        StringSplitOptions.RemoveEmptyEntries);

                    _currentArray = parts.Select(int.Parse).ToArray();
                    _useDetailedOutput = _currentArray.Length <= 30;

                    Context.Array = (int[])_currentArray.Clone();
                    DisplayOriginalArray();
                    ClearStepByStep();
                    txtSortedArray.Text = "";
                    txtStatistics.Text = "";

                    if (!_useDetailedOutput)
                    {
                        AppendStep($"Массив из {_currentArray.Length} элементов !!!\n");
                        AppendStep($"Детальный пошаговый вывод отключен для ускорения работы.\n\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (_currentArray == null || _currentArray.Length == 0)
            {
                MessageBox.Show("Нет данных для сохранения", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Сохранить результат",
                FileName = "sorted_result.txt"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    string content = "Исходный массив \n";
                    content += string.Join(" ", _currentArray) + "\n\n";
                    content += "Отсортированный массив \n";
                    content += string.Join(" ", Context.Array) + "\n\n";
                    content += " Статистика сортировки \n";
                    content += txtStatistics.Text + "\n\n";
                    content += "Пошаговый процесс\n";

                    TextRange textRange = new TextRange(rtbStepByStep.Document.ContentStart,
                        rtbStepByStep.Document.ContentEnd);
                    content += textRange.Text;

                    File.WriteAllText(sfd.FileName, content);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _currentArray = null;
            Context.Array = null;
            txtOriginalArray.Text = "";
            txtSortedArray.Text = "";
            txtStatistics.Text = "";
            ClearStepByStep();
            _useDetailedOutput = true;
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (_currentArray == null || _currentArray.Length == 0)
            {
                MessageBox.Show("Сначала загрузите или сгенерируйте массив", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ClearStepByStep();
            SortingStats.Reset();
            _buffer.Clear();
            _lineCount = 0;

            if (rbBubbleSort.IsChecked == true)
            {
                _context = new Context(new BubbleSort());
                AppendStep("Выбран метод: Пузырьковая сортировка (Bubble Sort)\n");
            }
            else
            {
                _context = new Context(new QuickSort());
                AppendStep("Выбран метод: Быстрая сортировка (Quick Sort)\n");
            }

            Context.Array = (int[])_currentArray.Clone();

            Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

            Stopwatch stopwatch = Stopwatch.StartNew();
            _context.ExecuteAlgorithm();
            stopwatch.Stop();

            FlushBuffer();

            txtSortedArray.Text = string.Join(" ", Context.Array);

            txtStatistics.Text = $"Сравнений: {SortingStats.Comparisons}\n" +
                                 $"Перестановок: {SortingStats.Permutations}\n" +
                                 $"Время: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";

            AppendStep($"Сортировка завершена!\n");
            AppendStep($"Сравнений: {SortingStats.Comparisons}\n");
            AppendStep($"Перестановок: {SortingStats.Permutations}\n");
            AppendStep($"Время: {stopwatch.Elapsed.TotalMilliseconds:F2} мс\n");
        }

        private void DisplayOriginalArray()
        {
            if (_currentArray != null)
            {
                txtOriginalArray.Text = string.Join(" ", _currentArray);
            }
        }

        public void ClearStepByStep()
        {
            Dispatcher.Invoke(() =>
            {
                rtbStepByStep.Document.Blocks.Clear();
                _buffer.Clear();
                _lineCount = 0;
            });
        }

        public void AppendStep(string text)
        {
            if (_useDetailedOutput)
            {
                _buffer.Append(text);
                _lineCount++;

                if (_lineCount >= _bufferThreshold)
                {
                    FlushBuffer();
                }
            }
            else
            {
                _buffer.Append(text);

                if (_buffer.Length > 5000)
                {
                    FlushBuffer();
                }
            }
        }

        public void FlushBuffer()
        {
            if (_buffer.Length > 0)
            {
                string content = _buffer.ToString();
                _buffer.Clear();
                _lineCount = 0;

                Dispatcher.Invoke(() =>
                {
                    rtbStepByStep.AppendText(content);
                    rtbStepByStep.ScrollToEnd();
                });
            }
        }

        private void btnOpenAnalysis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnalysisWindow analysisWindow = new AnalysisWindow();
                analysisWindow.Owner = this;
                analysisWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия окна анализа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}