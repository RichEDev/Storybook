using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Infragistics.Web.UI.GridControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using SpendManagementLibrary.DocumentMerge;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace UnitTest2012Ultimate.DocumentMerge
{
    [TestClass]
    public class TorchTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        private static string location;

        private int accountId;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
            location = ConfigurationManager.AppSettings["LocationForCardTestFiles"];
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            GlobalVariables.DefaultModule = Modules.CorporateDiligence;
            this.accountId = GlobalTestVariables.AccountId;
        }

        #endregion

        [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteMergeDocument()
        {
            var groupingFilters = new List<TorchGroupingFieldFilter>();
            var sortingConfig = new List<TorchReportSorting>();
            //ExecuteMergeForDocuments(location + @"x.docx", new List<string> { "Country", "PracticeArea" }, groupingFilters, sortingConfig);
            ExecuteMergeForDocuments(@"c:\grr\begingroup.docx", new List<string> { "PracticeArea", "Country" }, groupingFilters, sortingConfig);
        }

        [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteMergeDocumentProjectDetails()
        {
            var groupingFilters = new List<TorchGroupingFieldFilter>();
            var sortingConfig = new List<TorchReportSorting>();
            var groupSortingConfig = new List<SortingColumn>();
            this.ExecuteMergeForDocumentsBasicProjectDetails(@"c:\grr\begingroup.docx", new List<string> { "Practice Area", "Country" }, groupingFilters, sortingConfig, groupSortingConfig);
            this.ExecuteMergeForDocumentsBasicProjectDetails(@"c:\grr\begingroup.docx", new List<string> { "Country", "Practice Area" }, groupingFilters, sortingConfig, groupSortingConfig);
        }

                [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteSummaryTableTest()
        {
            var groupingFilters = new List<TorchGroupingFieldFilter>();
            var sortingConfig = new List<TorchReportSorting>();
            this.ExecuteSummaryTableTest(@"c:\grr\AutoSummary.docx", new List<string>(), groupingFilters, sortingConfig);
        }


        [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteMergeDocumentProjectDetailsWithSortingAndFilteringOnGroupings()
        {
            var filter = new TorchGroupingFieldFilter("Currency", ConditionType.Equals, "Dollars", string.Empty, string.Empty, "S");
            
            var groupingFilters = new List<TorchGroupingFieldFilter> { filter };
            var groupSort = new SortingColumn("Practice Area", "desc");
            var groupSortingConfig = new List<SortingColumn> {groupSort};
            var sortingConfig = new List<TorchReportSorting>();
            this.ExecuteMergeForDocumentsBasicProjectDetails(location + @"BeginGroup.docx", new List<string> { "Practice Area", "Country" }, groupingFilters, sortingConfig, groupSortingConfig);
        }


        [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteMergeDocumentWithFilter()
        {
            var filter = new TorchGroupingFieldFilter("Country", ConditionType.Equals, "United Kingdom", string.Empty, string.Empty, "S");
            
            var groupingFilters = new List<TorchGroupingFieldFilter> { filter };
            var sortingConfig = new List<TorchReportSorting>();
            ExecuteMergeForDocuments(@"c:\grr\x.docx", new List<string> { "Country", "PracticeArea" }, groupingFilters, sortingConfig);
        }

        [TestMethod, TestCategory("DocumentMerge")]
        public void TextExecuteMergeDocumentWithSorting()
        {
            var filter = new TorchGroupingFieldFilter("Country", ConditionType.Equals, "United Kingdom", string.Empty, string.Empty, "S");
            
            var groupingFilters = new List<TorchGroupingFieldFilter> { filter };
            var sortingConfiguration = new List<TorchReportSorting>();
            var sortingColumn1 = new TorchReportSortingColumn("ZanzibarName", TorchSummaryColumnSortDirection.Descending);
            var sortingColumn2 = new TorchReportSortingColumn("Data room reference", TorchSummaryColumnSortDirection.Ascending);
            sortingConfiguration.Add(new TorchReportSorting("Zanzibar", new List<TorchReportSortingColumn> { sortingColumn1, sortingColumn2 }));
            ExecuteMergeForDocuments(@"c:\grr\x.docx", new List<string> { "Country", "PracticeArea" }, groupingFilters, sortingConfiguration);
        }

        [TestMethod, TestCategory("DocumentMerge")]
        public void TestBirdAndBirdMerge()
        {
            var myDataSet = new DataSet();
            myDataSet.ReadXml(@"c:\grr\mergedata.xml");
            var torchGroupingConfiguration = new TorchGroupingConfiguration(1, 1, new List<string> { "Data Room Reference"}, new List<SortingColumn>(), new List<TorchGroupingFieldFilter>(), new List<string>(), new List<TorchReportSorting>());
            //torchGroupingConfiguration.GroupingSources.Add("NEW Open Source Software");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Material trade secrets");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Licence/permit/authority");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Domain Name");
            //torchGroupingConfiguration.GroupingSources.Add("NEW IP Licensed by the Co.");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Registered Owner");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Data Protection");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Unregistered IP");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Standard Employment");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Patent");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Unregistered Design");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Finance Documents");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Security Documents");
            //torchGroupingConfiguration.GroupingSources.Add("NEW IP licensed to the Co.");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Pensions");
            torchGroupingConfiguration.GroupingSources.Add("NEW Corporate Comp info");
            
            //torchGroupingConfiguration.GroupingSources.Add("NEW Material Copyright");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Insurance");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Other Areas");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Material Contracts");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Trade Union/Works Council");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Registered Design");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Employ Dispute/Litigation");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Individual Consul Agree");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Employment contract");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Registered Trade Mark");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Competition");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Environmental");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Dispute Details");
            //torchGroupingConfiguration.GroupingSources.Add("NEW Property");


            var wordDoc = new WordDocument(@"c:\grr\docs\MinorTemplate.docx");
            using (var mergeEngine = new TorchDocIoMergeEngine(torchGroupingConfiguration, this.accountId))
            {
                mergeEngine.DataSet = myDataSet;
                mergeEngine.ExecuteNestedMerge(wordDoc);
                wordDoc.Save(@"c:\grr\1.docx", FormatType.Docx);
            }
        }

        private void ExecuteSummaryTableTest(string documentPath, List<string> groupingColumns, List<TorchGroupingFieldFilter> groupingFilters, List<TorchReportSorting> sortingConfiguration)
        {
            var torchGroupingConfiguration = new TorchGroupingConfiguration(1, 1, groupingColumns, new List<SortingColumn>(), groupingFilters, new List<string>(), sortingConfiguration);
            var myDataSet = new DataSet();
            //myDataSet.ReadXml(@"c:\grr\dataset.xml");

            var wordDoc = new WordDocument(documentPath);

            //for (int i = 50; i > 47; i--)
            //{
            //    myDataSet.Tables["NEWProjectDetails"].Columns.RemoveAt(i);
            //}

            //for (int i = 46; i >= 0; i--)
            //{
            //    myDataSet.Tables["NEWProjectDetails"].Columns.RemoveAt(i);
            //}

            //myDataSet.Tables.Remove("AutoSummary1");

            var projectTable = new DataTable("NEWProjectDetails");
            projectTable.Columns.Add("ID0", typeof(string));
            projectTable.Columns.Add("Project", typeof(string));

            var tableRow = projectTable.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "My juicy project";
            projectTable.Rows.Add(tableRow);

            var table = new DataTable("AutoSummary1");
            table.Columns.Add("ID0", typeof(string));
            table.Columns.Add("Issue", typeof(string));
            table.Columns.Add("PracticeArea", typeof(string));
            table.Columns.Add("Recommendation", typeof(string));
            table.Columns.Add("References", typeof(string));

            tableRow = table.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "Issue 1";
            tableRow[2] = "lala 1";
            tableRow[3] = "Go do one";
            tableRow[4] = "Paul G";

            table.Rows.Add(tableRow);
            tableRow = table.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "Issue 2";
            tableRow[2] = "lala 1";
            tableRow[3] = "Go do one";
            tableRow[4] = "Paul G"; 
            
            table.Rows.Add(tableRow);
            tableRow = table.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "Issue 3";
            tableRow[2] = "lala 3";
            tableRow[3] = "Go do one";
            tableRow[4] = "Paul G";
            table.Rows.Add(tableRow);
            tableRow = table.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "Issue 4";
            tableRow[2] = "lala 4";
            tableRow[3] = "Go do one";
            tableRow[4] = "Paul G";
            table.Rows.Add(tableRow);
            tableRow = table.NewRow();
            tableRow[0] = "1";
            tableRow[1] = "Issue 5";
            tableRow[2] = "lala 5";
            tableRow[3] = "Go do one";
            tableRow[4] = "Paul G";
            table.Rows.Add(tableRow);

            myDataSet.Tables.Add(projectTable);
            myDataSet.Tables.Add(table);

            using (var mergeEngine = new TorchDocIoMergeEngine(torchGroupingConfiguration, this.accountId))
            {
                mergeEngine.DataSet = myDataSet;
                mergeEngine.ExecuteNestedMerge(wordDoc);
            }

            wordDoc.Save(@"c:\shared\output.docx", FormatType.Docx);
            wordDoc.Close();
            wordDoc = null;

        }

        private void ExecuteMergeForDocuments(string documentPath, List<string> groupingColumns, List<TorchGroupingFieldFilter> groupingFilters, List<TorchReportSorting> sortingConfiguration)
        {
            var torchGroupingConfiguration = new TorchGroupingConfiguration(1, 1, groupingColumns, new List<SortingColumn>(), groupingFilters, new List<string>(), sortingConfiguration);
            torchGroupingConfiguration.GroupingSources.Add("Companies");
            torchGroupingConfiguration.GroupingSources.Add("Employment");


            var wordDoc = new WordDocument(documentPath);
            var myDataset = new DataSet();

            var projectTable = new DataTable("NEWProjectDetails");
            projectTable.Columns.Add("ID0", typeof (string));
            projectTable.Columns.Add("ProjectName", typeof (string));
            projectTable.Columns.Add("Overview", typeof(string));
            var prow = projectTable.NewRow();
            prow[0] = "1";
            prow[1] = "Project1";
            prow[2] = "This is the main overview text. woopee.";
            projectTable.Rows.Add(prow);

            var companiesTable = new DataTable("Companies");
            companiesTable.Columns.Add("ID0", typeof(string));
            companiesTable.Columns.Add("Country", typeof(string));
            companiesTable.Columns.Add("CompanyName", typeof(string));
            companiesTable.Columns.Add("PracticeArea", typeof(string));
            companiesTable.Columns.Add("ID1", typeof(string));

            var compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "United Kingdom";
            compRow[2] = "Acme Ltd";
            compRow[3] = "Practice Area 1";
            compRow[4] = "1";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "France";
            compRow[2] = "Fred Ltd";
            compRow[3] = "Practice Area 2";
            compRow[4] = "2";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "United Kingdom";
            compRow[2] = "The corner shop Ltd";
            compRow[3] = "Practice Area 1";
            compRow[4] = "3";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "Colombia";
            compRow[2] = "Oooh la la";
            compRow[3] = "Practice Area 1";
            compRow[4] = "4";
            companiesTable.Rows.Add(compRow);

            var employmentTable  = new DataTable("Employment");
            employmentTable.Columns.Add("ID0", typeof(string));
            employmentTable.Columns.Add("Country", typeof(string));
            employmentTable.Columns.Add("EmploymentName", typeof(string));
            employmentTable.Columns.Add("PracticeArea", typeof(string));
            employmentTable.Columns.Add("Data room reference", typeof(string));
            employmentTable.Columns.Add("Date of company search", typeof(string));
            employmentTable.Columns.Add("ID1", typeof(string));

            var zRow = employmentTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Paul Grfids";
            zRow[3] = "Practice Area 1";
            zRow[4] = "DataRoom1";
            zRow[5] = "Today";
            zRow[6] = "1";
            employmentTable.Rows.Add(zRow);
            zRow = employmentTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Manfred Ashaye";
            zRow[3] = "Practice Area 1";
            zRow[4] = "DataRoom1";
            zRow[5] = "Today";
            zRow[6] = "2";
            employmentTable.Rows.Add(zRow);
            zRow = employmentTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "France";
            zRow[2] = "Robyn Clarse";
            zRow[3] = "Practice Area 1";
            zRow[4] = "DataRoom10";
            zRow[5] = "Today";
            zRow[6] = "2";
            employmentTable.Rows.Add(zRow);
            zRow = employmentTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "France";
            zRow[2] = "Frigid Edwards";
            zRow[3] = "Practice Area 1";
            zRow[4] = "DataRoom22";
            zRow[5] = "Today";
            zRow[6] = "3";
            employmentTable.Rows.Add(zRow);



            myDataset.Tables.Add(companiesTable);
            myDataset.Tables.Add(projectTable);
            myDataset.Tables.Add(employmentTable);


            using (var mergeEngine = new TorchDocIoMergeEngine(torchGroupingConfiguration, this.accountId))
            {
                mergeEngine.DataSet = myDataset;
                mergeEngine.ExecuteNestedMerge(wordDoc);
            }

            wordDoc.Save(@"c:\shared\output.docx", FormatType.Docx);
            wordDoc.Close();
            wordDoc = null;
        }

        private void ExecuteMergeForDocumentsBasicProjectDetails(string documentPath, 
                                                                List<string> groupingColumns, 
                                                                List<TorchGroupingFieldFilter> groupingFilters, 
                                                                List<TorchReportSorting> sortingConfiguration,
                                                                List<SortingColumn> groupingSortingConfiguration)
        {
            var torchGroupingConfiguration = new TorchGroupingConfiguration(1, 1, groupingColumns, groupingSortingConfiguration, groupingFilters, new List<string>(), sortingConfiguration);
            torchGroupingConfiguration.GroupingSources.Add("New Corporate Comp info");

            var wordDoc = new WordDocument(documentPath);
            var myDataset = new DataSet();

            var projectTable = new DataTable("NEW Project Details");
            projectTable.Columns.Add("ID0", typeof(string));
            projectTable.Columns.Add("Project Name", typeof(string));
            projectTable.Columns.Add("introduction", typeof(string));
            var prow = projectTable.NewRow();
            prow[0] = "1";
            prow[1] = "Project1";
            prow[2] = "This is the main overview text. woopee.";
            projectTable.Rows.Add(prow);

            var companiesTable = new DataTable("New Corporate Comp info");
            companiesTable.Columns.Add("ID0", typeof(string));
            companiesTable.Columns.Add("Country", typeof(string));
            companiesTable.Columns.Add("Name of Company", typeof(string));
            companiesTable.Columns.Add("Practice Area", typeof(string));
            companiesTable.Columns.Add("ID1", typeof(string));
            companiesTable.Columns.Add("Currency", typeof(string));

            var compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "United Kingdom";
            compRow[2] = "The Massive Sausage Corporation";
            compRow[3] = "Practice Area 1";
            compRow[4] = "1";
            compRow[5] = "Sterling";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "United Kingdom";
            compRow[2] = "Ring a Ding a Ding Dong Limited";
            compRow[3] = "Practice Area 1";
            compRow[4] = "2";
            compRow[5] = "Sterling";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "United Kingdom";
            compRow[2] = "The John Junior and Junior John Junior and Sons Limited";
            compRow[3] = "Practice Area 1";
            compRow[4] = "3";
            compRow[5] = "Sterling";
            companiesTable.Rows.Add(compRow);
            compRow = companiesTable.NewRow();
            compRow[0] = "1";
            compRow[1] = "USA";
            compRow[2] = "Butt Cheeks R Us.gov.uk";
            compRow[3] = "Practice Area 1";
            compRow[4] = "4";
            compRow[5] = "Dollars";
            companiesTable.Rows.Add(compRow);

            var newIssuedSharesTable = new DataTable("NEW Issued Shares");
            newIssuedSharesTable.Columns.Add("ID0", typeof(string));
            newIssuedSharesTable.Columns.Add("Country", typeof(string));
            newIssuedSharesTable.Columns.Add("Class of share", typeof(string));
            newIssuedSharesTable.Columns.Add("Practice Area", typeof(string));
            newIssuedSharesTable.Columns.Add("ID1", typeof(string));
            newIssuedSharesTable.Columns.Add("ID2", typeof(string));
            newIssuedSharesTable.Columns.Add("Currency", typeof(string));
            
            var zRow = newIssuedSharesTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Class A";
            zRow[3] = "Practice Area 1";
            zRow[4] = "1";
            zRow[5] = "1";
            zRow[5] = "Sterling";
            newIssuedSharesTable.Rows.Add(zRow);
            zRow = newIssuedSharesTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Class B";
            zRow[3] = "Practice Area 1";
            zRow[4] = "1";
            zRow[5] = "2";
            zRow[6] = "Sterling";
            newIssuedSharesTable.Rows.Add(zRow);
            zRow = newIssuedSharesTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Class C";
            zRow[3] = "Practice Area 1";
            zRow[4] = "1";
            zRow[5] = "3";
            zRow[6] = "Sterling";
            newIssuedSharesTable.Rows.Add(zRow);
            zRow = newIssuedSharesTable.NewRow();
            zRow[0] = "1";
            zRow[1] = "United Kingdom";
            zRow[2] = "Class D";
            zRow[3] = "Practice Area 1";
            zRow[4] = "1";
            zRow[5] = "4";
            zRow[6] = "Sterling";
            newIssuedSharesTable.Rows.Add(zRow);

            myDataset.Tables.Add(companiesTable);
            myDataset.Tables.Add(projectTable);
            myDataset.Tables.Add(newIssuedSharesTable);


            using (var mergeEngine = new TorchDocIoMergeEngine(torchGroupingConfiguration, this.accountId))
            {
                mergeEngine.DataSet = myDataset;
                mergeEngine.ExecuteNestedMerge(wordDoc);
            }

            wordDoc.Save(@"c:\shared\output.docx", FormatType.Docx);
            wordDoc.Close();
            wordDoc = null;
        }

    }
}
