namespace SortingApp
{
    public static class SortingStats
    {
        public static int Comparisons { get; set; }
        public static int Permutations { get; set; }

        public static void Reset()
        {
            Comparisons = 0;
            Permutations = 0;
        }
    }
}