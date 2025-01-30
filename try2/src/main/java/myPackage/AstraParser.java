package myPackage;

import com.fasterxml.jackson.core.type.TypeReference;
import com.intellij.lang.ASTNode;
import com.intellij.lang.LightPsiParser;
import com.intellij.lang.PsiBuilder;
import com.intellij.lang.PsiParser;
import com.intellij.psi.tree.IElementType;
import myPackage.Packages.*;
import myPackage.Packages.Package;
import org.jetbrains.annotations.NotNull;


public class AstraParser implements PsiParser, LightPsiParser {

    @Override
    public @NotNull ASTNode parse(@NotNull IElementType root, @NotNull PsiBuilder builder) {
        parseLight(root, builder);
        return builder.getTreeBuilt();
    }

    @Override
    public void parseLight(IElementType root, PsiBuilder builder) {

        Package pack = new Package()
        {{
            command = "parse";
            data = builder.getOriginalText();
        }};

        Package response = AstraLanguage.INSTANCE.bridge.SendAndRead(pack);

        ParserData data = AstraLanguage.INSTANCE.bridge.mapper.convertValue(response.data, new TypeReference<ParserData>() {});

        LogEntries entries = data.entries;

        PsiBuilder.Marker marker = builder.mark();

        int currentTokenIndex = 0;
        int currentMarkerIndex = 0;

        while (builder.eof() == false)
        {
            if (currentMarkerIndex < entries.markers.size())
            {
                LogEntry entry = entries.markers.get(currentMarkerIndex);

                if (currentTokenIndex >= entry.tokenBeginIndex)
                {
                    PsiBuilder.Marker subMarker = builder.mark();
                    builder.advanceLexer();
                    subMarker.error(entry.message);

                    currentMarkerIndex++;
                    currentTokenIndex++;
                    continue;
                }
            }

            builder.advanceLexer();

            currentTokenIndex++;
        }

        marker.done(root);
    }
}
