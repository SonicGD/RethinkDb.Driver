using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RethinkDb.Driver.Linq;
using RethinkDb.Driver.Model;
using RethinkDb.Driver.Net;

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


        [Test]
        public void Test()
        {
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


            var reql = R.Db(mydb).Table(mytable)
                .filter(f => f["FirstName"] == "Brian")
                .RunResult<List<Person>>(conn);

            reql.Count.Should().Be(1);

            //var linq = R.Db("mydb").Table<Person>("mytable", conn);
            var linq = R.Db(mydb).Table(mytable).AsQueryable<Person>(conn);

            var result = linq.Where(f => f.FirstName == "Brian").ToList();

            

        }
    }
}