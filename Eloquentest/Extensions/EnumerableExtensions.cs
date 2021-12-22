namespace ToolBX.Eloquentest.Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            var list = collection.ToList();
            var random = new Random().Next(list.Count);
            return list[random];
        }

        public static int GetRandomIndex<T>(this ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            return new Random().Next(collection.Count);
        }
    }
}