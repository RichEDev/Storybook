using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.Library.Helpers
{
    using Moq;

    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;

    [TestClass]
    public class EsrInboundBuilderTests
    {
        [TestMethod]
        public void EsrInboundBuilderNoSummary()
        {
            var esrFormatting = new Mock<IEsrInboundFormatting>();
            esrFormatting.SetupAllProperties();
            var esrInboundBuilder = new EsrInboundBuilder(false, esrFormatting.Object, EsrRoundingType.Down);
            esrInboundBuilder.Append("one\r");
            esrInboundBuilder.Append("two,two\r");
            esrInboundBuilder.Append("three,three,three");
            var result = esrInboundBuilder.ToString();
            Assert.IsTrue(result == "one\rtwo,two\rthree,three,three");
        }


        [TestMethod]
        public void EsrInboundBuilderWithSummary()
        {
            var esrFormatting = new Mock<IEsrInboundFormatting>();
            esrFormatting.SetupAllProperties();
            var esrInboundBuilder = new EsrInboundBuilder(true, esrFormatting.Object, EsrRoundingType.Down);
            esrInboundBuilder.Append("HDR,TA_431_SEL_WN1701_00000048.DAT,20160427135134,SEL,431,W,N,01\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",23-Mar-2012,\"Claim End Date\",23 - Mar - 2012,\"Period Cash Amount\",12.30,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",29-May-2013,\"Claim End Date\",29 - May - 2013,\"Period Cash Amount\",4.90,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",02-Jul-2013,\"Claim End Date\",02 - Jul - 2013,\"Period Cash Amount\",222.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,123456-5,,\"Expenses NR NP NHS\",\"Claim Start Date\",14-Aug-2013,\"Claim End Date\",14 - Aug - 2013,\"Period Cash Amount\",8.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",11 - Dec -2013,\"Claim End Date\",11 - Dec - 2013,\"Period Cash Amount\",25.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",17 - Nov -2014,\"Claim End Date\",17 - Nov - 2014,\"Period Cash Amount\",20.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,123456-5,,\"Expenses NR NP NHS\",\"Claim Start Date\",17 - Nov - 2014,\"Claim End Date\",17 - Nov - 2014,\"Period Cash Amount\",15.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",18 - Nov - 2014,\"Claim End Date\",18 - Nov - 2014,\"Period Cash Amount\",1.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",18 - Nov - 2014,\"Claim End Date\",18 - Nov - 2014,\"Period Cash Amount\",3.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",19 - Nov - 2014,\"Claim End Date\",19 - Nov - 2014,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,54321,,\"Expenses NR NP NHS\",\"Claim Start Date\",05 - Mar - 2015,\"Claim End Date\",05 - Mar - 2015,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,54321,,\"Expenses NR NP NHS\",\"Claim Start Date\",05 - Mar - 2015,\"Claim End Date\",05 - Mar - 2015,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",22 - Mar - 2016,\"Claim End Date\",22 - Mar - 2016,\"Period Cash Amount\",10.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",22 - Mar - 2016,\"Claim End Date\",22 - Mar - 2016,\"Period Cash Amount\",10.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160427,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",10 - Apr - 2016,\"Claim End Date\",10 - Apr - 2016,\"Period Cash Amount\",15.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("FTR,15,0\r");

            var result = esrInboundBuilder.ToString();
            Assert.IsTrue(result.EndsWith("FTR,12,0"));
        }

        [TestMethod]
        public void EsrInboundBuilderWithSummaryAndMileage()
        {
            var esrFormatting = new Mock<IEsrInboundFormatting>();
            esrFormatting.SetupAllProperties();
            var esrInboundBuilder = new EsrInboundBuilder(true, esrFormatting.Object, EsrRoundingType.Down);
            esrInboundBuilder.Append("HDR,TA_365_SEL_WN1705_00000086.DAT,20160830100002,SEL,365,W,N,05\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123456,,\"Business Miles NR NHS\",\"Claim Start Date\",08-Jul-2009,\"Claim End Date\",08-Jul-2009,\"Claimed Mileage\",25,\"Actual Mileage\",25,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123,,\"Business Miles NR NHS\",\"Claim Start Date\",20-Oct-2011,\"Claim End Date\",20-Oct-2011,\"Claimed Mileage\",48,\"Actual Mileage\",48,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123,,\"Business Miles NR NHS\",\"Claim Start Date\",20-Oct-2011,\"Claim End Date\",20-Oct-2011,\"Claimed Mileage\",37,\"Actual Mileage\",37,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123,,\"Business Miles NR NHS\",\"Claim Start Date\",27-Nov-2011,\"Claim End Date\",27-Nov-2011,\"Claimed Mileage\",127,\"Actual Mileage\",127,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",23-Mar-2012,\"Claim End Date\",23-Mar-2012,\"Claimed Mileage\",3,\"Actual Mileage\",3,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",23-Mar-2012,\"Claim End Date\",23-Mar-2012,\"Period Cash Amount\",12.30,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Sep-2012,\"Claim End Date\",12-Sep-2012,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Feb-2013,\"Claim End Date\",12-Feb-2013,\"Claimed Mileage\",80,\"Actual Mileage\",80,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",29-May-2013,\"Claim End Date\",29-May-2013,\"Period Cash Amount\",4.90,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",29-May-2013,\"Claim End Date\",29-May-2013,\"Claimed Mileage\",58,\"Actual Mileage\",58,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Jun-2013,\"Claim End Date\",06-Jun-2013,\"Claimed Mileage\",23,\"Actual Mileage\",23,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",02-Jul-2013,\"Claim End Date\",02-Jul-2013,\"Period Cash Amount\",222.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",05-Jul-2013,\"Claim End Date\",05-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Jul-2013,\"Claim End Date\",06-Jul-2013,\"Claimed Mileage\",23,\"Actual Mileage\",23,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Jul-2013,\"Claim End Date\",06-Jul-2013,\"Claimed Mileage\",23,\"Actual Mileage\",23,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Jul-2013,\"Claim End Date\",06-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Jul-2013,\"Claim End Date\",06-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",07-Jul-2013,\"Claim End Date\",07-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",10-Jul-2013,\"Claim End Date\",10-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",14-Jul-2013,\"Claim End Date\",14-Jul-2013,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",22-Jul-2013,\"Claim End Date\",22-Jul-2013,\"Claimed Mileage\",19,\"Actual Mileage\",19,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123456-5,,\"Expenses NR NP NHS\",\"Claim Start Date\",14-Aug-2013,\"Claim End Date\",14-Aug-2013,\"Period Cash Amount\",8.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",11-Dec-2013,\"Claim End Date\",11-Dec-2013,\"Period Cash Amount\",25.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",17-Dec-2013,\"Claim End Date\",17-Dec-2013,\"Claimed Mileage\",12,\"Actual Mileage\",12,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",09-Sep-2014,\"Claim End Date\",09-Sep-2014,\"Claimed Mileage\",56,\"Actual Mileage\",56,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",09-Sep-2014,\"Claim End Date\",09-Sep-2014,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Sep-2014,\"Claim End Date\",12-Sep-2014,\"Claimed Mileage\",12,\"Actual Mileage\",12,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Sep-2014,\"Claim End Date\",12-Sep-2014,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Sep-2014,\"Claim End Date\",12-Sep-2014,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",15-Sep-2014,\"Claim End Date\",15-Sep-2014,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",15-Sep-2014,\"Claim End Date\",15-Sep-2014,\"Claimed Mileage\",15,\"Actual Mileage\",15,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",17-Nov-2014,\"Claim End Date\",17-Nov-2014,\"Period Cash Amount\",20.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",17-Nov-2014,\"Claim End Date\",17-Nov-2014,\"Claimed Mileage\",65,\"Actual Mileage\",65,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,123456-5,,\"Expenses NR NP NHS\",\"Claim Start Date\",17-Nov-2014,\"Claim End Date\",17-Nov-2014,\"Period Cash Amount\",15.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",18-Nov-2014,\"Claim End Date\",18-Nov-2014,\"Period Cash Amount\",1.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",18-Nov-2014,\"Claim End Date\",18-Nov-2014,\"Period Cash Amount\",3.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Expenses NR NP NHS\",\"Claim Start Date\",19-Nov-2014,\"Claim End Date\",19-Nov-2014,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",04-Nov-2014,\"Claim End Date\",04-Nov-2014,\"Claimed Mileage\",61,\"Actual Mileage\",61,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,54321,,\"Expenses NR NP NHS\",\"Claim Start Date\",05-Mar-2015,\"Claim End Date\",05-Mar-2015,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,54321,,\"Expenses NR NP NHS\",\"Claim Start Date\",05-Mar-2015,\"Claim End Date\",05-Mar-2015,\"Period Cash Amount\",12.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",27-Mar-2015,\"Claim End Date\",27-Mar-2015,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,RP123456,,\"Business Miles NR NHS\",\"Claim Start Date\",26-May-2015,\"Claim End Date\",26-May-2015,\"Claimed Mileage\",34,\"Actual Mileage\",34,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",06-Aug-2015,\"Claim End Date\",06-Aug-2015,\"Claimed Mileage\",20,\"Actual Mileage\",20,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Aug-2015,\"Claim End Date\",12-Aug-2015,\"Claimed Mileage\",20,\"Actual Mileage\",20,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,robyn-1,,\"Business Miles NR NHS\",\"Claim Start Date\",18-Sep-2015,\"Claim End Date\",18-Sep-2015,\"Claimed Mileage\",9,\"Actual Mileage\",9,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",21-Jan-2016,\"Claim End Date\",21-Jan-2016,\"Claimed Mileage\",18,\"Actual Mileage\",18,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234,,\"Business Miles NR NHS\",\"Claim Start Date\",12-Feb-2016,\"Claim End Date\",12-Feb-2016,\"Claimed Mileage\",1,\"Actual Mileage\",1,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",22-Mar-2016,\"Claim End Date\",22-Mar-2016,\"Period Cash Amount\",10.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",22-Mar-2016,\"Claim End Date\",22-Mar-2016,\"Period Cash Amount\",10.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Business Miles NR NHS\",\"Claim Start Date\",07-Apr-2016,\"Claim End Date\",07-Apr-2016,\"Claimed Mileage\",7,\"Actual Mileage\",7,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Business Miles NR NHS\",\"Claim Start Date\",07-Apr-2016,\"Claim End Date\",07-Apr-2016,\"Claimed Mileage\",3,\"Actual Mileage\",3,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Business Miles NR NHS\",\"Claim Start Date\",07-Apr-2016,\"Claim End Date\",07-Apr-2016,\"Claimed Mileage\",19,\"Actual Mileage\",19,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Business Miles NR NHS\",\"Claim Start Date\",07-Apr-2016,\"Claim End Date\",07-Apr-2016,\"Claimed Mileage\",2,\"Actual Mileage\",2,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("ATT,20160830,ADD,,1234141,,\"Expenses NR NP NHS\",\"Claim Start Date\",10-Apr-2016,\"Claim End Date\",10-Apr-2016,\"Period Cash Amount\",15.00,,,,,,,,,,,,,,,,,,,,,,,,,,,\r");
            esrInboundBuilder.Append("FTR,54,0\r");

            var result = esrInboundBuilder.ToString();
            Assert.IsTrue(result.EndsWith("FTR,41,0"));
        }
    }
}
