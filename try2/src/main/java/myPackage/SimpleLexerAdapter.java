package myPackage;

import com.intellij.lexer.FlexAdapter;

public class SimpleLexerAdapter extends FlexAdapter {

    public SimpleLexerAdapter() {
        super(new CustomLexer());
    }

}