using Astra.Compilation;

public class Lexer
{
    public List<char> chars = new();

    public int currentPos;
    public int startRead;
    public int endRead;
    public int markedPos;

    private string word;

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

    public void Reset(List<char> chars)
    {
        this.chars = chars;
        currentPos = 0;
        startRead = 0;
        markedPos = 0;
        endRead = chars.Count;

        Console.WriteLine("Lexer got " + chars.Count + " chars");
    }

    public Token Advance()
    {
        // Skip spaces
        while (currentPos < endRead && chars[currentPos] == ' ')
        {
            currentPos++;
        }

        if (currentPos >= endRead)
        {
            return new Token_EOF();
        }

        // Save word start pos
        startRead = currentPos;


        char currentChar = chars[currentPos];
        currentPos++;

        if (char.IsLetter(currentChar))
        {
            while (currentPos < endRead)
            {
                string word = string.Concat(chars[startRead..currentPos]);

                if (tokenTypeBySingleWord.TryGetValue(word, out Type tokenType))
                {
                    markedPos = currentPos;
                    return (Token)Activator.CreateInstance(tokenType);
                }

                if (char.IsLetterOrDigit(chars[currentPos]) == false)
                {
                    break;
                }

                currentPos++;
            }

            markedPos = currentPos; // Save word end pos
            return new Token_Identifier();
        }
        else
        {
            markedPos = currentPos;
            return new Token_Bad();
        }

        //String tokenName = SimpleLanguage.INSTANCE.SendAndWait("advance");
        //return DynTypes.tokenByName.get(tokenName);


        //if (word == "example")
        //{
        //    word = "";

        //    return new Token_Comment();
        //}
        //else
        //{
        //    word = "";
        //    return new Token_Identifier()
        //    {
        //        name = "aboba"
        //    };
        //}
    }
}