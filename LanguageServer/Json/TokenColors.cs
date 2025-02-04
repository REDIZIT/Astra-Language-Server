using Astra.Compilation;

public class TokenColors
{
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

    public int GetTokenType(Token token)
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

    public string[] GetNames()
    {
        return groups.Select(g => g.name).ToArray();
    }
}
