package myPackage;

import com.intellij.lang.ASTNode;
import com.intellij.lang.LightPsiParser;
import com.intellij.lang.PsiBuilder;
import com.intellij.lang.PsiParser;
import com.intellij.psi.tree.IElementType;
import org.jetbrains.annotations.NotNull;

public class AstraParser implements PsiParser, LightPsiParser {

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
            var tokenType = builder.getTokenType();


            PsiBuilder.Marker subMarker = builder.mark();
            if (tokenType.getDebugName().equals("Token_Return") || tokenType.getDebugName().equals("Token_Visibility")) {
                builder.advanceLexer();
                subMarker.error("Return error");
            }
            else{
                builder.advanceLexer();
                subMarker.done(tokenType);
            }


        }

        marker.done(root);
    }


}
