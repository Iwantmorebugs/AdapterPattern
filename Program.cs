using System;
using System.Collections.Generic;
using System.Linq;
using ExternalLibrary;
using Newtonsoft.Json;

namespace Adapter
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Storing three Person records to the database.\r\n");
            Console.ReadKey();

            //1.Store three Person instances in the db

            IPersonDatabase personDatabase = new PersonDatabaseAdapter(new ExternalDatabaseInterface());
            personDatabase.WritePerson(new PersonRecord {Id = 1, Name = "Boubakeur", Age = 15, Hobby = "Trains"});
            personDatabase.WritePerson(new PersonRecord {Id = 2, Name = "Fatiha", Age = 28 ,Hobby = "Space"} );
            personDatabase.WritePerson(new PersonRecord {Id = 3, Name = "Salma", Age = 50 ,Hobby  ="Documentaries"});

            Console.ReadKey();
            Console.WriteLine("\r\nDemo: Reading the Person records from the db \r\n");
            Console.ReadKey();

            //2. Read people one at a time;
            personDatabase.ReadPerson(1);
            personDatabase.ReadPerson(2);
            personDatabase.ReadPerson(3);

            Console.ReadKey();
            Console.WriteLine("\r\nDemo: Reading the Person Records by age\r\n");
            Console.ReadKey();

            foreach (var personRecord in personDatabase.PeopleByAge)
                Console.WriteLine($"PersonRecord:  {JsonConvert.SerializeObject(personRecord)}");

            Console.ReadKey();
        }
    }

    internal interface IPersonDatabase
    {
        ICollection<PersonRecord> PeopleByAge { get; }
        PersonRecord ReadPerson(int id);
        public void WritePerson(PersonRecord personRecord);
    }

    internal class PersonDatabaseAdapter : IPersonDatabase
    {
        private readonly ExternalDatabaseInterface _dbInterface;

        public PersonDatabaseAdapter(ExternalDatabaseInterface dbInterface)
        {
            _dbInterface = dbInterface;
        }

        public ICollection<PersonRecord> PeopleByAge
        {
            get
            {
                Console.WriteLine("[PersonDatabaseAdapter:PeopleByAge]");
                return _dbInterface.Records.Select(Convert).OrderBy(x => x.Age).ToList();
            }
        }

        public PersonRecord ReadPerson(int id)
        {
            var externalRecord = _dbInterface.ReadRecord(id);
            var personRecord = Convert(externalRecord);

            Console.WriteLine($"[PersonDatabaseAdapter:ReadPerson]: {JsonConvert.SerializeObject(personRecord)}");

            return personRecord;
        }

        public void WritePerson(PersonRecord personRecord)
        {
            Console.WriteLine($"[PersonDatabaseAdapter:WritePerson]: {JsonConvert.SerializeObject(personRecord)}");

            _dbInterface.CreateRecord(personRecord.Id, personRecord.Name, $"{personRecord.Age}:{personRecord.Hobby}");
        }

        private PersonRecord Convert(ExternalDatabaseRecord externalDatabaseRecord)
        {
            return new PersonRecord
            {
                Id = externalDatabaseRecord.Id,
                Age = uint.Parse(externalDatabaseRecord.Content.Split(":")[0]),
                Name = externalDatabaseRecord.Name,
                Hobby = externalDatabaseRecord.Content.Split(":")[1]
            };
        }
    }

    internal class PersonRecord
    {
        public uint Age { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Hobby { get; set; }
    }
}