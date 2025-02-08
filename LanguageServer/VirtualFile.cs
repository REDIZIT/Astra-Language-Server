using Astra.Compilation;

public class VirtualFile
{
    public List<char> chars;
    public List<Token> tokens;
    public List<Node> nodes;
    public ResolvedModule module;

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