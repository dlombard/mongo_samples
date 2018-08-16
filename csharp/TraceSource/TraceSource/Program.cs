using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MongoTraceSource
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            TraceSource traceSource = new TraceSource("mongodb-test", SourceLevels.Verbose);
            var listener = new TextWriterTraceListener(Console.Out)
            {
                TraceOutputOptions = TraceOptions.DateTime
            };
            traceSource.Listeners.Add(listener);

            IEnumerable<MongoServerAddress> lServers = new List<MongoServerAddress>();

            List<MongoServerAddress> _servers = new List<MongoServerAddress>();

            _servers.Add(new MongoServerAddress("cluster0-shard-00-00-mddzz.mongodb.net"));
            _servers.Add(new MongoServerAddress("cluster0-shard-00-01-mddzz.mongodb.net"));
            _servers.Add(new MongoServerAddress("cluster0-shard-00-02-mddzz.mongodb.net"));

            var settings = new MongoClientSettings
            {

                Credential = MongoCredential.CreateCredential("admin", Environment.GetEnvironmentVariable("MONGO_USER"), Environment.GetEnvironmentVariable("MONGO_PWD")),
                ReplicaSetName = "Cluster0-shard-0",
                Servers= _servers,
                UseSsl= true,
                VerifySslCertificate=false,
                ClusterConfigurator = _cb => {
                    
                    _cb.TraceWith(traceSource);
                }

            };
            var client = new MongoClient(settings);

            Console.WriteLine(client.ListDatabases().ToString());
            listener.Flush();
        }
    }
}
