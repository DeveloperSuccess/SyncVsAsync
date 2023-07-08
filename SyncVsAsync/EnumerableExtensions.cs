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

        public static async Task<IEnumerable<T>> WhenAllAsync<T>(this IEnumerable<Task<T>> source, int numberOfChunks = 0, bool inNewThreads = false)
        {
            if (numberOfChunks == 0)
            {
                if (inNewThreads == false)
                    return await Task.WhenAll(source);

                return await Task.WhenAll(source.Select(_ => Task.Run(() => _)));
            }

            int chunkSize = 1;

            if (source.Count() > numberOfChunks)
                chunkSize = (int)Math.Ceiling((double)source.Count() / numberOfChunks);

            var partitionedSource = source.Partition(chunkSize);

            if (inNewThreads == false)            
                return partitionedSource.Select(async _ => await Task.WhenAll(_)).SelectMany(_ => _.Result).ToList();

            return partitionedSource.Select(async _ => await Task.Run(async () => await Task.WhenAll(_)))
                .SelectMany(_ => _.Result).ToList();
        }
    }
}
