using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Core;

public class DocumentFormattingHandler : IDocumentFormattingHandler
{
    private Logger logger;

    public DocumentFormattingHandler(Logger logger)
    {
        this.logger = logger;
    }

    public DocumentFormattingRegistrationOptions GetRegistrationOptions(DocumentFormattingCapability capability, ClientCapabilities clientCapabilities)
    {
        logger.Information("Formatting get options");
        return new DocumentFormattingRegistrationOptions()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage("Astra")
        };
    }

    public Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken)
    {
        logger.Information("Formatting HANDLE!");
        throw new NotImplementedException();
    }
}