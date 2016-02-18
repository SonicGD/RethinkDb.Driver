using System.Linq;
using NUnit.Framework;
using RethinkDb.Driver.Linq;

namespace RethinkDb.Driver.Tests.ReQL
{
    [TestFixture]
    [Explicit]
    public class LinqTests : QueryTestFixture
    {
        [TestFixtureSetUp]
        public void BeforeRunningTestSession()
        {

        }

        [TestFixtureTearDown]
        public void AfterRunningTestSession()
        {

        }


        [SetUp]
        public void BeforeEachTest()
        {

        }

        [TearDown]
        public void AfterEachTest()

        {

        }


        [Test]
        public void Test()
        {
            var linqQueryable = R.Db("mydb").Table<Foo>("mytable", conn);

            var linqResult = linqQueryable.Where(f => f.id == "jjj").FirstOrDefault();

            var asreql = R.Db("mydb").Table("mytable").filter(f => f["id"] == "jjj");

        }
    }
}