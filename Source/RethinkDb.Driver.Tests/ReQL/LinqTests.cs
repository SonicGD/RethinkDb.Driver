using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RethinkDb.Driver.Linq;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Tests.Utils;

namespace RethinkDb.Driver.Tests.ReQL
{
    [ReqlTable("mydb", "mytable")]
    public class Person
    {
        [ReqlPrimaryKey]
        public Guid Id { get; set; }
        
        public string FirstName { get; set; }

        [ReqlIndex("lastname_ix")]
        public string LastName { get; set; }
    }


    [TestFixture]
    [Explicit]
    public class LinqTests : QueryTestFixture
    {
        string mytable = TableName;
        private string mydb = DbName;

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            ClearTable(mydb, mytable);

            var people = new[]
                {
                    new Person {FirstName = "Brian", LastName = "Chavez"},
                    new Person {FirstName = "Major", LastName = "Tom"},
                    new Person {FirstName = "Britney", LastName = "Spears"},
                    new Person {FirstName = "Flo", LastName = "Rida"}
                };


            R.Db(mydb).table(mytable).insert(people)
                .RunResult(conn)
                .AssertInserted(4);
        }


        [Test]
        public void Test()
        {
            var reql = R.Db(mydb).Table(mytable)
                .filter(f => f["FirstName"] == "Brian")
                .RunResult<List<Person>>(conn);

            reql.Count.Should().Be(1);

            //var linq = R.Db("mydb").Table<Person>("mytable", conn);
            var linq = R.Db(mydb).Table(mytable).AsQueryable<Person>(conn);

            var result = linq.Where(f => f.FirstName == "Brian").ToList();

            foreach( var r in result )
            {
                r.Dump();
            }            
        }

        [Test]
        public void linq2()
        {
            var tableOfPeople = R.Db(mydb).table(mytable);

            var peopleNamedBrian =
                from person in tableOfPeople.AsQueryable<Person>(conn)
                where person.FirstName == "Brian"
                select person;

            foreach ( var brians in peopleNamedBrian )
            {
                brians.FirstName.Should().Be("Brian");
            }

            var findBriansUsingMethods = tableOfPeople.AsQueryable<Person>(conn)
                .Where(f => f.FirstName == "Brian");

            foreach( var brians in findBriansUsingMethods )
            {
                brians.FirstName.Should().Be("Brian");
            }
        }
    }
}