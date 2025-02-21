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
            color = "dcdcdc",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Identifier)
            }
        },
        new ColorTokenGroup()
        {
            name = "type-name",
            color = "4ec9b0",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_TypeName)
            }
        },
        new ColorTokenGroup()
        {
            name = "keyword",
            color = "569cd6",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_New),
                typeof(Token_Visibility),
                typeof(Token_Class),
                typeof(Token_Static),
                typeof(Token_As),
            }
        },
        new ColorTokenGroup()
        {
            name = "keyword-control",
            color = "d8a0df",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Return),
                typeof(Token_If),
                typeof(Token_Else),
                typeof(Token_For),
                typeof(Token_While),
                typeof(Token_Try),
                typeof(Token_Catch),
                typeof(Token_Throw),
            }
        },
        new ColorTokenGroup()
        {
            name = "punctuation",
            color = "b4b4b4",
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
                typeof(Token_BitOperator),
                typeof(Token_Unary),
            }
        },
        new ColorTokenGroup()
        {
            name = "number",
            color = "b5cea8",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Constant),
            }
        },
        new ColorTokenGroup()
        {
            name = "string",
            color = "d69d85",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_String),
                typeof(Token_Char),
            }
        },
        new ColorTokenGroup()
        {
            name = "comment",
            color = "57a64a",
            compilerTokenTypes = new HashSet<Type>()
            {
                typeof(Token_Comment),
            }
        },
        new ColorTokenGroup()
        {
            name = "function-name",
            color = "dcdcaa",
            compilerTokenTypes = new()
        }
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
    
    public ColorTokenGroup TryGetGroup(string name)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            ColorTokenGroup group = groups[i];

            if (group.name == name)
            {
                return group;
            }
        }

        return null;
    }


    public string[] GetNames()
    {
        return groups.Select(g => g.name).ToArray();
    }
}