namespace UnitTest2012Ultimate.Administration.Hotel
{
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary.Hotels;
using SpendManagementLibrary.Interfaces;
using UnitTest2012Ultimate.DatabaseMock;
using Utilities.DistributedCaching;

    [TestClass]
    public class HotelTest
    {
        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestCleanup]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion

        /// <summary>
        ///     Tests a hotel is cached correctly
        /// </summary>
        [TestMethod]
        public void CacheHotel()
        {
            const string hotelSQL =
                "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels";
            const string oneHotelSql =
                "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels where hotelid = @hotelid";

            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();

            MockReaderData hotelData =
                Reader.MockReaderDataFromClassData(
                    hotelSQL,
                    new List<object>
                    {
                        hotel,
                    });

            MockReaderData oneHotelData = Reader.MockReaderDataFromClassData(oneHotelSql, new List<object> {hotel});
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {hotelData, oneHotelData});

            //clear the cache of hotel with the ID of 1
            var cache = new Cache();
            cache.Delete(0, "hotel", "1");

            //Get hotel data, should be read from the DB
            Hotel hotel1Retrieved = Hotel.Get(1, dbConnection.Object);

            //the database should have been called once   
            dbConnection.Verify(d => d.GetReader(oneHotelSql, CommandType.Text), Times.Once());

            Assert.IsNotNull(hotel1Retrieved);
            Assert.AreEqual("UT Hotel", hotel1Retrieved.hotelname);

            //Get hotel data, should be read from the Cache
            Hotel hotel2Retrieved = Hotel.Get(1, dbConnection.Object);

            Assert.IsNotNull(hotel2Retrieved);
            Assert.AreEqual("UT Hotel", hotel2Retrieved.hotelname);

