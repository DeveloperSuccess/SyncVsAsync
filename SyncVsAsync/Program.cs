using System.Diagnostics;

var upperLimitsOfRanges = new List<int>() { 100, 1000, 5000, 15000 };

var millisecondsTimeout = 100;

foreach (var upperLimitOfRange in upperLimitsOfRanges)
{
    var collection = Enumerable.Range(1, upperLimitOfRange);

    var syncExecutionTimeThread = await GetSyncExecutionTimeThread(collection, millisecondsTimeout);

    var syncExecutionTimeDelay = await GetSyncExecutionTimeDelay(collection, millisecondsTimeout);

    var asyncExecutionTime = await GetAsyncExecutionTime(collection, millisecondsTimeout);

    var asyncExecutionTimeInTasks = await GetAsyncExecutionTimeInTasks(collection, millisecondsTimeout);

    Console.WriteLine($"Iterations: {upperLimitOfRange}; " +
        $"Sync thread: {syncExecutionTimeThread} ms; " +
        $"Sync delay: {syncExecutionTimeDelay} ms; " +
        $"Async: {asyncExecutionTime} ms; " +
        $"Async in tasks: {asyncExecutionTimeInTasks} ms");
}

Console.Read();

async Task<long> GetSyncExecutionTimeThread(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => Delay(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetSyncExecutionTimeDelay(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => DelayAsync(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTime(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(async _ => await DelayAsync(millisecondsTimeout).ConfigureAwait(false)));

    executionTimeWithAsync.Stop();

    return executionTimeWithAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTimeInTasks(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

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