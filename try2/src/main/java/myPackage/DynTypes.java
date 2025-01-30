package myPackage;

import java.util.HashMap;
import java.util.Map;

public interface DynTypes {
  Map<String, DynamicToken> tokenByName = new HashMap<String, DynamicToken>();
}