            //Check again that the database is only called once:    
            dbConnection.Verify(d => d.GetReader(oneHotelSql, CommandType.Text), Times.Once());
        }

        /// <summary>
        ///     Tests hotel details are retrieved from the database if the hotel details are updated.
        /// </summary>
        [TestMethod]
        public void UpdateHotelIsReflected()
        {
            const string hotelSQL =
                "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels";
            const string oneHotelSql =
                "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels where hotelid = @hotelid";

            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();

            MockReaderData hotelData =
                Reader.MockReaderDataFromClassData(
                    hotelSQL,
                    new List<object>
                    {
                        hotel,
                    });

            MockReaderData oneHotelData = Reader.MockReaderDataFromClassData(oneHotelSql, new List<object> {hotel});
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {hotelData, oneHotelData});

            //clear the cache of hotel with the ID of 1
            var cache = new Cache();
            cache.Delete(0, "hotel", "1");

            //Get hotel data, should be read from the DB
            Hotel hotel1Retrieved = Hotel.Get(1, dbConnection.Object);

            Assert.IsNotNull(hotel1Retrieved);
            Assert.AreEqual("UT Hotel", hotel1Retrieved.hotelname);

            //clear the cache of hotel with the ID of 1    
            dbConnection.Verify(d => d.GetReader(oneHotelSql, CommandType.Text), Times.Once());

            //Update hotel name 
            hotel.Update(1, "The Hilton", "25 Test Street", "", "Lincoln", "Lincolnshire", "LN5 8RG", "UK", 4,
                "01522 855444", "UTHotel@test.com", dbConnection.Object);

            Hotel hotel2Retrieved = Hotel.Get(1, dbConnection.Object);

            //Verify the database is hit again to retireive updated hotel details        
            dbConnection.Verify(c => c.GetReader(oneHotelSql, CommandType.Text), Times.Exactly(2));
        }

        /// <summary>
        ///     Tests new a new hotel record is saved to the database
        /// </summary>
        [TestMethod]
        public void AddHotel()
        {
            int result;
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);
            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);
            }
            finally
            {
                //clean up 
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }

        /// <summary>
        ///     Tests a hotel can be retrieved from the database when searching for it by name
        /// </summary>
        [TestMethod]
        public void GetByName()
        {
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();
            Hotel objHotel;
            int result;

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);
            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);

                objHotel = Hotel.Get(hotel.hotelname);

                Assert.AreEqual(objHotel.hotelname, hotel.hotelname);
            }
            finally
            {
                //clean up
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }

        /// <summary>
        ///     Tests a hotel already exists in the database
        /// </summary>
        [TestMethod]
        public void AlreadyExists()
        {
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();
            var hotels = new Hotels();
            int result;
            bool outcome;

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);

            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);

                outcome = hotels.AlreadyExists(hotel.hotelname);

                Assert.IsTrue(outcome);
            }
            finally
            {
                //clean up
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }

        /// <summary>
        ///     Tests a distinct dataset of counties associated with hotels can be retrieved
        /// </summary>
        [TestMethod]
        public void GetCountyList()
        {
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();
            var hotels = new Hotels();
            var results = new DataSet();
            DataTable counties;
            DataRow[] foundRows;
            int result;

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);
            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);
                results = hotels.GetCountyList();

                if (results.Tables[0].Rows.Count > 0)
                {
                    counties = results.Tables[0];
                    //search for county within the dataset
                    foundRows = counties.Select("county = '" + hotel.county + "'");
                    Assert.IsTrue(foundRows.Count() == 1);
                }
            }
            finally
            {
                //clean up
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }

        /// <summary>
        ///     Tests a distinct dataset of cities associated with hotels can be retrieved
        /// </summary>
        [TestMethod]
        public void GetCityList()
        {
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();
            var hotels = new Hotels();
            var results = new DataSet();
            DataTable cities;
            DataRow[] foundRows;
            int result;

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);
            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);
                results = hotels.GetCityList(hotel.county);

                if (results.Tables[0].Rows.Count > 0)
                {
                    cities = results.Tables[0];
                    //search for city within the dataset
                    foundRows = cities.Select("city = '" + hotel.city + "'");
                    Assert.IsTrue(foundRows.Count() == 1);
                }
            }
            finally
            {
                //clean up
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }

        /// <summary>
        ///     Tests hotel data can be retrieved from the database by searching by its name or county/city
        /// </summary>
        [TestMethod]
        public void HotelSearch()
        {
            Hotel hotel = cHotelTemplate.GetUTHotelTemplateObject();
            var hotels = new Hotels();
            var resultsByName = new DataSet();
            var resultsByCountyCity = new DataSet();
            DataTable hotelByName, hotelByCountyCity;
            DataRow[] foundRows;
            int result;

            result = hotel.Add(hotel.hotelname, hotel.address1, hotel.address2, hotel.city, hotel.county,
                hotel.postcode,
                hotel.country, hotel.rating, hotel.telno, hotel.email, hotel.createdby);
            try
            {
                // Ensure the hotel has been saved and an ID has been set
                Assert.AreNotEqual(0, result);
                resultsByName = hotels.SearchForHotels(hotel.hotelname);

                if (resultsByName.Tables[0].Rows.Count > 0)
                {
                    hotelByName = resultsByName.Tables[0];
                    //search for hotel within the dataset
                    foundRows = hotelByName.Select("hotelname = '" + hotel.hotelname + "'");
                    Assert.IsTrue(foundRows.Count() == 1);
                }

                resultsByCountyCity = hotels.SearchForHotels(hotel.county, hotel.city);

                if (resultsByCountyCity.Tables[0].Rows.Count > 0)
                {
                    hotelByCountyCity = resultsByCountyCity.Tables[0];
                    //search for hotel within the dataset
                    foundRows = hotelByCountyCity.Select("hotelname = '" + hotel.hotelname + "'");
                    Assert.IsTrue(foundRows.Count() == 1);
                }
            }
            finally
            {
                if (result != 0 | result != -1)
                {
                    hotel.DeleteHotel(result);
                }
            }
        }
    }
}