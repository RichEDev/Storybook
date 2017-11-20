using Spend_Management.AttachmentBackups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Configuration;
using System.Linq;
using Syncfusion.XlsIO;

namespace UnitTest2012Ultimate.Attachments
{

    /// <summary>
    ///This is a test class for ZipCreatorZipCreatorTest and is intended
    ///to contain all ZipCreatorZipCreatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttachmentBackupManagerTests
    {
        private static string sZipDestinationPath;
        private static string sTestFilesPath;
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Class Initialise & Cleanup

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            sZipDestinationPath = ConfigurationManager.AppSettings["AttachmentsZipDestinationPath"];
            sTestFilesPath = ConfigurationManager.AppSettings["AttachmentsTestFilesPath"];
            // build server will perform tests on the d drive.
            if (!Directory.Exists(sTestFilesPath))
            {
                sTestFilesPath = sTestFilesPath.Replace("c:", "d:");
                sZipDestinationPath = sZipDestinationPath.Replace("c:", "d:");
            }
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            string[] tempFiles = Directory.GetFiles(sZipDestinationPath);
            foreach (string file in tempFiles)
                File.Delete(file);
        }

        #endregion

        /// <summary>
        /// If there are no files in the destination path to compress, then processing engine should return an empty zip file and not crash out.
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void CompressingEmptyDirectoryShouldCreateEmptyZip()
        {
            string zip_label = "zipvolume";
            string[] files = Directory.GetFiles(sTestFilesPath, "*.NOTEXIST", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Should receive an error when trying to add the same file in the same directory twice to the zip
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ZipProcessShouldErrorWhenAddingTheSameFileTwice()
        {
            string zip_label = "zipvol1";
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            AttachmentBackupDetails abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test1\test.jpg";
            attachments.Add(abd);
            abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test1\test.jpg";
            attachments.Add(abd);
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
        }

        /// <summary>
        /// Compressing one valid docx file should produce a zip with one docx file.
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void CompressingOneDocxFileShouldCreateValidZip()
        {
            string zip_label = "zipvol10";
            string[] files = Directory.GetFiles(sTestFilesPath + "test10", "*.docx", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Compressing files that are open should produce a valid zip.
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void CompressingOpenFilesShouldCreateValidZip()
        {
            string zip_label = "zipvol11";
            string[] files = Directory.GetFiles(sTestFilesPath + "test11", "*.*", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            using (FileStream fs1 = File.OpenRead(files[0]))
            {
                using (FileStream fs2 = File.OpenRead(files[1]))
                {
                    List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
                    Assert.AreEqual(1, actual.Count);
                    using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
                    {
                        Assert.AreEqual(files.Length, zip.Entries.Count);
                        zip.Dispose();
                    }
                    fs2.Close();
                }
                fs1.Close();
            }
        }

        /// <summary>
        /// Should receive a valid zip when adding the same file from different directories
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void ShouldReceiveValidZipWhenAddingSameFileFromDifferentDirectories()
        {
            string zip_label = "zipvol02";
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            AttachmentBackupDetails abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test2\test.jpg";
            attachments.Add(abd);
            abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test2\directory\test.jpg";
            attachments.Add(abd);
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(attachments.Count, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// A test for adding 200 jpg files
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void Compress70JPGsNoVolumeSplitting()
        {
            string zip_label = "zipvol03";
            string[] files = Directory.GetFiles(sTestFilesPath + "test3", "*.jpg", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Two zip volumes should be created when splitting over 5Mb of jpgs with a 3Mb split range, 70 files should exists between the two
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void TwoVolumesShouldBeCreatedWhenA3MbSplitIsUsedOnOver5MbOfJpgs()
        {
            string zip_label = "zipvol04";
            string[] files = Directory.GetFiles(sTestFilesPath + "test4", "*.jpg", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 3);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(sZipDestinationPath + "zipvol04.z01", actual[1].ZipFilePath);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Six zip volumes should be created when splitting over 5Mb of jpgs with a 1Mb split range, 70 files should exists between all
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void FiveVolumesShouldBeCreatedWhenA1MbSplitIsUsedOnOver5MbOfJpgs()
        {
            string zip_label = "zipvol05";
            string[] files = Directory.GetFiles(sTestFilesPath + "test5", "*.jpg", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 1);
            Assert.AreEqual(6, actual.Count);
            Assert.AreEqual(sZipDestinationPath + "zipvol05.z01", actual[1].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol05.z02", actual[2].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol05.z03", actual[3].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol05.z04", actual[4].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol05.z05", actual[5].ZipFilePath);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// If the split volume size is smaller than the size of an individual file, then the file will not be placed in the zip
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void SplittingA3MbBMPWithA1MbSplitShouldCreateValidZipFile()
        {
            string zip_label = "zipvol06";
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            AttachmentBackupDetails abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test6\silver surfer.bmp";
            attachments.Add(abd);
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 1);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(sZipDestinationPath + "zipvol06.zip", actual[0].ZipFilePath);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(attachments.Count, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// When more than ten volumes are created, we should see zipfile.z10 and not zipfile.z010 returned in the list of volumes
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void WhenMoreThan10VolumesAreCreatedThe10thShouldBeDotZ10()
        {
            string zip_label = "zipvol07";
            var vfiles = Directory.GetFiles(sTestFilesPath + "test7", "*.*", SearchOption.TopDirectoryOnly).Where(x => x.ToLower().EndsWith(".zip"));
            string[] files = vfiles.ToArray();
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 90);
            Assert.IsTrue(actual.Count > 10);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z01", actual[1].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z02", actual[2].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z03", actual[3].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z04", actual[4].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z05", actual[5].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z06", actual[6].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z07", actual[7].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z08", actual[8].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z09", actual[9].ZipFilePath);
            Assert.AreEqual(sZipDestinationPath + "zipvol07.z10", actual[10].ZipFilePath);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual((int)files.Count(), zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// When the created zip file will be larger than 700Mb, we should still see a nice split in the volume archives
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void TwoVolumesShouldBeCreatedWithOver1GbOfFilesZippedWithDefault700MbSplit()
        {
            string zip_label = "zipvol08";
            string[] files = Directory.GetFiles(sTestFilesPath + "test8", "*.zip", SearchOption.TopDirectoryOnly);
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            for (int i = 0; i < files.Length; i++)
            {
                AttachmentBackupDetails abd = new AttachmentBackupDetails();
                abd.FileName = files[i];
                attachments.Add(abd);
            }
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(sZipDestinationPath + "zipvol08.z01", actual[1].ZipFilePath);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(files.Length, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Should receive a valid zip + config spreadsheet if option selected
        ///</summary>
        [TestCategory("Attachment Backup Manager"), TestMethod()]
        public void ShouldReceiveConfigSpreadsheetWhenOptionSelected()
        {
            string zip_label = "zipvol09";
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            AttachmentBackupDetails abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test9\test.jpg";
            attachments.Add(abd);
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 700, true);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(attachments.Count + 1, zip.Entries.Count);
                zip.Dispose();
            }
        }

        /// <summary>
        /// Should receive a valid zip + config (not empty) spreadsheet if option selected
        ///</summary>
        //[TestCategory("Attachment Backup Manager"), TestMethod()]
        public void ShouldReceiveNonEmptyConfigSpreadsheetWhenOptionSelected()
        {
            string zip_label = "zipvol12";
            List<AttachmentBackupDetails> attachments = new List<AttachmentBackupDetails>();
            AttachmentBackupDetails abd = new AttachmentBackupDetails();
            abd.FileName = sTestFilesPath + @"test12\test.jpg";
            attachments.Add(abd);
            List<AttachmentBackupZipDetails> actual = AttachmentBackupManager.CompressFiles(zip_label, sZipDestinationPath, attachments, 700, true);
            Assert.AreEqual(1, actual.Count);
            using (ZipFile zip = new ZipFile(actual[0].ZipFilePath))
            {
                Assert.AreEqual(attachments.Count + 1, zip.Entries.Count);
                zip.ExtractAll(sZipDestinationPath, ExtractExistingFileAction.OverwriteSilently);
                ExcelEngine xl = new ExcelEngine();
                using (xl)
                {
                    IApplication xlApp = xl.Excel;
                    IWorkbook wkbk = xl.Excel.Workbooks.OpenReadOnly(sZipDestinationPath + sTestFilesPath.Replace(Path.GetPathRoot(sTestFilesPath), "") + @"test12\info.xls");
                    IWorksheet sht1 = wkbk.Worksheets[0];
                    Assert.AreEqual(attachments[0].FileName, sht1.Range["J2"].Text);
                    wkbk.Close();
                }
                zip.Dispose();
            }
            Directory.Delete(sZipDestinationPath + sTestFilesPath.Replace(Path.GetPathRoot(sTestFilesPath), ""), true);
        }
    }
}

