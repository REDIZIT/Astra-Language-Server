using Astra.Compilation;
using LspTypes;
using Serilog.Core;

public class Workspace
{
    private Logger logger;

    private Dictionary<string, VirtualFile> fileByUri = new();

    private Lexer lexer = new();

    public Workspace(Logger logger)
    {
        this.logger = logger;
    }

    public void LoadFile(TextDocumentItem doc)
    {
        logger.Information("Load file: " + doc.Uri);

        VirtualFile file = new();
        file.chars = doc.Text.ToCharArray().ToList();
        RetokenizeFile(file);

        fileByUri.Add(doc.Uri, file);
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

        logger.Information("File after changes:\n" + string.Concat(file.chars));

        RetokenizeFile(file);
    }

    public void RetokenizeFile(VirtualFile file)
    {
        file.tokens = lexer.Tokenize(file.chars, false);
    }

    public List<Token> GetTokens(string fileUri)
    {
        VirtualFile file = fileByUri[fileUri];

        return file.tokens;
    }
}
public class VirtualFile
{
    public List<char> chars;
    public List<Token> tokens;

    public int GetLineIndex(uint line)
    {
        if (line == 0) return 0;

        for (int i = 0; i < chars.Count; i++)
        {
            if (chars[i] == '\n')
            {
                line--;
                if (line == 0)
                {
                    return i + 1;
                }
            }
        }

        return -1;
    }
}