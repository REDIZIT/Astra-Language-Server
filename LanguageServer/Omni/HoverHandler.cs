using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Core;

public class HoverHandler : IHoverHandler
{
    private Logger logger;
    private TextDocumentSelector selector;

    public HoverHandler(Logger logger, TextDocumentSelector selector)
    {
        this.logger = logger;
        this.selector = selector;
    }

    public HoverRegistrationOptions GetRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities)
    {
        logger.Information("Hover get options");
        return new HoverRegistrationOptions()
        {
            DocumentSelector = selector
        };
    }

    public async Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        logger.Information("Hover HANDLE!");
        return new Hover()
        {
            Contents = new MarkedStringsOrMarkupContent(new MarkupContent { Kind = MarkupKind.Markdown, Value = "This is my md text" }),
        };
    }
}