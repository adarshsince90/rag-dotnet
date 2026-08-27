using System.Text;
using System.Text.Json;
using Spectre.Console;

var httpClient = new HttpClient();

var conversationId =
    Guid.NewGuid().ToString();

AnsiConsole.Write(
    new FigletText("RagDemo")
        .Centered()
        .Color(Color.Cyan));

AnsiConsole.MarkupLine(
    "[grey]Conversational RAG Chatbot[/]");

AnsiConsole.WriteLine();

AnsiConsole.MarkupLine(
    $"[green]Conversation:[/] {conversationId}");

AnsiConsole.WriteLine();

AnsiConsole.MarkupLine(
    "[grey]Commands:[/]");

AnsiConsole.MarkupLine(
    "[green]/new[/]   Start a new conversation");

AnsiConsole.MarkupLine(
    "[green]/exit[/]  Exit application");

AnsiConsole.WriteLine();

while (true)
{
    Console.ForegroundColor =
        ConsoleColor.Green;

    var question =
        AnsiConsole.Ask<string>(
            "[green]You:[/]");

    if (string.IsNullOrWhiteSpace(question))
    {
        continue;
    }

    if (question.Equals(
        "/exit",
        StringComparison.OrdinalIgnoreCase))
        {
             AnsiConsole.MarkupLine(
            "\n[yellow]Shutting down...[/]");
            return;
        }

    if (question.Equals(
        "/new",
        StringComparison.OrdinalIgnoreCase))
    {
        conversationId =
            Guid.NewGuid().ToString();

        AnsiConsole.MarkupLine(
            $"[yellow]New Conversation:[/] {conversationId}");

        AnsiConsole.WriteLine();

        continue;
    }

    Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;

        AnsiConsole.MarkupLine(
            "\n[yellow]Shutting down...[/]");

        Environment.Exit(0);
    };

    Console.WriteLine();

    Console.ForegroundColor =
        ConsoleColor.Yellow;

    Console.WriteLine(
        "Retrieving and generating answer...");

    Console.ResetColor();

    Console.WriteLine();

    try
    {
        await StreamResponseAsync(
            httpClient,
            conversationId,
            question);
    }
    catch (Exception ex)
    {
        RenderError(ex);
    }
    Console.WriteLine();
}

static async Task StreamResponseAsync(
    HttpClient httpClient,
    string conversationId,
    string question)
{
    var payload =
        JsonSerializer.Serialize(
            new
            {
                conversationId,
                question
            });

    using var request =
        new HttpRequestMessage(
            HttpMethod.Post,
            "http://localhost:5000/conversation/stream");

    request.Content =
        new StringContent(
            payload,
            Encoding.UTF8,
            "application/json");

    using var response =
        await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
    {
        var error =
            await response.Content
                .ReadAsStringAsync();

        throw new InvalidOperationException(
            $"API Error ({(int)response.StatusCode}): {error}");
    }

    await using var stream =
        await response.Content.ReadAsStreamAsync();

    using var reader =
        new StreamReader(stream);

    string? currentEvent = null;

    var spinner = new Spinner();

    spinner.Start();

    var firstTokenReceived = false;

    RetrievalMetrics? retrieval = null;

    GenerationDiagnostics? completion = null;
    string? line;
    while ((line = await reader.ReadLineAsync()) is not null)
    //while(!reader.EndOfStream)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (line.StartsWith("event:"))
        {
            currentEvent =
                line["event:".Length..]
                    .Trim();

            continue;
        }

        if (!line.StartsWith("data:"))
        {
            continue;
        }

        var data =
            line["data:".Length..]
                .Trim();

        switch (currentEvent)
        {
            case "retrieval":

                try
                {
                    retrieval =
                        JsonSerializer.Deserialize<
                            RetrievalMetrics>(
                                data);
                }
                catch
                {
                    // Ignore malformed event
                }

                break;

            case "completed":

                try
                {
                    completion =
                    JsonSerializer.Deserialize<
                        GenerationDiagnostics>(
                            data);
                }
                catch
                {
                    // Ignore malformed event
                }

                break;

            case "token":

                if (!firstTokenReceived)
                {
                    firstTokenReceived = true;

                    await spinner.StopAsync();

                    AnsiConsole.WriteLine();

                    AnsiConsole.MarkupLine(
                        "[cyan]Assistant[/]");

                    AnsiConsole.MarkupLine(
                        "[cyan]------------------------------------------------[/]");

                    AnsiConsole.WriteLine();
                }

                var token =
                    JsonSerializer.Deserialize<string>(
                        data);

                if (!string.IsNullOrEmpty(token))
                {
                    AnsiConsole.Write(
                        new Markup(
                            Markup.Escape(token)));
                }

                break;
        }
    }

    await spinner.StopAsync();

    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine();

    AnsiConsole.Write(
    new Rule("[yellow]Diagnostics[/]"));

    AnsiConsole.WriteLine();

    if (retrieval is not null)
    {
        RenderRetrievalTable(
            retrieval);
    }

    if (completion is not null)
    {
        RenderGenerationTable(
            completion);
    }

    AnsiConsole.WriteLine();
}

static void RenderRetrievalTable(
    RetrievalMetrics retrieval)
{
    var table = new Table();

    table.Border(
        TableBorder.Rounded);

    table.Title(
        "[yellow]Retrieval[/]");

    table.AddColumn("Metric");

    table.AddColumn("Value");

    table.AddRow(
        "Time",
        $"{retrieval.RetrievalMs} ms");

    table.AddRow(
        "Chunks",
        retrieval.ReturnedChunks
            .ToString());

    table.AddRow(
        "Highest Score",
        retrieval.HighestScore
            .ToString("F3"));

    table.AddRow(
        "Average Score",
        retrieval.AverageScore
            .ToString("F3"));

    AnsiConsole.Write(table);

    var sources =
        retrieval.Sources.Any()
        ? string.Join(
            Environment.NewLine,
            retrieval.Sources
                .Select(
                    x => $"• {x}"))
        : "<none>";

    AnsiConsole.Write(
        new Panel(sources)
        {
            Header =
                new PanelHeader(
                    "Sources")
        });
}

static void RenderGenerationTable(
    GenerationDiagnostics generation)
{
    var table = new Table();

    table.Border(
        TableBorder.Rounded);

    table.Title(
        "[cyan]Generation[/]");

    table.AddColumn("Metric");

    table.AddColumn("Value");

    table.AddRow(
        "Generation Time",
        $"{generation.GenerationMs} ms");

    table.AddRow(
        "Total Time",
        $"{generation.TotalMs} ms");

    table.AddRow(
        "Characters",
        generation.AnswerCharacters
            .ToString());

    AnsiConsole.Write(table);
}

static void RenderError(
    Exception exception)
{
    var message =
        exception switch
        {
            HttpRequestException =>
                """
Unable to connect to RagDemo API.

Please verify:

 • RagDemo.Api is running
 • API URL is correct
 • Network connectivity is available
""",

            TaskCanceledException =>
                """
The request timed out.

Possible causes:

 • Ollama generation is taking too long
 • API is unavailable
""",

            _ =>
                exception.Message
        };

    AnsiConsole.Write(
        new Panel(message)
        {
            Header =
                new PanelHeader(
                    "[red]Error[/]")
        }
        .Border(BoxBorder.Rounded)
        .BorderStyle(
            new Style(Color.Red)));
}