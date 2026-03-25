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
            if (left >= right)
                return;

            if (_detailedOutput)
            {
                MainWindow.Instance?.AppendStep($"\nСортировка подмассива [{left}, {right}]\n");
            }

            int pivotIndex = (left + right) / 2;
            int pivot = array[pivotIndex];

            if (_detailedOutput)
            {
                MainWindow.Instance?.AppendStep($"Опорный элемент (индекс {pivotIndex}): {pivot}\n");

                string currentState = "Текущий подмассив: ";
                for (int i = left; i <= right; i++)
                {
                    if (i == pivotIndex)
                        currentState += $"[{array[i]}] ";
                    else
                        currentState += $"{array[i]} ";
                }
                MainWindow.Instance?.AppendStep(currentState + "\n");
            }

            int partitionIndex = Partition(array, left, right);

            if (_detailedOutput)
            {
                MainWindow.Instance?.AppendStep($"После разделения: ");
                for (int i = left; i <= right; i++)
                {
                    if (i == partitionIndex)
                        MainWindow.Instance?.AppendStep($"[{array[i]}] ");
                    else
                        MainWindow.Instance?.AppendStep($"{array[i]} ");
                }
                MainWindow.Instance?.AppendStep("\n");
            }

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
                {
                    MainWindow.Instance?.AppendStep($"  Сравниваем {array[j]} и {pivot}\n");
                }

                if (array[j] <= pivot)
                {
                    i++;
                    if (i != j)
                    {
                        SortingStats.Permutations++;

                        if (_detailedOutput)
                        {
                            MainWindow.Instance?.AppendStep($"  Перестановка {array[i]} и {array[j]}\n");
                        }

                        int temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }

            if (i + 1 != right)
            {
                SortingStats.Permutations++;

                if (_detailedOutput)
                {
                    MainWindow.Instance?.AppendStep($"  Ставим опорный элемент {pivot} на место\n");
                }

                int temp = array[i + 1];
                array[i + 1] = array[right];
                array[right] = temp;
            }

            return i + 1;
        }
    }
}