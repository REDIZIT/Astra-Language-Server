using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Core;

public class DocumentHighlightHandler : IDocumentHighlightHandler
{
    //private DocumentHighlightRegistrationOptions _registrationOptions;

    private TextDocumentSelector selector;

    private Logger logger;

    public DocumentHighlightHandler(Logger logger, TextDocumentSelector selector)
    {
        this.logger = logger;
        this.selector = selector;

        //_registrationOptions = new DocumentHighlightRegistrationOptions
        //{
        //    DocumentSelector = new TextDocumentSelector()
        //};
    }

    public DocumentHighlightRegistrationOptions GetRegistrationOptions(DocumentHighlightCapability capability, ClientCapabilities clientCapabilities)
    {
        logger.Information("Highlight GetRegistrationOptions");
        return new DocumentHighlightRegistrationOptions
        {
            DocumentSelector = selector
        };
    }

    public Task<DocumentHighlightContainer?> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
    {
        logger.Information("Highlight handle");
        var result = new List<DocumentHighlight>();

        // Подсветка первой строки (строка 0, с 0 до конца строки)
        var highlights = new List<DocumentHighlight>
        {
            new DocumentHighlight
            {
                Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(0, 0), new Position(0, int.MaxValue)),
                Kind = DocumentHighlightKind.Read
            }
        };

        return Task.FromResult<DocumentHighlightContainer?>(new DocumentHighlightContainer(highlights));


        //return result;
    }

    //public DocumentHighlightRegistrationOptions GetRegistrationOptions(DocumentHighlightCapability capability, ClientCapabilities clientCapabilities)
    //{
    //    return new DocumentHighlightRegistrationOptions()
    //    {
    //        DocumentSelector = new(),
    //        WorkDoneProgress = true
    //    };
    //}

    //public async Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken token)
    //{
    //    logger.Information("Handle highlight");
    //    logger.Information(request.Position.Line.ToString());
    //    //var (document, position) = _workspace.GetLogicalDocument(request);

    //    //var documentHighlightsService = _workspace.Services.GetService<IDocumentHighlightsService>();

    //    //var documentHighlightsList = await documentHighlightsService.GetDocumentHighlightsAsync(
    //    //    document, position,
    //    //    ImmutableHashSet<Document>.Empty, // TODO
    //    //    token);

    //    //var result = new List<DocumentHighlight>();

    //    //foreach (var documentHighlights in documentHighlightsList)
    //    //{
    //    //    if (documentHighlights.Document != document)
    //    //    {
    //    //        continue;
    //    //    }

    //    //    foreach (var highlightSpan in documentHighlights.HighlightSpans)
    //    //    {
    //    //        result.Add(new DocumentHighlight
    //    //        {
    //    //            Kind = highlightSpan.Kind == HighlightSpanKind.Definition
    //    //                ? DocumentHighlightKind.Write
    //    //                : DocumentHighlightKind.Read,
    //    //            Range = Helpers.ToRange(document.SourceText, highlightSpan.TextSpan)
    //    //        });
    //    //    }
    //    //}

    //    //return result;

    //    var result = new List<DocumentHighlight>();

    //    result.Add(new DocumentHighlight()
    //    {
    //        Kind = DocumentHighlightKind.Write,
    //        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 1, 1)
    //    });

    //    return result;
    //}
}