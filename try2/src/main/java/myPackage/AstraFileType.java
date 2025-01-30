package myPackage;

import com.intellij.openapi.fileTypes.LanguageFileType;
import org.jetbrains.annotations.NotNull;

import javax.swing.*;

public final class AstraFileType extends LanguageFileType {

    public static final AstraFileType INSTANCE = new AstraFileType();

    private AstraFileType() {
        super(AstraLanguage.INSTANCE);
    }

    @NotNull
    @Override
    public String getName() {
        return "Astra File";
    }

    @NotNull
    @Override
    public String getDescription() {
        return "Astra language file";
    }

    @NotNull
    @Override
    public String getDefaultExtension() {
        return "ac";
    }

    @Override
    public Icon getIcon() {
        return Icons.FILE;
    }

}