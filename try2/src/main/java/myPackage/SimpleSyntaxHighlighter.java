package myPackage;

import com.intellij.lexer.Lexer;
import com.intellij.openapi.editor.DefaultLanguageHighlighterColors;
import com.intellij.openapi.editor.HighlighterColors;
import com.intellij.openapi.editor.colors.TextAttributesKey;
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase;
import com.intellij.psi.TokenType;
import com.intellij.psi.tree.IElementType;
import org.intellij.sdk.language.psi.DynTypes;
import org.intellij.sdk.language.psi.SimpleTypes;
import org.jetbrains.annotations.NotNull;

import java.util.HashMap;

import static com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey;

public class SimpleSyntaxHighlighter extends SyntaxHighlighterBase {

    public static final TextAttributesKey IDENTIFIER = createTextAttributesKey("IDENTIFIER");
    public static final TextAttributesKey NUMBER = createTextAttributesKey("NUMBER");
    public static final TextAttributesKey COMMENT = createTextAttributesKey("COMMENT");
    public static final TextAttributesKey KEYWORD = createTextAttributesKey("KEYWORD");
    public static final TextAttributesKey CONTROL_FLOW = createTextAttributesKey("CONTROL_FLOW");
    public static final TextAttributesKey BAD_CHARACTER = HighlighterColors.BAD_CHARACTER;


    private static final TextAttributesKey[] BAD_CHAR_KEYS = new TextAttributesKey[]{BAD_CHARACTER};

    public static HashMap<String, TextAttributesKey> colorByTokenName = new HashMap<String, TextAttributesKey>()
    {{
        put("Token_Identifier", IDENTIFIER);
        put("Token_Bad", BAD_CHARACTER);

        put("Token_If", CONTROL_FLOW);
        put("Token_Return", CONTROL_FLOW);
    }};

    @NotNull
    @Override
    public Lexer getHighlightingLexer() {
        return new SimpleLexerAdapter();
    }

    @Override
    public TextAttributesKey @NotNull [] getTokenHighlights(IElementType tokenType) {

        if (colorByTokenName.containsKey(tokenType.getDebugName()))
        {
            TextAttributesKey key = colorByTokenName.get(tokenType.getDebugName());
            return new TextAttributesKey[]{key};
        }
        else
        {
            return BAD_CHAR_KEYS;
        }
    }

}