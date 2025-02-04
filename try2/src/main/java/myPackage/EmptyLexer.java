package myPackage;

import com.intellij.lexer.FlexLexer;
import com.intellij.psi.TokenType;
import com.intellij.psi.tree.IElementType;

import java.io.IOException;

public class EmptyLexer implements FlexLexer
{
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

    public final int length() {
        return markedPos - startRead;
    }

    @Override
    public IElementType advance() throws IOException {

        startRead = currentPos;



        if (currentPos >= endRead){
//            return DynTypes.tokenByName.get("Token_EOF");
            return null;
        }

        markedPos++;
        currentPos++;

        return TokenType.BAD_CHARACTER;
    }

    @Override
    public void reset(CharSequence buf, int start, int end, int initialState) {
        buffer = buf;
        currentPos = markedPos = startRead = start;
        isAtEOF = false;
        isAtBOL = true;
        endRead = end;
        yybegin(initialState);
    }
}
