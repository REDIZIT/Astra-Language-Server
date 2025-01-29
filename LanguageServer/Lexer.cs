using Astra.Compilation;

public class Lexer
{
    public List<char> chars = new();

    public int currentPos;
    public int startRead;
    public int endRead;
    public int markedPos;
    public int lexicalState;

    private static Dictionary<string, Type> tokenTypeBySingleWord = new Dictionary<string, Type>()
    {
        { "if", typeof(Token_If) },
        { "else", typeof(Token_Else) },
        { "while", typeof(Token_While) },
        { "for", typeof(Token_For) },
        { "fn", typeof(Token_Fn) },
        { "return", typeof(Token_Return) },
        { "class", typeof(Token_Class) },
        { "new", typeof(Token_New) },
    };
    private static Dictionary<string, Type> tokenTypeBySingleChar = new Dictionary<string, Type>()
    {
        { "(", typeof(Token_BracketOpen) },
        { ")", typeof(Token_BracketClose) },
        { "=", typeof(Token_Assign) },
        { "{", typeof(Token_BlockOpen) },
        { "}", typeof(Token_BlockClose) },
        { ";", typeof(Token_Terminator) },
        { ":", typeof(Token_Colon) },
        { ",", typeof(Token_Comma) },
        { ".", typeof(Token_Dot) },
        { "[", typeof(Token_SquareBracketOpen) },
        { "]", typeof(Token_SquareBracketClose) },
    };

    public void Reset(List<char> chars, int start, int end, int initialState)
    {
        this.chars = chars;
        currentPos = markedPos = startRead = start;
        endRead = end;

        lexicalState = initialState;

        Console.WriteLine($"Lexer got {chars.Count} chars, {start}:{end} with state {initialState}");
    }

    public Token Advance()
    {
        if (currentPos >= endRead)
        {
            return new Token_EOF();
        }

        // Save word start pos
        startRead = currentPos;

        
        Token token = AdvanceInternal();
        markedPos = currentPos; // Save word end pos

        if (token == null)
        {
            return new Token_Bad();
        }
        else
        {
            return token;
        }
    }

    private Token AdvanceInternal()
    {
        char startChar = chars[currentPos];
        currentPos++;

        if (startChar == ' ' || startChar == '\t')
        {
            return new Token_Space();
        }

        if (tokenTypeBySingleChar.TryGetValue(startChar.ToString(), out Type singleCharTokenType))
        {
            return (Token)Activator.CreateInstance(singleCharTokenType);
        }
        

        if (char.IsDigit(startChar))
        {
            //
            // Iterate digits for numbers
            //
            while (currentPos < endRead)
            {
                if (char.IsDigit(chars[currentPos]) == false)
                {
                    string word = string.Concat(chars[startRead..currentPos]);

                    if (int.TryParse(word, out int value))
                    {
                        return new Token_Constant();
                    }

                    break;
                }

                currentPos++;
            }
        }
        else
        {
            //
            // Iterate chars for tokens
            //
            string word = "";

            while (currentPos < endRead)
            {
                word = string.Concat(chars[startRead..currentPos]);



                // Do before tokenTypeBySingleWord due to it has '=' too
                if (startChar == '=' || startChar == '!')
                {
                    if (currentPos + 1 < endRead)
                    {
                        char secondChar = chars[currentPos];
                        if (secondChar == '=' && Token_Equality.TryMatch(word + secondChar, out var seq))
                        {
                            currentPos++;
                            return seq;
                        }
                    }
                }



                if (tokenTypeBySingleWord.TryGetValue(word, out Type tokenType))
                {
                    return (Token)Activator.CreateInstance(tokenType);
                }

                if (Token_Equality.TryMatch(word, out var eq)) return eq;

                if (Token_AddSub.TryMatch(word, out var term)) return term;
                if (Token_Factor.TryMatch(word, out var fact)) return fact;
                if (Token_Unary.TryMatch(word, out var un)) return un;

                if (Token_Comprassion.TryMatch(word, out var cmp)) return cmp;



                if (char.IsLetterOrDigit(chars[currentPos]) == false)
                {
                    break;
                }

                currentPos++;
            }

            if (Token_Visibility.TryMatch(word, out Token_Visibility tokenVisibility)) return tokenVisibility;
            return new Token_Identifier();
        }

        return null;
    }
}