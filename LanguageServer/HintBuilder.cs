using Astra.Compilation;

namespace LanguageServer;

public class HintBuilder
{
    private List<string> strings = new();
    private const string newLine = "  \n";

    private TokenColors colors;

    public HintBuilder(TokenColors colors)
    {
        this.colors = colors;
    }

    public string Build(TypeInfo type, string name)
    {
        Append("### ");
        Append(type.name, "typename");
        Append(" ");
        Append(name, "identifier");

        return ToString();
    }
    public string Build(FunctionInfo info)
    {
        Append("### ");
        Append(info.owner.name, "typename");
        Append(".", "punctuation");
        Append(info.name, "functionname");
                
        if (info.arguments.Count > 0)
        {
            NewLine();
            Append("params: ", "keyword.control");
                    
            bool isFirst = true;
            foreach (FieldInfo argument in info.arguments)
            {
                if (isFirst == false) Append(", ");
                isFirst = false;
                        
                Append(argument.type.name, "typename");
                Append(" ");
                Append(argument.name, "identifier");
            }
        }
                
        if (info.returns.Count > 0)
        {
            NewLine();
            Append("returns: ", "keyword.control");
                    
            bool isFirst = true;
            foreach (TypeInfo ret in info.returns)
            {
                if (isFirst == false) Append(", ");
                isFirst = false;
                        
                Append(ret.name, "typename");
                        
            }
        }

        return ToString();
    }

    public void Append(string str, string colorGroupName = null)
    {
        if (colorGroupName == null)
        {
            strings.Add(str);
        }
        else
        {
            strings.Add($"<span style='color:#{colors.TryGetGroup(colorGroupName).color};'>{str}</span>");
        }
        
    }

    public void AppendLine(string str)
    {
        Append(str);
        NewLine();
    }

    public void NewLine()
    {
        strings.Add(newLine);
    }

    public override string ToString()
    {
        string str = string.Concat(strings);
        strings.Clear();
        return str;
    }
}