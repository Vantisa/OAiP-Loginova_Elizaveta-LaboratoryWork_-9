namespace SortingApp
{
    public class Context
    {
        public IStrategy ContextStrategy { get; set; }
        public static int[] Array { get; set; }

        public Context(IStrategy strategy)
        {
            ContextStrategy = strategy;
        }

        public void ExecuteAlgorithm()
        {
            ContextStrategy.Algorithm(Array);
        }
    }
}