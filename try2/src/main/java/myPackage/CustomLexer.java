package myPackage;

import com.intellij.lexer.FlexLexer;
import com.intellij.psi.TokenType;
import com.intellij.psi.tree.IElementType;
import com.sun.jna.Library;
import com.sun.jna.Native;
import org.intellij.sdk.language.psi.DynTypes;
import org.intellij.sdk.language.psi.SimpleTypes;

import java.io.IOException;


public class CustomLexer implements FlexLexer {

    private int lexicalState = 0;
    private int startRead = 0;
    private int markedPos = 0;
    private int currentPos = 0;
    private CharSequence buffer;

    private boolean isAtEOF;
    private boolean isAtBOL;
    private int endRead = 0;

    @Override
    public void yybegin(int state) {
        lexicalState = state;
    }

    @Override
    public int yystate() {
        return lexicalState;
    }

    @Override
    public int getTokenStart() {
        return startRead;
    }

    @Override
    public int getTokenEnd() {
        return getTokenStart() + length();
    }

    @Override
    public IElementType advance() throws IOException {
        // Запоминаем начало токена
        startRead = currentPos;

        SimpleLanguage.INSTANCE.Send("advance");

        currentPos = Integer.parseInt(SimpleLanguage.INSTANCE.ReadMessage());
        markedPos = Integer.parseInt(SimpleLanguage.INSTANCE.ReadMessage());

        String tokenName = SimpleLanguage.INSTANCE.ReadMessage();

        if (tokenName.equals("Token_EOF"))
        {
            isAtEOF = true;
            return null;
        }
        else if (tokenName.equals("Token_Space"))
        {
            return TokenType.WHITE_SPACE;
        }
        else
        {
            DynToken token = DynTypes.tokenByName.get(tokenName);
            return token;
        }
    }


    @Override
    public void reset(CharSequence buf, int start, int end, int initialState) {
        buffer = buf;
        currentPos = markedPos = startRead = start;
        isAtEOF = false;
        isAtBOL = true;
        endRead = end;
        yybegin(initialState);

        SimpleLanguage.INSTANCE.Send("reset");
        SimpleLanguage.INSTANCE.Send(buf.toString());
        SimpleLanguage.INSTANCE.Send(String.valueOf(start));
        SimpleLanguage.INSTANCE.Send(String.valueOf(end));
        SimpleLanguage.INSTANCE.Send(String.valueOf(initialState));
    }

    public final int length() {
        return markedPos - startRead;
    }
}
