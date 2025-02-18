﻿using Astra.Compilation;
using LspTypes;
using Newtonsoft.Json;
using Serilog.Core;

public class Workspace
{
    private Logger logger;

    private Dictionary<string, VirtualFile> fileByUri = new();

    private Lexer lexer = new();
    private AstraAST parser = new();

    public Workspace(Logger logger)
    {
        this.logger = logger;
    }

    public void LoadFile(TextDocumentItem doc)
    {
        logger.Information("Load file: " + doc.Uri);

        VirtualFile file = new();
        file.chars = doc.Text.ToCharArray().ToList();
        fileByUri.Add(doc.Uri, file);

        RetokenizeFile(doc.Uri);
    }

    public void ChangeFile(DidChangeTextDocumentParams p)
    {
        VirtualFile file = fileByUri[p.TextDocument.Uri];

        foreach (TextDocumentContentChangeEvent e in p.ContentChanges)
        {
            int beginIndex = file.GetLineIndex(e.Range.Start.Line) + (int)e.Range.Start.Character;
            int endIndex = file.GetLineIndex(e.Range.End.Line) + (int)e.Range.End.Character;

            file.chars.RemoveRange(beginIndex, endIndex - beginIndex);

            file.chars.InsertRange(beginIndex, e.Text);

            //logger.Information("Change: '" + e.Text + "' at " + e.Range.Start.Line + "." + e.Range.Start.Character + ":" + e.Range.End.Line + "." + e.Range.End.Character + " = " + e.RangeLength);
        }

        //logger.Information("File after changes:\n" + string.Concat(file.chars));

        RetokenizeFile(p.TextDocument.Uri);
    }

    public void RetokenizeFile(string fileUri)
    {
        VirtualFile file = fileByUri[fileUri];

        file.tokens = lexer.Tokenize(file.chars, false);
    }

    public PublishDiagnosticParams Analyze(string fileUri)
    {
        try
        {
            // logger.Information("Analyze " + fileUri);

            VirtualFile file = fileByUri[fileUri];

            ErrorLogger errorLogger = new();
            file.nodes = parser.Parse(file.tokens, errorLogger);

            try
            {
                file.module = Resolver.DiscoverModule(file.nodes);
            }
            catch (Exception err)
            {
                file.module = null;
            }
            


            PublishDiagnosticParams p = new()
            {
                Uri = fileUri,
                Diagnostics = new Diagnostic[errorLogger.entries.Count]
            };

            for (int i = 0; i < errorLogger.entries.Count; i++)
            {
                LogEntry e = errorLogger.entries[i];

                //Token beginToken = file.tokens[e.tokenBeginIndex];
                //Token endToken = file.tokens[e.tokenEndIndex];
                Token token = e.token;

                logger.Information(JsonConvert.SerializeObject(e));

                p.Diagnostics[i] = new Diagnostic()
                {
                    Message = e.message,
                    Severity = DiagnosticSeverity.Error,
                    Range = new LspTypes.Range()
                    {
                        Start = new Position((uint)token.line, (uint)token.linedBegin),
                        End = new Position((uint)token.line, (uint)token.linedBegin + (uint)(token.end - token.begin))
                    },
                    //Source = "Test source",
                    //Code = "public"
                };
            }

            // logger.Information($"Parsed {file.tokens.Count} tokens into {file.nodes.Count} nodes with {errorLogger.entries.Count} errors");

            return p;
        }
        catch (Exception err)
        {
            logger.Error(err, "Analyze error");
            throw;
        }
    }

    public List<Token> GetTokens(string fileUri)
    {
        VirtualFile file = fileByUri[fileUri];

        return file.tokens;
    }
    
    public VirtualFile GetFile(string fileUri)
    {
        return fileByUri[fileUri];
    }
}