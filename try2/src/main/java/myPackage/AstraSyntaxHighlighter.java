package myPackage;

import com.intellij.lexer.Lexer;
import com.intellij.openapi.editor.HighlighterColors;
import com.intellij.openapi.editor.colors.TextAttributesKey;
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase;
import com.intellij.psi.tree.IElementType;
import org.jetbrains.annotations.NotNull;

import java.util.HashMap;

import static com.intellij.openapi.editor.colors.TextAttributesKey.EMPTY_ARRAY;
import static com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey;

public class AstraSyntaxHighlighter extends SyntaxHighlighterBase {

    public static final TextAttributesKey IDENTIFIER = createTextAttributesKey("IDENTIFIER");
    public static final TextAttributesKey NUMBER = createTextAttributesKey("NUMBER");
    public static final TextAttributesKey COMMENT = createTextAttributesKey("COMMENT");
    public static final TextAttributesKey KEYWORD = createTextAttributesKey("KEYWORD");
    public static final TextAttributesKey CONTROL_FLOW = createTextAttributesKey("CONTROL_FLOW");
    public static final TextAttributesKey PUNCTUATION = createTextAttributesKey("PUNCTUATION");
    public static final TextAttributesKey BAD_CHARACTER = HighlighterColors.BAD_CHARACTER;


    private static final TextAttributesKey[] BAD_CHAR_KEYS = new TextAttributesKey[]{BAD_CHARACTER};

    public static HashMap<String, TextAttributesKey> colorByTokenName = new HashMap<String, TextAttributesKey>()
    {{
        put("Token_Identifier", IDENTIFIER);
        put("Token_Bad", BAD_CHARACTER);

        put("Token_If", CONTROL_FLOW);
        put("Token_Else", CONTROL_FLOW);
        put("Token_Return", CONTROL_FLOW);
        put("Token_For", CONTROL_FLOW);
        put("Token_While", CONTROL_FLOW);

        put("Token_New", KEYWORD);
        put("Token_Visibility", KEYWORD);
        put("Token_Class", KEYWORD);

        put("Token_BracketOpen", PUNCTUATION);
        put("Token_BracketClose", PUNCTUATION);
        put("Token_BlockOpen", PUNCTUATION);
        put("Token_BlockClose", PUNCTUATION);
        put("Token_SquareBracketOpen", PUNCTUATION);
        put("Token_SquareBracketClose", PUNCTUATION);
        put("Token_Assign", PUNCTUATION);
        put("Token_Terminator", PUNCTUATION);
        put("Token_Colon", PUNCTUATION);
        put("Token_Comma", PUNCTUATION);
        put("Token_Dot", PUNCTUATION);

        put("Token_Operator", PUNCTUATION);
        put("Token_Equality", PUNCTUATION);
        put("Token_Comprassion", PUNCTUATION);
        put("Token_AddSub", PUNCTUATION);
        put("Token_Factor", PUNCTUATION);
        put("Token_Unary", PUNCTUATION);

        put("Token_Constant", NUMBER);
    }};

    @NotNull
    @Override
    public Lexer getHighlightingLexer() {
        return new LexerAdapter();
    }

    @Override
    public TextAttributesKey @NotNull [] getTokenHighlights(IElementType tokenType) {

        String name = tokenType.getDebugName();

        if (name.equals("WHITE_SPACE"))
        {
            return EMPTY_ARRAY;
        }
        else if (colorByTokenName.containsKey(name))
        {
            TextAttributesKey key = colorByTokenName.get(name);
            return new TextAttributesKey[]{key};
        }
        else
        {
            return BAD_CHAR_KEYS;
        }
    }

}