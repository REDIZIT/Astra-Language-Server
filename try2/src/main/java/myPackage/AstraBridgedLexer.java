package myPackage;

import com.fasterxml.jackson.core.type.TypeReference;
import com.intellij.lexer.FlexLexer;
import com.intellij.psi.TokenType;
import com.intellij.psi.tree.IElementType;
import myPackage.Packages.AdvanceResponse;
import myPackage.Packages.Package;
import myPackage.Packages.ResetData;
import myPackage.Packages.TokensData;

import java.io.IOException;


public class AstraBridgedLexer implements FlexLexer {

    private int lexicalState = 0;
    private int startRead = 0;
    private int markedPos = 0;
    private int currentPos = 0;
    private CharSequence buffer;

    private boolean isAtEOF;
    private boolean isAtBOL;
    private int endRead = 0;

    private int timer;

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
        startRead = currentPos;

        System.out.println("__ begin advance __");

        long a = System.currentTimeMillis();

        Package pack = new Package();
        pack.command = "advance";

        Package responsePack = AstraLanguage.INSTANCE.bridge.SendAndRead(pack);
        AdvanceResponse response = AstraLanguage.INSTANCE.bridge.mapper.convertValue(responsePack.data, new TypeReference<AdvanceResponse>() {});

        currentPos = response.currentPos;
        markedPos = response.markedPos;
        String tokenName = response.tokenName;

        System.out.println("__ end advance __");

        long b = System.currentTimeMillis();
        timer += b - a;

        if (tokenName.equals("Token_EOF"))
        {
            isAtEOF = true;
            System.out.println("Whole file advanced in " + timer + " ms");
            return null;
        }
        else if (tokenName.equals("Token_Space"))
        {
            return TokenType.WHITE_SPACE;
        }
        else
        {
            return DynTypes.tokenByName.get(tokenName);
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

        System.out.println("__ begin reset__");

        timer = 0;

        ResetData data = new ResetData();
        data.chars = buf.toString();
        data.start = start;
        data.end = end;
        data.initialState = initialState;

        Package pack = new Package();
        pack.command = "reset";
        pack.data = data;

        AstraLanguage.INSTANCE.bridge.Send(pack);

        System.out.println("__ end reset__");
    }

    public final int length() {
        return markedPos - startRead;
    }
}
