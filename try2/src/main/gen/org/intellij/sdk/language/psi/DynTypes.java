package org.intellij.sdk.language.psi;

import myPackage.DynToken;

import java.util.HashMap;
import java.util.Map;

public interface DynTypes {
  Map<String, DynToken> tokenByName = new HashMap<String, DynToken>();
}
