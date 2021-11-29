using System;
using System.IO;
using Example;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharp_Test
{
    [TestClass]
    public class UnitTest1
    {
        static string OutDir = "../../../ProtoData/";

        [TestMethod]
        public void TestPerson3()
        {
            string fileName;
            fileName = "person3.bytes";
            WritePerson(fileName, new Person3()
            {
                Id = 1,
                Name = "张三",
                Address = "xxx city",
            });

            Person3 data = ReadPerson3(fileName);
            Assert.AreEqual(1, data.Id);
            Assert.AreEqual("张三", data.Name);
            Assert.AreEqual(string.Empty, data.Email);
            Assert.AreEqual("xxx city", data.Address);
        }

        static void WritePerson(string fileName, Person3 data)
        {
            using (var fs = File.Create(OutDir + fileName))
            using (var cos = new Google.Protobuf.CodedOutputStream(fs))
            {
                data.WriteTo(cos);
            }
        }

        static Person3 ReadPerson3(string fileName)
        {
            Person3 data;
            using (var fs = File.OpenRead(OutDir + fileName))
            {
                data = Person3.Parser.ParseFrom(fs);
            }
            return data;
        }
        [TestMethod]
        public void TestPerson2()
        {
            string fileName;
            fileName = "person2.bytes";
            WritePerson(fileName, new Person2()
            {
                Id = 1,
                Name = "张三",
                Address = "xxx city",
            });

            Person2 data = ReadPerson2(fileName);

            Assert.AreEqual(1, data.Id);
            Assert.AreEqual("张三", data.Name);
            Assert.AreEqual(string.Empty, data.Email);
            Assert.AreEqual("xxx city", data.Address);
        }

        static void WritePerson(string fileName, Person2 data)
        {
            using (var fs = File.Create(OutDir + fileName))
            using (var cos = new Google.Protobuf.CodedOutputStream(fs))
            {
                data.WriteTo(cos);
            }
        }

        static Person2 ReadPerson2(string fileName)
        {
            Person2 data;
            using (var fs = File.OpenRead(OutDir + fileName))
            {
                data = Person2.Parser.ParseFrom(fs);
            }
            return data;
        }
    }
}
