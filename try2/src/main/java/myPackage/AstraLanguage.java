package myPackage;

import com.intellij.lang.Language;

public class AstraLanguage extends Language {

    public static final AstraLanguage INSTANCE = new AstraLanguage();

    public Bridge bridge;

    private AstraLanguage()
    {
        super("Astra");

//        bridge = new Bridge();
//        bridge.Begin(this);
    }
}