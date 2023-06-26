using System.Diagnostics;

var upperLimitsOfRanges = new List<int>() { 100, 1000, 5000, 15000 };

var millisecondsTimeout = 100;

foreach (var upperLimitOfRange in upperLimitsOfRanges)
{
    var collection = Enumerable.Range(1, upperLimitOfRange);

    var syncExecutionTimeDelayInNewThreads = await GetSyncExecutionTimeDelayInNewThreads(collection, millisecondsTimeout);

    var asyncExecutionTimeDelay = await GetAsyncExecutionTimeDelay(collection, millisecondsTimeout);

    var asyncExecutionTimeDelayWithExpectation = await GetAsyncExecutionTimeDelayWithExpectation(collection, millisecondsTimeout);

    var asyncExecutionTimeDelayInNewThreads = await GetAsyncExecutionTimeDelayInNewThreads(collection, millisecondsTimeout);

    var asyncExecutionTimeDelayInNewThreadWithExpectation = await GetAsyncExecutionTimeDelayInNewThreadWithExpectation(collection, millisecondsTimeout);

    var syncAndAsyncExecutionTimeDelayInNewThreads = await GetSyncAndAsyncExecutionTimeDelayInNewThreads(collection, millisecondsTimeout);

    var syncAndAsyncExecutionTimeDelayInNewThreadWithExpectation = await GetSyncAndAsyncExecutionTimeDelayInNewThreadWithExpectation(collection, millisecondsTimeout);

    Console.WriteLine($"Iterations: {upperLimitOfRange}; " +
      $"1: {syncExecutionTimeDelayInNewThreads} ms; " +
      $"2: {asyncExecutionTimeDelay} ms; " +
      $"3: {asyncExecutionTimeDelayWithExpectation} ms; " +
      $"4: {asyncExecutionTimeDelayInNewThreads} ms; " +
      $"5: {asyncExecutionTimeDelayInNewThreadWithExpectation} ms; " +
      $"6: {syncAndAsyncExecutionTimeDelayInNewThreads} ms; " +
      $"7: {syncAndAsyncExecutionTimeDelayInNewThreadWithExpectation} ms");
}

Console.Read();

async Task<long> GetSyncExecutionTimeDelayInNewThreads(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => Delay(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTimeDelay(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => DelayAsync(millisecondsTimeout)));

    executionTimeWithAsync.Stop();

    return executionTimeWithAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTimeDelayWithExpectation(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(async _ => await DelayAsync(millisecondsTimeout).ConfigureAwait(false)));

    executionTimeWithAsync.Stop();

    return executionTimeWithAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTimeDelayInNewThreads(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => DelayAsync(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetAsyncExecutionTimeDelayInNewThreadWithExpectation(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(async () => await DelayAsync(millisecondsTimeout).ConfigureAwait(false))));

    executionTimeWithAsync.Stop();

    return executionTimeWithAsync.ElapsedMilliseconds;
}

async Task<long> GetSyncAndAsyncExecutionTimeDelayInNewThreads(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithoutAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(() => DelaySyncAndAsync(millisecondsTimeout))));

    executionTimeWithoutAsync.Stop();

    return executionTimeWithoutAsync.ElapsedMilliseconds;
}

async Task<long> GetSyncAndAsyncExecutionTimeDelayInNewThreadWithExpectation(IEnumerable<int> collection, int millisecondsTimeout)
{
    var executionTimeWithAsync = Stopwatch.StartNew();

    await Task.WhenAll(collection.Select(_ => Task.Run(async () => await DelaySyncAndAsync(millisecondsTimeout).ConfigureAwait(false))));

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

async Task<bool> DelaySyncAndAsync(int millisecondsTimeout)
{
    Thread.Sleep(millisecondsTimeout);
    await Task.Delay(millisecondsTimeout);
    return true;
}