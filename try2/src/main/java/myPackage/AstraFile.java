package myPackage;

import com.intellij.extapi.psi.PsiFileBase;
import com.intellij.openapi.fileTypes.FileType;
import com.intellij.psi.FileViewProvider;
import org.jetbrains.annotations.NotNull;

public class AstraFile extends PsiFileBase {

    public AstraFile(@NotNull FileViewProvider viewProvider) {
        super(viewProvider, AstraLanguage.INSTANCE);
    }

    @NotNull
    @Override
    public FileType getFileType() {
        return AstraFileType.INSTANCE;
    }

    @Override
    public String toString() {
        return "Astra File";
    }

}