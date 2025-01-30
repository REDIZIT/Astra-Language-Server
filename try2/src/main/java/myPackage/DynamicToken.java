package myPackage;

import com.intellij.lang.Language;
import com.intellij.psi.tree.IElementType;
import org.jetbrains.annotations.NonNls;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

public class DynamicToken extends IElementType {
    public DynamicToken(@NonNls @NotNull String debugName, @Nullable Language language) {
        super(debugName, language);
    }
}
