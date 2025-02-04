using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Serilog.Core;

public class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
{
    //private readonly LanguageServerWorkspace _workspace;
    private readonly WorkspaceSymbolRegistrationOptions _registrationOptions;

    private Logger logger;

    public WorkspaceSymbolsHandler(Logger logger)
    {
        this.logger = logger;
        //_workspace = workspace;
        _registrationOptions = new WorkspaceSymbolRegistrationOptions();
    }

    public WorkspaceSymbolRegistrationOptions GetRegistrationOptions(WorkspaceSymbolCapability capability, ClientCapabilities clientCapabilities)
    {
        logger.Information("Symbol GetRegistrationOptions");

        return _registrationOptions;
    }

    public async Task<Container<WorkspaceSymbol>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken)
    {
        logger.Information("Symbol Handle");

        //var symbols = ImmutableArray.CreateBuilder<SymbolInformation>();
        List<WorkspaceSymbol> symbols = new();

        symbols.Add(new WorkspaceSymbol()
        {
            Location = new Location() { Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 1, 1) },
            ContainerName = "container name",
            Kind = SymbolKind.Variable,
            Name = "name",
        });

        return new Container<WorkspaceSymbol>(symbols);
    }

    //public async Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken token)
    //{
    //    var searchService = _workspace.Services.GetService<INavigateToSearchService>();

    //    var symbols = ImmutableArray.CreateBuilder<SymbolInformation>();

    //    foreach (var document in _workspace.CurrentDocuments.Documents)
    //    {
    //        await Helpers.FindSymbolsInDocument(searchService, document, request.Query, token, symbols);
    //    }

    //    return new Container<SymbolInformation>(symbols);
    //}

    //WorkspaceSymbolRegistrationOptions IRegistration<WorkspaceSymbolRegistrationOptions>.GetRegistrationOptions()
    //{
    //    return _registrationOptions;
    //}

    //void ICapability<WorkspaceSymbolCapability>.SetCapability(WorkspaceSymbolCapability capability) { }
}