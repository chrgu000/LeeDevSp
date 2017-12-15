using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace ConsoleMongoDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            new MongoTest().Test();

            Console.Read();
        }
    }

    class MongoTest
    {
        public async void Test()
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("admin");
            var collection = database.GetCollection<BsonDocument>("person");

            var list = await collection.Find(new BsonDocument("Name", "Account")).ToListAsync();
            foreach(var document in list)
            {
                Console.WriteLine(document["Name"]);
            }
            //await collection.InsertOneAsync(new BsonDocument("Name",))
        }
    }

    //class Account : MongoIdentity
    //{
    //    public int AccountID { get; set; }

    //    public string UserName { get; set; }

    //    public string Password { get; set; }

    //    public int Age { get; set; }

    //    public string Email { get; set; }

    //    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    //    public DateTime RegisterDate { get; set; }

    //    public Account()
    //    {

    //    }
    //}
}