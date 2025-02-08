using System.Diagnostics;
using System.Text;
using Astra.Compilation;
using LanguageServer;
using LspTypes;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using StreamJsonRpc;

public class CustomLanguageServer
{
    private Logger logger;
    private Workspace workspace;

    private TokenColors colors = new();
    private JsonRpc rpc;

    public CustomLanguageServer(Logger logger, JsonRpc rpc)
    {
        this.logger = logger;
        this.rpc = rpc;

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
                SemanticTokensProvider = new SemanticTokensOptions()
                {
                    Full = true,
                    Range = false,
                    Legend = new SemanticTokensLegend()
                    {
                        tokenTypes = colors.GetNames(),
                        tokenModifiers = new string[] {
                            "declaration",
                            "documentation",
                        }
                    }
                },
                HoverProvider = true,
                DocumentSymbolProvider = false,
                ColorProvider = false,
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

        PublishDiagnosticParams d = workspace.Analyze(p.TextDocument.Uri);
        rpc.NotifyAsync(Methods.TextDocumentPublishDiagnosticsName, d);
    }

    [JsonRpcMethod(Methods.TextDocumentDidChangeName)]
    public void DidChange(JToken arg)
    {
        DidChangeTextDocumentParams p = arg.ToObject<DidChangeTextDocumentParams>();

        workspace.ChangeFile(p);

        PublishDiagnosticParams d = workspace.Analyze(p.TextDocument.Uri);
        rpc.NotifyAsync(Methods.TextDocumentPublishDiagnosticsName, d);
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
        HoverParams param = arg.ToObject<HoverParams>();
        
        Hover result = new Hover();

        VirtualFile file = workspace.GetFile(param.TextDocument.Uri);

        
        
        Token hoveredToken = null;
        foreach (Token token in file.tokens)
        {
            if (token.line == param.Position.Line && token.linedBegin <= param.Position.Character && param.Position.Character <= token.linedBegin + (token.end - token.begin))
            {
                hoveredToken = token;
                break;
            }
        }
        if (hoveredToken == null) return null;

        
        
        Node hoveredNode = null;
        // StringBuilder b = new();
        foreach (Node node in Resolver.EnumerateAllNodes(file.nodes))
        {
            if (node.consumedTokens == null) continue;

            // b.AppendLine(node.GetType().Name + ": " + string.Join(", ", node.consumedTokens.Select(t => t.GetType().Name)));

            if (node.consumedTokens.Contains(hoveredToken))
            {
                hoveredNode = node;
                break;
            }
        }
        if (hoveredNode == null) return null;



        if (hoveredNode is Node_VariableUse use)
        {
            TypeInfo type = use.type;
            result.Contents = new HintBuilder(colors).Build(type, use.variableName).ToString();
        }
        else if (hoveredNode is Node_VariableDeclaration varDecl)
        {
            result.Contents = new HintBuilder(colors).Build(varDecl.fieldInfo.type, varDecl.fieldInfo.name).ToString();
        }
        else if (hoveredNode is Node_FieldAccess acc)
        {
            TypeInfo targetType = acc.targetType;
            
            FunctionInfo functionInfo = targetType.functions.FirstOrDefault(f => f.name == acc.targetFieldName);
            
            if (functionInfo != null)
            {
                result.Contents = new HintBuilder(colors).Build(functionInfo).ToString();
            }
        }
        else if (hoveredNode is Node_Function funcDecl)
        {
            result.Contents = new HintBuilder(colors).Build(funcDecl.functionInfo).ToString();
        }
        else
        {
            // result.Contents = hoveredNode.ToString();
        }
        
        return result;
    }
    

    [JsonRpcMethod(Methods.TextDocumentSemanticTokensFull)]
    public object TextDocumentSemanticTokensFull(JToken arg)
    {
        SemanticTokensParams p = arg.ToObject<SemanticTokensParams>();

        //logger.Information("Semantics handle: " + JsonConvert.SerializeObject(p));


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
            int tokenType = colors.GetTokenType(token);
            int tokenModifiers = 0;

            prevLine = token.line;
            prevStart = token.linedBegin;


            data.Add((uint)deltaLine);
            data.Add((uint)deltaStart);
            data.Add((uint)length);
            data.Add((uint)tokenType);
            data.Add((uint)tokenModifiers);


            //// Token_Comment can be multiline, but VS Code does not allow multilines tokenes
            //if (token is Token_Comment comment)
            //{
            //    logger.Information("Comment from " + comment.line + " to " + comment.endLine + ": " + string.Concat(comment.chars));

            //    //int first

            //    // Duplicate comment some lines
            //    for (int i = 0; i < comment.endLine - comment.line - 2; i++)
            //    {
            //        logger.Information(i.ToString());

            //        AppendComment(data, tokenType);
            //    }

            //    prevLine = token.endLine;
            //}
        }

        tokens.Data = data.ToArray();

        return tokens;
    }

    private void AppendComment(List<uint> data, int tokenType)
    {
        data.Add(1);
        data.Add(0);
        data.Add(5);
        data.Add((uint)tokenType);
        data.Add(0);
    }

    //[JsonRpcMethod(Methods.TextDocumentPublishDiagnosticsName)]
    //public object TextDocumentPublishDiagnosticsName(JToken arg)
    //{
    //    logger.Information("Publish diagnostics");

    //    return null;
    //}



    //[JsonRpcMethod("textDocument/documentColor")]
    //public object TextDocumentHoverName(JToken arg)
    //{
    //    logger.Information("Hover");

    //    return null;
    //}


    [JsonRpcMethod("exit")]
    public void Exit() => Environment.Exit(0);
}