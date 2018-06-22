using System;
using System.Collections.Generic;  
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KryptonDotNet;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data.Entity;
using Effort;
using System.ComponentModel.DataAnnotations;

namespace KryptonDotNetTests
{
    public class TestDB : DbContext
    {
        public TestDB(DbConnection conn):base(conn, false) { }
        public DbSet<TestModel> Models { get; set; }
    }

    public class TestModel
    {
        [Key]
        public string Name { get; set; }
        public string Class { get; set; }
        public DateTime Created { get; set; }
    }

    [TestClass]
    public class FilteredResultTests
    {

        [ClassInitialize]
        public static void SetupTests(TestContext context)
        {
            using (var testDb = new TestDB(DbConnectionFactory.CreatePersistent("Test")))
            {
                testDb.Models.Add(new TestModel()
                {
                    Name = "John",
                    Class = "Ranger",
                    Created = DateTime.UtcNow
                });

                testDb.Models.Add(new KryptonDotNetTests.TestModel()
                {
                    Name = "Sam",
                    Class = "Warrior",
                    Created = DateTime.UtcNow.AddMonths(-1)
                });

                testDb.Models.Add(new KryptonDotNetTests.TestModel()
                {
                    Name = "Sally",
                    Class = "Warrior",
                    Created = DateTime.UtcNow.AddMonths(-6)
                });

                testDb.Models.Add(new KryptonDotNetTests.TestModel()
                {
                    Name = "Josh",
                    Class = "Mage",
                    Created = DateTime.UtcNow.AddMonths(-5)
                });

                testDb.Models.Add(new KryptonDotNetTests.TestModel()
                {
                    Name = "Alyssa",
                    Class = "Mage",
                    Created = DateTime.UtcNow.AddMonths(-4)
                });
                testDb.SaveChanges();
            }

        }

        IQueryable<TestModel> linqToObjects = new List<TestModel>()
        {
            new TestModel()
            {
                Name = "John",
                Class = "Ranger",
                Created = DateTime.UtcNow
            },

            new TestModel()
            {
                Name = "Sam",
                Class = "Warrior",
                Created = DateTime.UtcNow.AddMonths(-1)
            },

            new TestModel()
            {
                Name = "Sally",
                Class = "Warrior",
                Created = DateTime.UtcNow.AddMonths(-6)
            },

            new TestModel()
            {
                Name = "Josh",
                Class = "Mage",
                Created = DateTime.UtcNow.AddMonths(-5)
            },

            new TestModel()
            {
                Name = "Alyssa",
                Class = "Mage",
                Created = DateTime.UtcNow.AddMonths(-4)
            }
        }.AsQueryable();


        [TestMethod]
        public void FilteredResultCreatedBeforeTest()
        {
            using (var testDb = new TestDB(DbConnectionFactory.CreatePersistent("Test")))
            {
                var today = DateTime.UtcNow.Ticks;
                var items = FilteredResultHelpers.ProcessFilters<TestModel>(linqToObjects, new JObject() { { "Created_before", today } });

                Assert.AreEqual(5, items.Count());
            }
        }

        [TestMethod]
        public void FilteredResultCreatedAfterTest()
        {
            using (var testDb = new TestDB(DbConnectionFactory.CreatePersistent("Test")))
            {
                var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3).Ticks;
                var items = FilteredResultHelpers.ProcessFilters<TestModel>(linqToObjects, new JObject() { { "Created_after", threeMonthsAgo } });

                Assert.AreEqual(2, items.Count());
            }
        }

        [TestMethod]
        public void FilteredResultCreatedBetweenTest()
        {

            using (var testDb = new TestDB(DbConnectionFactory.CreatePersistent("Test")))
            {

                var oneMonthAgo = DateTime.UtcNow.AddMonths(-1).Ticks;
                var fiveMonthsAgo = DateTime.UtcNow.AddMonths(-5).AddDays(-1).Ticks;

                var items = FilteredResultHelpers.ProcessFilters<TestModel>(linqToObjects, new JObject() {
                { "Created_after", fiveMonthsAgo },
                { "Created_before", oneMonthAgo } });

                Assert.AreEqual(3, items.Count());
            }
        }




    }
}
