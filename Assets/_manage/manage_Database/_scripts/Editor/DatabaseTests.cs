using System.Collections.Generic;
using EA4S.Database;
using EA4S.Helpers;
using NUnit.Framework;

namespace EA4S.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        public void QueryStaticDB()
        {
            var dbManager = new DatabaseManager();
            var allLetterData = dbManager.GetAllLetterData();
        }

        [Test]
        public void QueryDynamicDB()
        {
            var dbManager = new DatabaseManager();
            dbManager.LoadDatabaseForPlayer("TEST");
            List<LogInfoData> list = dbManager.FindLogInfoData(x => x.Timestamp > 1000);
        }

        [Test]
        public void InsertDynamicDB()
        {
            var dbManager = new DatabaseManager();
            dbManager.LoadDatabaseForPlayer("TEST");
            var newLogInfoData = new LogInfoData();
            newLogInfoData.AppSession = GenericHelper.GetTimestampForNow();
            newLogInfoData.Timestamp = GenericHelper.GetTimestampForNow();
            newLogInfoData.Event = InfoEvent.Book;
            newLogInfoData.AdditionalData = "test:1";
            dbManager.Insert(newLogInfoData);
        }

    }
}
