package myPackage;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.intellij.lang.Language;
import myPackage.Packages.Package;
import myPackage.Packages.TokensData;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.channels.Channels;
import java.nio.channels.FileChannel;
import java.nio.channels.SeekableByteChannel;
import java.nio.file.NoSuchFileException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardOpenOption;
import java.util.ArrayList;
import java.util.LinkedHashMap;

public class Bridge {

    private BufferedReader reader;
    private BufferedWriter writer;
    private SeekableByteChannel channel;
    private Socket socket;

    public ObjectMapper mapper;

    private Language language;

    private boolean isWaitingResponse;

    public void Begin(Language language){

        this.language = language;

        mapper = new ObjectMapper();

        try
        {
            socket = new Socket("127.0.0.1", 4784);
            System.out.println("Client connected!");

            reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            writer = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));

            ReadTokens();
        }
        catch (IOException e) {
            e.printStackTrace();
        }

//        Path pipePath = Paths.get("\\\\.\\pipe\\AstraLanguageServer");
//        try
//        {
//            channel = FileChannel.open(pipePath, StandardOpenOption.READ, StandardOpenOption.WRITE);
//            reader = new BufferedReader(new InputStreamReader(Channels.newInputStream(channel)));
//            writer = new BufferedWriter(new OutputStreamWriter(Channels.newOutputStream(channel)));
//
//            Runtime.getRuntime().addShutdownHook(new Thread(() -> {
//                System.out.println("Shutdown hook received");
//                try {
//                    close();
//                } catch (IOException e) {
//                    e.printStackTrace();
//                }
//            }));
//
//
//            ReadTokens();
//        }
//        catch (NoSuchFileException e)
//        {
//            System.err.println("\nNoSuchFileException: Astra language server is not launched\n");
//        }
//        catch (IOException e) {
//            e.printStackTrace();
//        }
    }

//    public String SendAndWait(String command) {
//        try {
//            Send(command);
//            return ReadMessage();
//        } catch (IOException e) {
//            e.printStackTrace();
//            return null;
//        }
//    }

    public void Send(Package pack)
    {
        try
        {
            while (isWaitingResponse){
                System.out.println("Holding package sending...");
                Thread.sleep(10);
            }

            String json = mapper.writeValueAsString(pack);
            Send(json);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }
    public Package SendAndRead(Package pack)
    {
        try
        {
            long a = System.nanoTime();

            while (isWaitingResponse){
                System.out.println("Holding package sending...");
                Thread.sleep(10);
            }

            isWaitingResponse = true;

            String json = mapper.writeValueAsString(pack);
            Send(json);
            Package response = Read();

            System.out.println("Responded in " + ((System.nanoTime() - a) / 1000000) + " ms");

            isWaitingResponse = false;
            return response;

        }
        catch (Exception e) {
            e.printStackTrace();
            return null;
        }
        finally {
            isWaitingResponse = false;
        }
    }
    public Package Read() throws IOException {
        String message = ReadMessage();
        Package pack = mapper.readerFor(Package.class).readValue(message);
        return pack;
    }

    private void Send(String message){
        try {
            System.out.println("... sending ... " + message);

            writer.write(message.length() + "\n");
            writer.write(message);
            writer.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private String ReadMessage() throws IOException {

        System.out.println("... reading ...");

        String line = reader.readLine();

        int messageLength = Integer.parseInt(line);

        char[] chars = new char[messageLength];
        for (int i = 0; i < messageLength; i++) {
            chars[i] = (char) reader.read();
        }

        System.out.println("> " + new String(chars));

        return new String(chars);
    }

    private void close() throws IOException {
        System.out.println("Closing language server connection...");

        if (writer != null) {
            Send("CLOSE");
            writer.close();
        }
        if (reader != null) reader.close();
        if (channel != null) channel.close();
    }

    private void ReadTokens() throws IOException
    {
        Package pack = Read();

        TokensData data = mapper.convertValue(pack.data, new TypeReference<TokensData>() {});
//        var data = (LinkedHashMap<String, ArrayList<String>>)pack.data;

        for (String name : data.tokenNames)
        {
            DynamicToken token = new DynamicToken(name, this.language);

            DynTypes.tokenByName.put(name, token);
        }
    }
}
