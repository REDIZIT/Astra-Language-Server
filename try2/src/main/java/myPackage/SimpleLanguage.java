package myPackage;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.intellij.lang.Language;
import org.intellij.sdk.language.psi.DynTypes;

import java.io.*;
import java.nio.channels.Channels;
import java.nio.channels.FileChannel;
import java.nio.channels.SeekableByteChannel;
import java.nio.file.NoSuchFileException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardOpenOption;

public class SimpleLanguage extends Language {

    public static final SimpleLanguage INSTANCE = new SimpleLanguage();

    private BufferedReader reader;
    private BufferedWriter writer;
    private SeekableByteChannel channel;

    private ObjectMapper mapper;

    private SimpleLanguage()
    {
        super("Simple");

        mapper = new ObjectMapper();

        Path pipePath = Paths.get("\\\\.\\pipe\\AstraLanguageServer");
        try
        {
            channel = FileChannel.open(pipePath, StandardOpenOption.READ, StandardOpenOption.WRITE);
            reader = new BufferedReader(new InputStreamReader(Channels.newInputStream(channel)));
            writer = new BufferedWriter(new OutputStreamWriter(Channels.newOutputStream(channel)));

            Runtime.getRuntime().addShutdownHook(new Thread(() -> {
                System.out.println("Shutdown hook received");
                try {
                    close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }));


            ReadTokens();
        }
        catch (NoSuchFileException e)
        {
            System.err.println("\nNoSuchFileException: Astra language server is not launched\n");
        }
        catch (IOException e) {
            e.printStackTrace();
        }
    }

    public String SendAndWait(String command) {
        try {
            Send(command);
            return ReadMessage();
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    public void Send(String message){
        try {
            writer.write(message.length() + "\n");
            writer.write(message);
            writer.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public String ReadMessage() throws IOException {
        int messageLength = Integer.parseInt(reader.readLine());

        char[] chars = new char[messageLength];
        for (int i = 0; i < messageLength; i++) {
            chars[i] = (char) reader.read();
        }

        return new String(chars);
    }

    public void close() throws IOException {
        System.out.println("Closing language server connection...");

        if (writer != null) {
            writer.write("EXIT\n");
            writer.flush();
            writer.close();
        }
        if (reader != null) reader.close();
        if (channel != null) channel.close();
    }

    private void ReadTokens() throws IOException
    {
        String json = ReadMessage();

        System.out.println(json);

        TokensPackage pack = mapper.readerFor(TokensPackage.class).readValue(json);

        System.out.println(pack.tokenNames[0]);

        for (String name : pack.tokenNames)
        {
            DynToken token = new DynToken(name, this);

            DynTypes.tokenByName.put(name, token);
        }
    }
}