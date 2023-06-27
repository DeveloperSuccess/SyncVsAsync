namespace SyncVsAsync
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize).ToList();
                source = source.Skip(chunkSize).ToList();
            }
        }

        public static async Task<IEnumerable<T>> WhenAllAsync<T>(this IEnumerable<Task<T>> source, int concurrently = 0, bool inNewThreads = false)
        {
            if (concurrently == 0)
            {
                if (inNewThreads == false)
                    return await Task.WhenAll(source);

                return await Task.WhenAll(source.Select(_ => Task.Run(() => _)));
            }
        }
    }
}
