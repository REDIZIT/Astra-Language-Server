using LspTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using StreamJsonRpc;

public class CustomLanguageServer
{
    private Logger logger;

    public CustomLanguageServer(Logger logger)
    {
        this.logger = logger;
    }

    public void Dispose()
    {

    }

    [JsonRpcMethod("test")]
    public string SayHello(string name)
    {
        return $"Hello, {name}!";
    }

    [JsonRpcMethod(Methods.InitializeName)]
    public object Initialize(JToken arg)
    {
        logger.Information("Custom language init");

        //InitializedParams initializedParams = 
        //logger.Information(capabilities.ToString());

        return new InitializeResult()
        {
            ServerInfo = new _InitializeResults_ServerInfo()
            {
                Name = "Astra Language Server",
                Version = "1.0"
            },
            Capabilities = new ServerCapabilities()
            {
                TextDocumentSync = new TextDocumentSyncOptions()
                {
                    OpenClose = true,
                    Change = TextDocumentSyncKind.Incremental,
                    Save = new SaveOptions
                    {
                        IncludeText = true
                    }
                },
                DocumentHighlightProvider = false,
                HoverProvider = true,
                DocumentSymbolProvider = false,
                ColorProvider = false,
                SemanticTokensProvider = new SemanticTokensOptions()
                {
                    Full = true,
                    Range = false,
                    Legend = new SemanticTokensLegend()
                    {
                        tokenTypes = new string[] {
                                "class",
                                "variable",
                                "enum",
                                "comment",
                                "string",
                                "keyword",
                            },
                        tokenModifiers = new string[] {
                                "declaration",
                                "documentation",
                            }
                    }
                },
            }
        };
    }

    [JsonRpcMethod("shutdown")]
    public void Shutdown() 
    {
        logger.Information("Custom shutdown");
    }

    [JsonRpcMethod(Methods.TextDocumentDidOpenName)]
    public void DidOpen(JToken arg)
    {
        logger.Information("Did open");
    }

    [JsonRpcMethod(Methods.TextDocumentDocumentHighlightName)]
    public object[] TextDocumentDocumentHighlightName(JToken arg)
    {
        logger.Information("HIGHLIGHT NAME!!!");

        object[] result = new object[0];

        return result;
    }

    [JsonRpcMethod(Methods.TextDocumentDocumentSymbolName)]
    public object[] TextDocumentDocumentSymbolName(JToken arg)
    {
        logger.Information("Symbol name");

        return new object[0];
    }

    [JsonRpcMethod(Methods.TextDocumentHoverName)]
    public object TextDocumentHoverName(JToken arg)
    {
        logger.Information("Hover");

        return null;
    }

    [JsonRpcMethod(Methods.TextDocumentSemanticTokensFull)]
    public object TextDocumentSemanticTokensFull(JToken arg)
    {
        logger.Information("TextDocumentSemanticTokensFull");

        SemanticTokensParams p = arg.ToObject<SemanticTokensParams>();

        logger.Information(p.TextDocument.Uri);
        SemanticTokens tokens = new();

        List<uint> data = new();

        data.Add(0);
        data.Add(0);
        data.Add(5);
        data.Add(0);
        data.Add(0);

        data.Add(1);
        data.Add(0);
        data.Add(10);
        data.Add(1);
        data.Add(0);

        tokens.Data = data.ToArray();

        return tokens;
    }

    //[JsonRpcMethod("textDocument/documentColor")]
    //public object TextDocumentHoverName(JToken arg)
    //{
    //    logger.Information("Hover");

    //    return null;
    //}


    [JsonRpcMethod("exit")]
    public void Exit() => Environment.Exit(0);
}

public class TextDocumentItem
{
    public string uri;
    public string languageId;
    public int version;
    public string text;
}