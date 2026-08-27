public sealed class Spinner : IDisposable
{
    private readonly CancellationTokenSource
        _cts = new();

    private Task? _spinnerTask;

    public void Start(
        string message = "Thinking")
    {
        _spinnerTask =
            Task.Run(async () =>
            {
                var frames =
                    new[] { '|', '/', '-', '\\' };

                var index = 0;

                while (!_cts.Token.IsCancellationRequested)
                {
                    Console.Write(
                        $"\r{message}... {frames[index++ % frames.Length]}");

                    await Task.Delay(
                        150,
                        _cts.Token);
                }
            });
    }

    public async Task StopAsync()
    {
        _cts.Cancel();

        if (_spinnerTask is not null)
        {
            try
            {
                await _spinnerTask;
            }
            catch
            {
            }
        }

        Console.Write(
            "\r                                                  \r");
    }
    public void Dispose()
    {
        _cts.Cancel();
    }
}