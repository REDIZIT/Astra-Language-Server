package myPackage;

import com.intellij.lexer.FlexAdapter;

public class LexerAdapter extends FlexAdapter {

    public LexerAdapter() {
        super(new AstraBridgedLexer());
    }

}