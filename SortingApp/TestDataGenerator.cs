using System;
using System.Diagnostics;
using System.IO;

namespace SortingApp
{
    public class TestDataGenerator
    {
        public static void RunPerformanceTest(int size)
        {
            Random rand = new Random();
            int[] originalArray = new int[size];
            for (int i = 0; i < size; i++)
            {
                originalArray[i] = rand.Next(1, 10000);
            }

            string results = $"Тестирование для массива из {size} элементов:\n";
            results += "=".PadRight(50, '=') + "\n";

            int[] bubbleArray = (int[])originalArray.Clone();
            SortingStats.Reset();
            Stopwatch sw = Stopwatch.StartNew();
            new BubbleSort().Algorithm(bubbleArray);
            sw.Stop();
            results += $"Пузырьковая сортировка:\n";
            results += $"  Сравнений: {SortingStats.Comparisons}\n";
            results += $"  Перестановок: {SortingStats.Permutations}\n";
            results += $"  Время: {sw.ElapsedMilliseconds} мс\n\n";

            int[] quickArray = (int[])originalArray.Clone();
            SortingStats.Reset();
            sw.Restart();
            new QuickSort().Algorithm(quickArray);
            sw.Stop();
            results += $"Быстрая сортировка:\n";
            results += $"Сравнений: {SortingStats.Comparisons}\n";
            results += $"Перестановок: {SortingStats.Permutations}\n";
            results += $"Время: {sw.ElapsedMilliseconds} мс\n\n";

            File.WriteAllText($"test_result_{size}.txt", results);
        }
    }
}