using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;
using Serilog.Core;

public class LanguageServerHost : IDisposable
{
    private ILanguageServer _server;

    private readonly LanguageServerWorkspace _workspace;

    private Logger logger;
    private LogLevel minLogLevel;

    private LanguageServerHost(Logger logger, LogLevel minLogLevel)
    {
        this.logger = logger;
        this.minLogLevel = minLogLevel;

        _workspace = new LanguageServerWorkspace();
    }

    public static async Task<LanguageServerHost> Create(
        Stream input,
        Stream output,
        Logger logger,
        LogLevel minLogLevel)
    {
        logger.Information("\n\nServer create");

        var result = new LanguageServerHost(logger, minLogLevel);

        await result.InitializeAsync(input, output);

        return result;
    }

    private async Task InitializeAsync(Stream input, Stream output)
    {
        TextDocumentSelector selector = new TextDocumentSelector(TextDocumentFilter.ForLanguage("Astra"));

        _server = await LanguageServer.From(options => options
            .WithInput(input)
            .WithOutput(output)
            .ConfigureLogging(x => x
                    .AddSerilog(logger)
                    .AddLanguageProtocolLogging()
                    .SetMinimumLevel(minLogLevel))
            .AddHandler(new TextDocumentSyncHandler(logger))
            .AddHandler(new DocumentHighlightHandler(logger, selector))
            .AddHandler(new HoverHandler(logger, selector))
            .AddHandler(new WorkspaceSymbolsHandler(logger))
            .AddHandler(new DocumentFormattingHandler(logger))
            .AddHandler(new SemanticHighlighHandler(logger, selector))
            );
    }

    public Task WaitForExit => _server.WaitForExit;

    public void Dispose()
    {
        _server.Dispose();
    }
}