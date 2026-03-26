using System;

namespace SortingApp
{
    public class QuickSort : IStrategy
    {
        private bool _detailedOutput;

        public void Algorithm(int[] array)
        {
            SortingStats.Reset();
            _detailedOutput = array.Length <= 30;
            QuickSortRecursive(array, 0, array.Length - 1);
            MainWindow.Instance?.FlushBuffer();
        }

        private void QuickSortRecursive(int[] array, int left, int right)
        {
            if (left >= right) return;

            if (_detailedOutput)
                MainWindow.Instance?.AppendStep($"\nСортировка подмассива [{left}, {right}]\n");

            int pivotIndex = (left + right) / 2;
            int pivot = array[pivotIndex];

            if (_detailedOutput)
            {
                MainWindow.Instance?.AppendStep($"Опорный элемент (индекс {pivotIndex}): {pivot}\n");
                MainWindow.Instance?.AppendStep($"Текущий подмассив: {FormatArray(array, left, right, pivotIndex)}\n");
            }

            int partitionIndex = Partition(array, left, right);

            if (_detailedOutput)
                MainWindow.Instance?.AppendStep($"После разделения: {FormatArray(array, left, right, partitionIndex)}\n");

            QuickSortRecursive(array, left, partitionIndex - 1);
            QuickSortRecursive(array, partitionIndex + 1, right);
        }

        private int Partition(int[] array, int left, int right)
        {
            int pivot = array[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                SortingStats.Comparisons++;

                if (_detailedOutput)
                    MainWindow.Instance?.AppendStep($"  Сравниваем {array[j]} и {pivot}\n");

                if (array[j] <= pivot)
                {
                    i++;
                    if (i != j)
                        Swap(array, i, j, $"  Перестановка {array[i]} и {array[j]}\n");
                }
            }

            if (i + 1 != right)
                Swap(array, i + 1, right, $"  Ставим опорный элемент {pivot} на место\n");

            return i + 1;
        }

        private void Swap(int[] array, int i, int j, string message)
        {
            SortingStats.Permutations++;

            if (_detailedOutput)
                MainWindow.Instance?.AppendStep(message);

            (array[i], array[j]) = (array[j], array[i]);
        }

        private string FormatArray(int[] array, int left, int right, int highlightIndex)
        {
            var result = new System.Text.StringBuilder();
            for (int i = left; i <= right; i++)
            {
                if (i == highlightIndex)
                    result.Append($"[{array[i]}] ");
                else
                    result.Append($"{array[i]} ");
            }
            return result.ToString().TrimEnd();
        }
    }
}