using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MongoTraceSource
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Configure TraceSource

            TraceSource traceSource = new TraceSource("mongodb-test", SourceLevels.Verbose);
            var listener = new TextWriterTraceListener(Console.Out)
            {
                TraceOutputOptions = TraceOptions.DateTime
            };
            traceSource.Listeners.Add(listener);



            var username = ConfigurationManager.AppSettings.Get("username");
            var pwd = ConfigurationManager.AppSettings.Get("password");


            //Servers List
            List<MongoServerAddress> _servers = new List<MongoServerAddress>();

            _servers.Add(new MongoServerAddress(ConfigurationManager.AppSettings.Get("server0")));
            _servers.Add(new MongoServerAddress(ConfigurationManager.AppSettings.Get("server1")));
            _servers.Add(new MongoServerAddress(ConfigurationManager.AppSettings.Get("server2")));



            var settings = new MongoClientSettings
            {


                Credential = MongoCredential.CreateCredential("admin", username, pwd),
                ReplicaSetName = ConfigurationManager.AppSettings.Get("replicaSetName"),
                Servers = _servers,
                UseSsl = true,
                VerifySslCertificate = false,
                ClusterConfigurator = _cb => {

                    _cb.TraceWith(traceSource);
                }

            };
            var client = new MongoClient(settings);


        }
    }
}