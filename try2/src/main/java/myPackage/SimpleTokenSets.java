package myPackage;

import com.intellij.psi.tree.TokenSet;
import org.intellij.sdk.language.psi.SimpleTypes;

public interface SimpleTokenSets {

    TokenSet IDENTIFIERS = TokenSet.create(SimpleTypes.KEY);

    TokenSet COMMENTS = TokenSet.create(SimpleTypes.COMMENT);

}