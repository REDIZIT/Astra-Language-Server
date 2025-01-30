using Astra.Compilation;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Reflection;

public static class Program
{
    private static StreamReader reader;
    private static StreamWriter writer;

    private static List<char> buffer = new();

    private static Lexer lexer = new();

    public static void Main()
    {
        while (true)
        {
            try
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("AstraLanguageServer", PipeDirection.InOut))
                {
                    Console.WriteLine("Client awaiting...");
                    pipeServer.WaitForConnection();
                    Console.WriteLine("Client connected.");

                    using (reader = new StreamReader(pipeServer))
                    using (writer = new StreamWriter(pipeServer) { AutoFlush = true })
                    {
                        WriteTokens();

                        while (true)
                        {
                            string input = ReadMessage();
                            if (input == "CLOSE") break;

                            OnReceived(input);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"Restart connection due to exception: {err.Message}");
            }
        }
    }

    private static void OnReceived(string input)
    {
        //Console.WriteLine("Received: " + input);

        if (input == "reset")
        {
            List<char> chars = ReadMessage().ToList();

            int start = int.Parse(ReadMessage());
            int end = int.Parse(ReadMessage());
            int initialState = int.Parse(ReadMessage());

            lexer.Reset(chars, start, end, initialState);
        }
        else if (input == "advance")
        {
            Token token = lexer.Advance();

            SendMessage(lexer.currentPos.ToString());
            SendMessage(lexer.markedPos.ToString());
            SendMessage(token.GetType().Name);
        }
        else if (input == "parse")
        {
            List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(ReadMessage());

            List<Node> ast = AbstractSyntaxTreeBuilder.Parse(tokens);

            SendMessage(JsonConvert.SerializeObject(ast));
        }
        else
        {
            throw new Exception($"Unknown command: '{input}'");
        }
    }

    private static string ReadMessage()
    {
        int messageLength = int.Parse(reader.ReadLine());
        buffer.Clear();

        for (int i = 0; i < messageLength; i++)
        {
            buffer.Add((char)reader.Read());
        }

        string message = string.Concat(buffer);

        //Console.WriteLine("> " + message);

        return message;
    }
    private static void SendMessage(string message)
    {
        writer.WriteLine(message.Length);
        writer.Write(message);
    }


    private static void WriteTokens()
    {
        TokensPackage tokensPackage = new();

        foreach (Type type in Assembly.GetAssembly(typeof(Token)).GetTypes())
        {
            if (typeof(Token).IsAssignableFrom(type) && type != typeof(Token))
            {
                tokensPackage.tokenNames.Add(type.Name);
                tokensPackage.typeByName.Add(type.Name, type);
            }
        }

        string json = JsonConvert.SerializeObject(tokensPackage);
        Console.WriteLine("Send tokens: " + json);

        SendMessage(json);
    }

    private static void WriteParse(List<Node> tokens)
    {


        List<string> names = tokens.Select(t => t.GetType().Name).ToList();

        string json = JsonConvert.SerializeObject(names);

        SendMessage(json);
    }

    public class TokensPackage
    {
        public List<string> tokenNames = new();

        [JsonIgnore]
        public Dictionary<string, Type> typeByName = new();
    }
}
