using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog.Core;
using System.Collections.Immutable;

public class SemanticHighlighHandler : ISemanticTokensFullHandler
{
    private Logger logger;
    private TextDocumentSelector selector;

    public SemanticHighlighHandler(Logger logger, TextDocumentSelector selector)
    {
        this.logger = logger;
        this.selector = selector;
    }

    public SemanticTokensRegistrationOptions GetRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities)
    {
        logger.Information("Semantic get options");
        return new SemanticTokensRegistrationOptions()
        {
            DocumentSelector = selector,
            Id = "Astra",
            Full = true,
            Legend = new SemanticTokensLegend()
            {
                TokenTypes = new Container<SemanticTokenType>(
                    SemanticTokenType.Class,
                    SemanticTokenType.Comment
                )
            }
        };
    }

    public async Task<SemanticTokens?> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
    {
        logger.Information("Semantic HANDLE!");

        //var tokens = new List<SemanticToken>
        //{
        //    new SemanticToken
        //    {
        //        Line = 0,
        //        Char = 0,
        //        Length = 5,  // Длина выделяемого токена
        //        Type = "keyword",  // Тип токена (например, "keyword", "variable", "function", и т.д.)
        //        Modifier = ""  // Модификаторы (например, "declaration", "readonly" и т.д.)
        //    },
        //    new SemanticToken
        //    {
        //        Line = 1,
        //        Char = 0,
        //        Length = 4,
        //        Type = "variable",
        //        Modifier = "readonly"
        //    }
        //};

        //SemanticTokens tokens = new()

        return null;
    }
}