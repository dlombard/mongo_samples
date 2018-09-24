import com.mongodb.ConnectionString;
import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import com.mongodb.*;
import org.bson.Document;
import java.util.Arrays;

public class App {

    public static void main (String [] args) throws InterruptedException {
        App app = new App();
        String url = System.getenv("MONGO_URI");
        MongoClient mongoClient = MongoClients.create(new ConnectionString(url));

        MongoDatabase myDatabase = mongoClient.getDatabase("test");
        MongoCollection collection = myDatabase.getCollection("longWrites");
        Block<Document> printBlock = new Block<Document>() {
            @Override
            public void apply(final Document document) {
                System.out.println(document.toJson());
            }
        };
        collection.find().limit(20).forEach(printBlock);

        Document explain =  myDatabase.runCommand(new Document("aggregate",  "longWrites").append("cursor", new Document()).append("hint", "type_1_name_1").append("pipeline", Arrays.asList(new Document("$match", new Document("type", "MongoDB") ))));
        System.out.println(explain.toJson());
        System.out.println("WRITES COMPLETED");


    }

}
