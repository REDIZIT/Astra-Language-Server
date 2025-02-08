using Astra.Compilation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using StreamJsonRpc;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

public static class Program
{
    private static StreamReader reader;
    private static StreamWriter writer;

    private static long timer;

    public static async Task Main(string[] args)
    {
        CustomLanguageServer host = null;

        string logFile = "C:\\Users\\REDIZIT\\Documents\\GitHub\\Astra-Rider-extension\\LanguageServer\\lsp.log";
        Logger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(logFile)
                .CreateLogger();

        try
        {
            logger.Information("Begin");

            var rpc = new JsonRpc(Console.OpenStandardOutput(), Console.OpenStandardInput());

            host = new CustomLanguageServer(logger, rpc);

            rpc.AddLocalRpcTarget(host);
            rpc.StartListening();

            await rpc.Completion;



            logger.Information("End");
        }
        catch (Exception err)
        {
            logger.Error(err, "Err");

            throw;
        }
        finally
        {
            if (host != null) host.Dispose();
        }       
    }
}




public class ParserData
{
    public LogEntries entries;
}