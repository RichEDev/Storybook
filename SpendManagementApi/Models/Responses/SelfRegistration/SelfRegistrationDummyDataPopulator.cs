namespace SpendManagementApi.Models.Responses.SelfRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;

    using SpendManagementLibrary;

    /// <summary>
    /// A static class used for creating dummy data via the self registration api. This can be used for developers to create a user interface for self regsitration.
    /// </summary>
    public static class SelfRegistrationDummyDataPopulator
    {
        private const int NumberOfDummyRecords = 30;

        private const int NumberOfDummyItems = 21;

        /// <summary>
        /// Crteates a response for self registration via the API containing dummy data.
        /// </summary>
        /// <param name="response">API response for self registration</param>
        public static void PopulateStandardDummyResponse(SelfRegistrationResponse response)
        {
            response.AccessRoles = CreateDummyAccessRoles();
            response.CarEngineTypes = CreateDummyTuples("Car engine type");
            response.CostCodes = CreateDummyTuples("Cost code");
            response.Countries = CreateDummyTuples("Country");
            response.Currencies = CreateDummyTuples("Currency");
            response.Departments = CreateDummyTuples("Department");
            response.LineManagers = CreateDummyLineManagers();
            response.MileageUoMs = CreateDummyTuples("Mileage unit of measure");
            response.PostCodeAnywhereLicenseKey = "PA53-DM28-UK87-AP39";
            response.ProjectCodes = CreateDummyTuples("Department");
            response.SignoffGroups = CreateDummySignoffGroups();
            response.UserDefinedFields = CreateDummyUserdefinedFields();
            response.ResponseInformation = new ApiResponseInformation { Status = ApiStatusCode.Success };
        }

        private static List<IAccessRole> CreateDummyAccessRoles()
        {
            var accessRoles = new List<IAccessRole>();

            for (int i = 1; i <= NumberOfDummyRecords; i++)
            {
                accessRoles.Add(
                    new AccessRoleBasic
                    {
                        Id = i,
                        Label = string.Format("Access Role {0}", i),
                        Description = string.Format("Description for Test Access Role {0}", i)
                    });
            }

            return accessRoles;
        }

        private static List<IEmployee> CreateDummyLineManagers()
        {
            var lineManagers = new List<IEmployee>();

            for (int i = 1; i <= NumberOfDummyRecords; i++)
            {
                lineManagers.Add(
                    new EmployeeBasic
                    {
                        Id = i,
                        Forename = "Line Manager",
                        Surname = i.ToString(CultureInfo.InvariantCulture),
                        Title = "Mr"
                    });
            }

            return lineManagers;
        }

        private static List<SignoffGroupBasic> CreateDummySignoffGroups()
        {
            var signoffGroups = new List<SignoffGroupBasic>();

            for (int i = 1; i <= NumberOfDummyRecords; i++)
            {
                signoffGroups.Add(new SignoffGroupBasic
                                  {
                                      GroupId = i,
                                      GroupName = string.Format("Group {0}", i),
                                      Description = string.Format("Description of Group {0}", i)
                                  });
            }

            return signoffGroups;
        }

        private static List<UserDefinedField> CreateDummyUserdefinedFields()
        {
            int attributeId = 1;
            string attributeName = "Type1";
            string displayName = "Display name for Type 1";
            string description = "Description text for Type 1";
            string tooltip = "Tooltip help text for Type 1";
            const FieldType FieldType = FieldType.List;
            DateTime createdOn = DateTime.Now;
            const int CreatedBy = 0;
            Guid fieldIdGuid = Guid.NewGuid();
            const bool IsAuditIdentity = false;
            const bool IsUnique = true;
            int currentNumber = 1;

            SortedList<int, cListAttributeElement> listItems = CreateDummyListItems("Car");

            var employeeListAttribute = new cListAttribute(
                attributeId,
                attributeName,
                displayName,
                description,
                tooltip,
                false,
                FieldType,
                createdOn,
                CreatedBy,
                null,
                null,
                listItems,
                fieldIdGuid,
                IsAuditIdentity,
                IsUnique,
                AttributeFormat.ListStandard,
                true,
                false,
                false,
                true);

            var employeeTextAttribute = new cTextAttribute(
                attributeId,
                attributeName,
                displayName,
                description,
                tooltip,
                true,
                FieldType.Text,
                createdOn,
                CreatedBy,
                null,
                null,
                150,
                AttributeFormat.SingleLine,
                fieldIdGuid,
                IsAuditIdentity,
                IsUnique,
                true,
                false,
                true,
                false,
                true);

            var employeeLargeTextAttribute = new cTextAttribute(
                attributeId,
                attributeName,
                displayName,
                description,
                tooltip,
                true,
                FieldType.LargeText,
                createdOn,
                CreatedBy,
                null,
                null,
                4000,
                AttributeFormat.MultiLine,
                fieldIdGuid,
                IsAuditIdentity,
                IsUnique,
                true,
                false,
                true,
                false,
                true);

            var employeeDateTimeAttribute = new cDateTimeAttribute(
                attributeId,
                attributeName,
                displayName,
                description,
                tooltip,
                false,
                FieldType.DateTime,
                createdOn,
                CreatedBy,
                null,
                null,
                AttributeFormat.DateTime,
                fieldIdGuid,
                IsAuditIdentity,
                IsUnique,
                true,
                false,
                false,
                true);

            return PrepareUserDefinedFields(ref attributeId,
                ref attributeName,
                ref displayName,
                ref description,
                ref tooltip,
                ref createdOn,
                ref fieldIdGuid,
                ref currentNumber,
                employeeListAttribute,
                employeeTextAttribute,
                employeeLargeTextAttribute,
                employeeDateTimeAttribute
                );
        }

        private static List<UserDefinedField> PrepareUserDefinedFields(ref int attributeId, ref string attributeName, ref string displayName, ref string description, ref string tooltip, ref DateTime createdOn, ref Guid fieldIdGuid, ref int currentNumber, params cAttribute[] attributes)
        {
            var userdefinedFields = new List<UserDefinedField>();

            foreach (var attribute in attributes)
            {
                var userDefinedField = new UserDefinedField
                                       {
                                           AppliesTo = UDFAppliesTo.Employees,
                                           AllowEmployeeToPopulate = true,
                                           Order = currentNumber,
                                           Attribute = attribute
                                       };

                userdefinedFields.Add(userDefinedField);
                PrepareNextAttribute(
                        ref attributeId,
                        ref attributeName,
                        ref displayName,
                        ref description,
                        ref tooltip,
                        ref createdOn,
                        ref fieldIdGuid,
                        ref currentNumber);
            }

            return userdefinedFields;
        }

        private static void PrepareNextAttribute(ref int attributeId, ref string attributeName, ref string displayName, ref string description, ref string tooltip, ref DateTime createdOn, ref Guid fieldIdGuid, ref int currentNumber)
        {
            string currentNumberString = currentNumber.ToString(CultureInfo.InvariantCulture);
            string newNumberString = (currentNumber + 1).ToString(CultureInfo.InvariantCulture);

            attributeId++;
            attributeName = attributeName.Replace(currentNumberString, newNumberString);
            displayName = displayName.Replace(currentNumberString, newNumberString);
            description = description.Replace(currentNumberString, newNumberString);
            tooltip = tooltip.Replace(currentNumberString, newNumberString);
            createdOn = DateTime.Now;
            fieldIdGuid = Guid.NewGuid();
            currentNumber++;
        }


        private static SortedList<int, cListAttributeElement> CreateDummyListItems(string description)
        {
            var listItems = new SortedList<int, cListAttributeElement>();

            for (int i = 1; i <= NumberOfDummyItems; i++)
            {
                listItems.Add(i, new cListAttributeElement(i, string.Format("{0} list item {1}", description, i), i));
            }

            return listItems;
        }

        private static List<Tuple<int, string>> CreateDummyTuples(string description)
        {
            var keyValuePairList = new List<Tuple<int, string>>();

            for (int i = 1; i <= NumberOfDummyRecords; i++)
            {
                keyValuePairList.Add(new Tuple<int, string>(i, string.Format("{0} {1}", description, i)));
            }

            return keyValuePairList;
        }
    }
}