using System.Diagnostics;

var upperLimitsOfRanges = new List<int>() { 100, 1000, 5000, 15000 };

var millisecondsTimeout = 100;

foreach (var upperLimitOfRange in upperLimitsOfRanges)
{
    var collection = Enumerable.Range(1, upperLimitOfRange);

    var syncExecutionTime = await GetSyncExecutionTime(collection, millisecondsTimeout);

    var asyncExecutionTime = await GetAsyncExecutionTime(collection, millisecondsTimeout);

    var difference = syncExecutionTime - asyncExecutionTime;

    Console.WriteLine($"Range limit: {upperLimitOfRange}; " +
        $"Sync execution time: {syncExecutionTime} ms; " +
        $"Async execution time: {asyncExecutionTime} ms; " +
        $"Acceleration async execution : {difference} ms; ");
}

Console.Read();

async Task<long> GetSyncExecutionTime(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => Delay(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTime(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    //await Task.WhenAll(collection.Select(async _ => await DelayAsync(millisecondsTimeout).ConfigureAwait(false)));

    await Task.WhenAll(collection.Select(_ => Task.Run(async () => await DelayAsync(millisecondsTimeout).ConfigureAwait(false))));

    executionTimeWithAsync.Stop();

    return executionTimeWithAsync.ElapsedMilliseconds;
}

bool Delay(int millisecondsTimeout)
{
    Thread.Sleep(millisecondsTimeout);
    return true;
}

async Task<bool> DelayAsync(int millisecondsTimeout)
{
    await Task.Delay(millisecondsTimeout);
    return true;
}