package myPackage;

import com.intellij.execution.ExecutionException;
import com.intellij.execution.configurations.GeneralCommandLine;
import com.intellij.openapi.project.Project;
import com.intellij.openapi.vfs.VirtualFile;
import com.intellij.platform.lsp.api.LspServerDescriptor;
import com.intellij.platform.lsp.api.LspServerSupportProvider;
import com.intellij.platform.lsp.api.ProjectWideLspServerDescriptor;
import org.eclipse.lsp4j.ClientCapabilities;
import org.eclipse.lsp4j.DocumentHighlightCapabilities;
import org.eclipse.lsp4j.InitializeParams;
import org.eclipse.lsp4j.TextDocumentClientCapabilities;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

public class AstraLspServerSupportProvider implements LspServerSupportProvider {
    @Override
    public void fileOpened(@NotNull Project project, @NotNull VirtualFile file, @NotNull LspServerSupportProvider.LspServerStarter serverStarter) {

        System.out.println("fileOpened: " + file.getExtension());

        if (file.getExtension().equals("ac")) {
            serverStarter.ensureServerStarted(new AstraLspServerDescriptor(project, file.getPresentableName()));
        }
    }

}

class AstraLspServerDescriptor extends LspServerDescriptor {

    public AstraLspServerDescriptor(@NotNull Project project, @NotNull String presentableName) {
        super(project, presentableName);

        System.out.println("description ctor");
    }

    @Override
    public @NotNull GeneralCommandLine createCommandLine() throws ExecutionException {
        String serverExePath = "C:/Users/REDIZIT/Documents/GitHub/Astra-Rider-extension/LanguageServer/bin/Debug/net8.0/LanguageServer.exe";
        System.out.println("create cmd");
        return new GeneralCommandLine(serverExePath, "lsp");
    }

    @Override
    public boolean isSupportedFile(@NotNull VirtualFile file) {
        return file.getExtension().equals("ac");
    }

//    @Override
//    public @Nullable org.eclipse.lsp4j.InitializeParams createInitializeParams() {
//
//        InitializeParams params = super.createInitializeParams();
//
//        var caps = params.getCapabilities();
//
//        var docs = caps.getTextDocument();
//        docs.setDocumentHighlight(new DocumentHighlightCapabilities(true));
//
//
//        System.out.println("createInitializeParams");
//
//        return params;
//    }
}