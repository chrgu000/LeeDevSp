using Google.Protobuf;
using Google.ProtocolBuffers.Examples.AddressBook;
using System;
using System.IO;

namespace ConsoleAppProtobuf
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Person.Builder().SetId(1).SetName("王尼玛").SetEmail("woshiwangnima@qq.com").Build().ToByteArray();
            byte[] bytes;
            Person.Builder newContact = Person.CreateBuilder();
            newContact.SetId(1).SetName("Foo").SetEmail("foo@bar");
            newContact.AddPhone(Person.Types.PhoneNumber.CreateBuilder().SetNumber("555-1212").BuildPartial());
            Person person = newContact.BuildPartial();
            newContact = null;
            using (MemoryStream ms = new MemoryStream())
            {
                person.WriteTo(new CodedOutputStream(ms));
                bytes = ms.ToArray();
            }

                Console.WriteLine("Hello World!");
        }
    }
}