using System;

namespace SortingApp
{
    public class BubbleSort : IStrategy
    {
        public void Algorithm(int[] array)
        {
            SortingStats.Reset();

            bool detailedOutput = array.Length <= 30;

            for (int i = 0; i < array.Length - 1; i++)
            {
                if (detailedOutput)
                {
                    MainWindow.Instance?.AppendStep($"Проход {i + 1}\n");
                }
                else if (i % 10 == 0 || i == array.Length - 2)
                {
                    MainWindow.Instance?.AppendStep($"Проход {i + 1}/{array.Length - 1}\n");
                }

                bool swapped = false;

                for (int j = 0; j < array.Length - i - 1; j++)
                {
                    SortingStats.Comparisons++;

                    if (detailedOutput)
                    {
                        string stepLine = "Сравнение: ";
                        for (int k = 0; k < array.Length; k++)
                        {
                            if (k == j || k == j + 1)
                                stepLine += $"[{array[k]}] ";
                            else
                                stepLine += $"{array[k]} ";
                        }
                        MainWindow.Instance?.AppendStep(stepLine + "\n");
                        MainWindow.Instance?.AppendStep($"Сравниваем {array[j]} и {array[j + 1]}\n");
                    }

                    if (array[j] > array[j + 1])
                    {
                        SortingStats.Permutations++;
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        swapped = true;

                        if (detailedOutput)
                        {
                            MainWindow.Instance?.AppendStep($"Перестановка: {array[j]} и {array[j + 1]}\n");

                            string afterSwap = "Состояние: ";
                            foreach (var item in array)
                                afterSwap += $"{item} ";
                            MainWindow.Instance?.AppendStep(afterSwap + "\n");
                        }
                    }

                    if (detailedOutput)
                    {
                        MainWindow.Instance?.AppendStep("\n");
                    }
                }

                if (!swapped)
                {
                    MainWindow.Instance?.AppendStep("За этот проход перестановок не было - массив отсортирован!\n");
                    break;
                }
            }

            MainWindow.Instance?.FlushBuffer();
        }
    }
}