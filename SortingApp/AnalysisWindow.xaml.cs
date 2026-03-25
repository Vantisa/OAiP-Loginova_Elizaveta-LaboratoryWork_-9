using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace SortingApp
{
    public partial class AnalysisWindow : Window
    {
        private ObservableCollection<TestResult> _results;

        public AnalysisWindow()
        {
            InitializeComponent();
            _results = new ObservableCollection<TestResult>();
            dgResults.ItemsSource = _results;
        }

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false;

            try
            {
                _results.Clear();
                int[] sizes = { 10, 100, 150 };

                foreach (int size in sizes)
                {
                    Random rand = new Random();
                    int[] originalArray = new int[size];
                    for (int i = 0; i < size; i++)
                        originalArray[i] = rand.Next(1, 10000);

                    int[] bubbleArray = (int[])originalArray.Clone();
                    SortingStats.Reset();
                    Stopwatch sw = Stopwatch.StartNew();
                    new BubbleSort().Algorithm(bubbleArray);
                    sw.Stop();
                    _results.Add(new TestResult
                    {
                        Size = size,
                        Method = "Пузырьковая сортировка (Bubble Sort)",
                        Comparisons = SortingStats.Comparisons,
                        Permutations = SortingStats.Permutations,
                        TimeMs = sw.ElapsedMilliseconds
                    });

                    int[] quickArray = (int[])originalArray.Clone();
                    SortingStats.Reset();
                    sw.Restart();
                    new QuickSort().Algorithm(quickArray);
                    sw.Stop();
                    _results.Add(new TestResult
                    {
                        Size = size,
                        Method = "Быстрая сортировка (Quick Sort)",
                        Comparisons = SortingStats.Comparisons,
                        Permutations = SortingStats.Permutations,
                        TimeMs = sw.ElapsedMilliseconds
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при тестировании: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (btn != null) btn.IsEnabled = true;
            }
        }

        private void btnClearResults_Click(object sender, RoutedEventArgs e)
        {
            _results.Clear();
        }
    }

    public class TestResult
    {
        public int Size { get; set; }
        public string Method { get; set; }
        public int Comparisons { get; set; }
        public int Permutations { get; set; }
        public long TimeMs { get; set; }
    }
}