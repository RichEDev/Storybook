using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cImportTemplateObject
    {
        /// <summary>
        /// Create a global import template object with associated mappings
        /// </summary>
        /// <returns></returns>
        public static cImportTemplate CreateImportTemplate()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cImportTemplates clsImportTemps = new cImportTemplates(cGlobalVariables.AccountID);
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();

            #region Create Import template Mappings

            cFields clsfields = new cFields(cGlobalVariables.AccountID);
            //Employee Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6a76898b-4052-416c-b870-61479ca15ac1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee Number", 2, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("28471060-247d-461c-abf6-234bcb4698aa"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Title", 3, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Last Name", 4, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6614acad-0a43-4e30-90ec-84de0792b1d6"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "First Name", 5, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b3caf703-e72b-4eb8-9d5c-b389e16c8c43"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Middle Names", 6, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("76473c0a-df08-40f9-8de0-632d0111a912"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Hire Date", 11, ImportElementType.Employee, true, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b7cbf994-4a23-4405-93df-d66d4c3ed2a3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Termination Date", 12, ImportElementType.Employee, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address 1st line", 14, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0330c639-1524-402b-b7bc-04d26bfc05a1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address Town", 16, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Postcode", 18, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("8816caec-b520-4223-b738-47d2f22f3e1a"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Country", 19, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0f951c3e-29d1-49f0-ac13-4cfcabf21fda"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Office e-Mail", 24, ImportElementType.Employee, false, DataType.stringVal));

            //ESR Assignment Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment ID", 1, ImportElementType.Assignment, true, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c23858b8-7730-440e-b481-c43fe8a1dbef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Number", 2, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c53828af-99ff-463f-93f9-2721df44e5f2"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Earliest Assignment Start Date", 3, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("36eb4bb6-f4d5-414c-9106-ee62db01d902"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Final Assignment End Date", 4, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9721ec22-404b-468b-83a4-d17d7559d3ef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Status", 5, ImportElementType.Assignment, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("3e5750f0-1061-46c4-ad94-089d40a62dec"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address 1st Line", 9, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("737a99ad-b0c5-4325-a565-c4d3fba536dd"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address Postcode", 13, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Supervisor Employee Number", 17, ImportElementType.Assignment, false, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("fec46ed7-57f9-4c51-9916-ec92834371c3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Primary Assignment", 22, ImportElementType.Assignment, true, DataType.booleanVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c50dae62-8dae-4289-a0ce-584c3129159e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Location", 37, ImportElementType.Assignment, true, DataType.stringVal));

            //Linked to the employees table
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("5f4a4551-1c05-4c85-b6d9-06d036bc327e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Job Name", 39, ImportElementType.Assignment, false, DataType.stringVal));

            #endregion

            int tempImportTemplateID = clsImportTemps.saveImportTemplate(new cImportTemplate(0, "Unit Test Template", ApplicationType.ESROutboundImport, true, cGlobalVariables.NHSTrustID, lstMappings, DateTime.Now, cGlobalVariables.EmployeeID, null, null));
            cGlobalVariables.TemplateID = tempImportTemplateID;

            //Interim solution
            //System.Threading.Thread.Sleep(1000);
            //clsImportTemps = new cImportTemplates(cGlobalVariables.AccountID);

            cImportTemplate template = clsImportTemps.getImportTemplateByID(tempImportTemplateID);
            return template;
        }

        /// <summary>
        /// Delete the global import template object from the database
        /// </summary>
        public static void DeleteImportTemplate()
        {
            cImportTemplates clsImportTemps = new cImportTemplates(cGlobalVariables.AccountID);
            clsImportTemps.deleteImportTemplate(cGlobalVariables.TemplateID);
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfo()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The header record has been removed
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoHeaderRecord()
        {
            StringBuilder strBuild = new StringBuilder();
            
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The footer record has been removed
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoFooterRecord()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The no of records in the footer record does not match the number of records in the file
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithInvalidNumberRecords()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithEmployeeNumberSetToNothing()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// with no employee title set
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoTitle()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// with no assignment location set 
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoAssignmentLocationSet()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// and the assignment record assignment number being set to nothing
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithAssignmentNumberSetToNothing()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an edited file with the employee email changed and the Assignment final date set
        /// </summary>
        /// <returns></returns>
        public static byte[] EditedDummyESROutboundFileInfo()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an edited file with new home and work addresses
        /// </summary>
        /// <returns></returns>
        public static byte[] EditedDummyESROutboundFileInfoWithNewHomeAndWorkAddresses()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an ESR Outbound file with the employee first and then their associated line manager
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithLineManagerNotPreExisting()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("PER,5073743,21272386,\"Mrs.\",\"Gill\",\"Catherine\",\"Spencer\",\"Walton\",\"Female\",19660407,NH829196B,20090302,,Yes,\"9, Oak Bank\",\"Whinney Hill Road\",\"Accrington\",\"Lancashire\",\"BB5 6NR\",\"GB\",\"linemanagertest@test.com\",\"01254 234539\",\"\",\"07528204188\",\"\",\"\",\"01282 474416\",,\"1610405\",\"\"\r\n");
            strBuild.Append("ASG,4689816,21272386,20090302,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Accrington Victoria Hospital\",\"Haywood Road\",\"Accrington\",\"LAN\",\"BB5 6AS\",\"GB\",1,,10807494,\"Mawdsley, Mrs. Carole Pamela\",\"\",FT,\"Permanent\",1,37.5,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"5317383|Modern Matron Band 7|NCH|Community Health Services|\",\"Modern Matron\",\"NCH\",\"638 04 Accrington Victoria Hos\",\"NHS|XR07|Review Body Band 7\",\"Nursing and Midwifery Registered|Modern Matron\",\"638 z 9643 USC Operational Managers|||\",\"GTA||\",,\"\",\"11569147\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an ESR Outbound file with the employee first and then their associated line manager
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithLineManagerPreExisting()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,5073743,21272386,\"Mrs.\",\"Gill\",\"Catherine\",\"Spencer\",\"Walton\",\"Female\",19660407,NH829196B,20090302,,Yes,\"9, Oak Bank\",\"Whinney Hill Road\",\"Accrington\",\"Lancashire\",\"BB5 6NR\",\"GB\",\"linemanagertest@test.com\",\"01254 234539\",\"\",\"07528204188\",\"\",\"\",\"01282 474416\",,\"1610405\",\"\"\r\n");
            strBuild.Append("ASG,4689816,21272386,20090302,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Accrington Victoria Hospital\",\"Haywood Road\",\"Accrington\",\"LAN\",\"BB5 6AS\",\"GB\",1,,10807494,\"Mawdsley, Mrs. Carole Pamela\",\"\",FT,\"Permanent\",1,37.5,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"5317383|Modern Matron Band 7|NCH|Community Health Services|\",\"Modern Matron\",\"NCH\",\"638 04 Accrington Victoria Hos\",\"NHS|XR07|Review Body Band 7\",\"Nursing and Midwifery Registered|Modern Matron\",\"638 z 9643 USC Operational Managers|||\",\"GTA||\",,\"\",\"11569147\"\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }
    }
}
