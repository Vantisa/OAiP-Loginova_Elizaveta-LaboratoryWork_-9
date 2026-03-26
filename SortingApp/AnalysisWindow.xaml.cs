using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
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

                double totalSpeedRatio = 0;
                double totalComparisonsRatio = 0;
                double totalPermutationsRatio = 0;
                int testCount = sizes.Length;

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

                    var bubbleResult = new TestResult
                    {
                        Size = size,
                        Method = "Пузырьковая сортировка (Bubble Sort)",
                        Comparisons = SortingStats.Comparisons,
                        Permutations = SortingStats.Permutations,
                        TimeMs = sw.ElapsedMilliseconds
                    };
                    _results.Add(bubbleResult);

                    int[] quickArray = (int[])originalArray.Clone();
                    SortingStats.Reset();
                    sw.Restart();
                    new QuickSort().Algorithm(quickArray);
                    sw.Stop();

                    var quickResult = new TestResult
                    {
                        Size = size,
                        Method = "Быстрая сортировка (Quick Sort)",
                        Comparisons = SortingStats.Comparisons,
                        Permutations = SortingStats.Permutations,
                        TimeMs = sw.ElapsedMilliseconds
                    };
                    _results.Add(quickResult);

                    if (quickResult.TimeMs > 0)
                    {
                        totalSpeedRatio += (double)bubbleResult.TimeMs / quickResult.TimeMs;
                    }
                    totalComparisonsRatio += (double)bubbleResult.Comparisons / quickResult.Comparisons;
                    totalPermutationsRatio += (double)bubbleResult.Permutations / quickResult.Permutations;
                }

                double avgSpeedRatio = totalSpeedRatio / testCount;
                double avgComparisonsRatio = totalComparisonsRatio / testCount;
                double avgPermutationsRatio = totalPermutationsRatio / testCount;

                var comparisonInfo = new StringBuilder();
                comparisonInfo.AppendLine("Средние значения");
                comparisonInfo.AppendLine();
                comparisonInfo.AppendLine($"Быстрая сортировка быстрее в {avgSpeedRatio:F2} раз(а)");
                comparisonInfo.AppendLine($"Сравнений: QuickSort эффективнее в {avgComparisonsRatio:F2} раз(а)");
                comparisonInfo.AppendLine($"Перестановок: QuickSort эффективнее в {avgPermutationsRatio:F2} раз(а)");
                comparisonInfo.AppendLine();

                if (txtComparisonInfo != null)
                {
                    txtComparisonInfo.Text = comparisonInfo.ToString();
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
            if (txtComparisonInfo != null)
            {
                txtComparisonInfo.Text = "";
            }
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