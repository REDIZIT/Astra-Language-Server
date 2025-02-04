using Astra.Compilation;
using LspTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using StreamJsonRpc;

public class ColorTokenGroup
{
    public string name;
    public HashSet<Type> compilerTokenTypes;
}
public class CustomLanguageServer
{
    private Logger logger;
    private Workspace workspace;

    private ColorTokenGroup[] groups = new ColorTokenGroup[]
    {
        new ColorTokenGroup()
        {
            name = "invalid",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Bad)
            }
        },
        new ColorTokenGroup()
        {
            name = "identifier",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Identifier)
            }
        },
        new ColorTokenGroup()
        {
            name = "typename",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_TypeName)
            }
        },
        new ColorTokenGroup()
        {
            name = "keyword",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_New),
                typeof(Token_Visibility),
                typeof(Token_Class),
            }
        },
        new ColorTokenGroup()
        {
            name = "keyword.control",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Return),
                typeof(Token_If),
                typeof(Token_Else),
                typeof(Token_For),
                typeof(Token_While),
            }
        },
        new ColorTokenGroup()
        {
            name = "punctuation",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_BracketOpen),
                typeof(Token_BracketClose),
                typeof(Token_BlockOpen),
                typeof(Token_BlockClose),
                typeof(Token_SquareBracketOpen),
                typeof(Token_SquareBracketClose),
                typeof(Token_Assign),
                typeof(Token_Terminator),
                typeof(Token_Colon),
                typeof(Token_Comma),
                typeof(Token_Dot),

                typeof(Token_Operator),
                typeof(Token_Equality),
                typeof(Token_Comprassion),
                typeof(Token_AddSub),
                typeof(Token_Factor),
                typeof(Token_Unary),
            }
        },
        new ColorTokenGroup()
        {
            name = "number",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Constant),
            }
        },
        new ColorTokenGroup()
        {
            name = "string",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_String),
                typeof(Token_Char),
            }
        },
        new ColorTokenGroup()
        {
            name = "comment",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Comment),
            }
        },
    };

    public CustomLanguageServer(Logger logger)
    {
        this.logger = logger;

        workspace = new Workspace(logger);
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
                HoverProvider = true,
                DocumentSymbolProvider = false,
                ColorProvider = false,
                SemanticTokensProvider = new SemanticTokensOptions()
                {
                    Full = true,
                    Range = false,
                    Legend = new SemanticTokensLegend()
                    {
                        tokenTypes = groups.Select(g => g.name).ToArray(),
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
        DidOpenTextDocumentParams p = arg.ToObject<DidOpenTextDocumentParams>();

        workspace.LoadFile(p.TextDocument);
    }

    [JsonRpcMethod(Methods.TextDocumentDidChangeName)]
    public void DidChange(JToken arg)
    {
        DidChangeTextDocumentParams p = arg.ToObject<DidChangeTextDocumentParams>();

        workspace.ChangeFile(p);
    }

    //[JsonRpcMethod(Methods.TextDocumentDocumentHighlightName)]
    //public object[] TextDocumentDocumentHighlightName(JToken arg)
    //{
    //    object[] result = new object[0];

    //    return result;
    //}

    [JsonRpcMethod(Methods.TextDocumentDocumentSymbolName)]
    public object[] TextDocumentDocumentSymbolName(JToken arg)
    {
        logger.Information("Symbol name");

        return new object[0];
    }

    [JsonRpcMethod(Methods.TextDocumentHoverName)]
    public object TextDocumentHoverName(JToken arg)
    {
        //logger.Information("Hover");

        return null;
    }

    [JsonRpcMethod(Methods.TextDocumentSemanticTokensFull)]
    public object TextDocumentSemanticTokensFull(JToken arg)
    {
        SemanticTokensParams p = arg.ToObject<SemanticTokensParams>();

        logger.Information("Semantics handle: " + JsonConvert.SerializeObject(p));


        logger.Information(p.TextDocument.Uri);
       
        var astraTokens = workspace.GetTokens(p.TextDocument.Uri);

        SemanticTokens tokens = new();

        List<uint> data = new();

        int prevLine = 0;
        int prevStart = 0;

        foreach (Token token in astraTokens)
        {
            int deltaLine = token.line - prevLine;
            int deltaStart = token.linedBegin - prevStart;
            int length = token.end - token.begin;
            int tokenType = GetTokenType(token);
            int tokenModifiers = 0;

            prevLine = token.line;
            prevStart = token.linedBegin;


            data.Add((uint)deltaLine);
            data.Add((uint)deltaStart);
            data.Add((uint)length);
            data.Add((uint)tokenType);
            data.Add((uint)tokenModifiers);
        }

        tokens.Data = data.ToArray();

        return tokens;
    }

    private int GetTokenType(Token token)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            ColorTokenGroup group = groups[i];

            if (group.compilerTokenTypes.Contains(token.GetType()))
            {
                return i;
            }
        }

        return 0;
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