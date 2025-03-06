namespace P3tr0viCh.Utils
{
    public class DefaultInstance<T> where T : new()
    {
        private static readonly T defaultInstance = new T();
        public static T Default => defaultInstance;
    }
}