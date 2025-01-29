package myPackage;

import com.intellij.openapi.editor.colors.TextAttributesKey;
import com.intellij.openapi.fileTypes.SyntaxHighlighter;
import com.intellij.openapi.options.colors.AttributesDescriptor;
import com.intellij.openapi.options.colors.ColorDescriptor;
import com.intellij.openapi.options.colors.ColorSettingsPage;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;

import javax.swing.*;
import java.util.Collection;
import java.util.HashSet;
import java.util.Map;

final class SimpleColorSettingsPage implements ColorSettingsPage {

    @Override
    public Icon getIcon() {
        return SimpleIcons.FILE;
    }

    @NotNull
    @Override
    public SyntaxHighlighter getHighlighter() {
        return new SimpleSyntaxHighlighter();
    }

    @NotNull
    @Override
    public String getDemoText() {
        return """
        # You are reading the ".properties" entry.
        ! The exclamation mark can also mark text as comments.
        website = https://en.wikipedia.org/
        language = English
        # The backslash below tells the application to continue reading
        # the value onto the next line.
        message = Welcome to \\
                  Wikipedia!
        # Add spaces to the key
        key\\ with\\ spaces = This is the value that could be looked up with the key "key with spaces".
        # Unicode
        tab : \\u0009""";
    }

    @Nullable
    @Override
    public Map<String, TextAttributesKey> getAdditionalHighlightingTagToDescriptorMap() {
        return null;
    }

    @Override
    public AttributesDescriptor @NotNull [] getAttributeDescriptors() {

        var values = SimpleSyntaxHighlighter.colorByTokenName.values().toArray();

        HashSet<TextAttributesKey> valuesSet = new HashSet<TextAttributesKey>();
        for (int i = 0; i < values.length; i++)
        {
            TextAttributesKey value = (TextAttributesKey)values[i];
            valuesSet.add(value);
        }

        AttributesDescriptor[] array = new AttributesDescriptor[valuesSet.size()];
        var valuesArray = valuesSet.toArray();

        for (int i = 0; i < valuesSet.size(); i++)
        {
            TextAttributesKey value = (TextAttributesKey)valuesArray[i];
            array[i] = new AttributesDescriptor(value.getExternalName(), value);
        }

        return array;
    }

    @Override
    public ColorDescriptor @NotNull [] getColorDescriptors() {
        return ColorDescriptor.EMPTY_ARRAY;
    }

    @NotNull
    @Override
    public String getDisplayName() {
        return "Astra";
    }

}