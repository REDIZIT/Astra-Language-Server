package myPackage;

import com.intellij.lang.ASTNode;
import com.intellij.lang.LightPsiParser;
import com.intellij.lang.PsiBuilder;
import com.intellij.lang.PsiParser;
import com.intellij.psi.tree.IElementType;
import myPackage.Packages.LogEntry;
import org.jetbrains.annotations.NotNull;

public class EmptyParser implements PsiParser, LightPsiParser {

    @Override
    public @NotNull ASTNode parse(@NotNull IElementType root, @NotNull PsiBuilder builder) {
        parseLight(root, builder);
        return builder.getTreeBuilt();
    }

    @Override
    public void parseLight(IElementType root, PsiBuilder builder) {
        PsiBuilder.Marker marker = builder.mark();

        while (builder.eof() == false)
        {
            builder.advanceLexer();
        }

        marker.done(root);
    }
}
