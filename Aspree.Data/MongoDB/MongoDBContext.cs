using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Data.MongoDB
{
    public class MongoDBContext
    {
        MongoClient _client;
        MongoServer _server;
        public MongoDatabase _database;
        public MongoDBContext()
        {
            // Reading credentials from Web.config file   
            var MongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //CarDatabase  
            //var MongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //demouser  
            //var MongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //Pass@123  
            var MongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
            var MongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

            //// Creating credentials  
            //var credential = MongoCredential.CreateMongoCRCredential
            //                (MongoDatabaseName,
            //                 MongoUsername,
            //                 MongoPassword);

            // Creating MongoClientSettings  
            var settings = new MongoClientSettings
            {
                //Credentials = new[] { credential },
                Server = new MongoServerAddress(MongoHost, Convert.ToInt32(MongoPort)),
                //ConnectTimeout=new TimeSpan(0,0,60)
            };

            //_client = new MongoClient(new MongoClientURI("mongodb://host:27017,host2:27017/?replicaSet=rs0"));

            _client = new MongoClient(settings);
            _server = _client.GetServer();
            _database = _server.GetDatabase(MongoDatabaseName);
        }
    }
}
