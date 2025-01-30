﻿using Astra.Compilation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

public static class Program
{
    private static StreamReader reader;
    private static StreamWriter writer;

    private static List<char> buffer = new();

    private static Lexer lexer = new();

    private static long timer;

    public static void Main()
    {
        bool isSingleRun = true;

        while (true)
        {
            try
            {
                //using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("AstraLanguageServer", PipeDirection.InOut))
                //{
                //    Console.WriteLine("Client awaiting...");
                //    pipeServer.WaitForConnection();
                //    Console.WriteLine("Client connected.");

                //    using (reader = new StreamReader(pipeServer))
                //    using (writer = new StreamWriter(pipeServer) { AutoFlush = true })
                //    {
                //        WriteTokens();

                //        while (true)
                //        {
                //            string input = ReadMessage();
                //            if (input == "CLOSE") break;

                //            OnReceived(input);
                //        }
                //    }
                //}
                using (TcpListener listener = new TcpListener(4784))
                {
                    listener.Start();
                    Console.WriteLine("TCP Client awaiting...");
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("TCP Client connected.");

                    NetworkStream stream = client.GetStream();
                    writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
                    reader = new StreamReader(stream, Encoding.ASCII);

                    WriteTokens();

                    while (true)
                    {
                        string input = ReadMessage();
                        if (input == "CLOSE") break;

                        OnReceived(input);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"Restart connection due to exception: {err.Message}");
            }

            if (isSingleRun)
            {
                break;
            }
        }
    }

    private static void OnReceived(string input)
    {
        //Console.WriteLine("Received: " + input);
        Package request = JsonConvert.DeserializeObject<Package>(input);
        if (request.command == "reset")
        {
            var obj = (JObject)request.data;
            ResetData data = obj.ToObject<ResetData>();

            List<char> chars = data.chars.ToList();

            timer = 0;

            int start = data.start;
            int end = data.end;
            int initialState = data.initialState;

            lexer.Reset(chars, start, end, initialState);
        }
        else if (request.command == "advance")
        {
            Stopwatch w = Stopwatch.StartNew();

            Token token = lexer.Advance();

            Package pack = new()
            {
                command = "",
                data = new AdvanceResponse()
                {
                    currentPos = lexer.currentPos,
                    markedPos = lexer.markedPos,
                    tokenName = token.GetType().Name
                }
            };

            Send(pack);

            timer += w.ElapsedMilliseconds;
            if (token is Token_EOF)
            {
                Console.WriteLine("Whole file advanced in " + timer + " ms");
            }

        }
        else if (request.command == "parse")
        {
            //List<Token> tokens = JsonConvert.DeserializeObject<List<Token>>(ReadMessage());

            //List<Node> ast = AbstractSyntaxTreeBuilder.Parse(tokens);

            //SendMessage(JsonConvert.SerializeObject(ast));
        }
        else
        {
            throw new Exception($"Unknown command: '{input}'");
        }
    }

    private static string ReadMessage()
    {
        //Console.WriteLine("... read message ...");

        string line = reader.ReadLine();
        if (line == null) throw new Exception("Message is null. Client is bad.");

        int messageLength = int.Parse(line);
        buffer.Clear();

        for (int i = 0; i < messageLength; i++)
        {
            buffer.Add((char)reader.Read());
        }

        string message = string.Concat(buffer);

        //Console.WriteLine("> " + message);

        return message;
    }

    private static void Send(Package pack)
    {
        string json = JsonConvert.SerializeObject(pack);
        SendMessage(json);
    }
    private static void SendMessage(string message)
    {
        //Console.WriteLine("... write message ... " + message);

        writer.WriteLine(message.Length);
        writer.Write(message);
    }


    private static void WriteTokens()
    {
        TokensData data = new();

        foreach (Type type in Assembly.GetAssembly(typeof(Token)).GetTypes())
        {
            if (typeof(Token).IsAssignableFrom(type) && type != typeof(Token))
            {
                data.tokenNames.Add(type.Name);
                data.typeByName.Add(type.Name, type);
            }
        }

        Package pack = new Package()
        {
            command = "tokens",
            data = data
        };

        Send(pack);
    }

    private static void WriteParse(List<Node> tokens)
    {
        List<string> names = tokens.Select(t => t.GetType().Name).ToList();

        string json = JsonConvert.SerializeObject(names);

        SendMessage(json);
    }

    public class TokensData
    {
        public List<string> tokenNames = new();

        [JsonIgnore]
        public Dictionary<string, Type> typeByName = new();
    }
}

public class Package
{
    public string command;
    public object data;
}
public class ResetData
{
    public string chars;
    public int start;
    public int end;
    public int initialState;
}
public class AdvanceResponse
{
    public int currentPos;
    public int markedPos;
    public string tokenName;
}