namespace Spend_Management.shared.code.EasyTree
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary;

    /// <summary>
    /// A class that manages the <see cref="cField"/>that should be excluded from the tree view.
    /// </summary>
    public class FilteredFields
    {
        private readonly List<Guid> _nonRequiredFields;
        private List<Guid> _topLevelFields;

        /// <summary>
        /// Create an instance of <see cref="FilteredFields"/>
        /// </summary>
        /// <param name="userCurrentActiveModule"></param>
        public FilteredFields(Modules userCurrentActiveModule)
        {
            this._nonRequiredFields = new List<Guid>
                                          {
                                              new Guid("A5C7640E-3D04-4C17-9917-83410A862A78"),
                                              new Guid("A3E74AE0-F3E5-4A03-93A0-A0F09D5232E4"),
                                              new Guid("FF305140-8950-4E0B-8484-37E16F42AB27"),
                                              new Guid("9746A85B-28D1-4F33-932F-E4F5F4C856FF"),
                                              new Guid("6C30E307-274A-45D2-A275-23EBA15C8B9F"),
                                              new Guid("362D9594-8A7C-4055-BE62-23F19627C575"),
                                              new Guid("10478586-2619-4863-B132-54FE6559CC09"),
                                              new Guid("150402D8-E657-47BB-AC44-7B32E81DEE7A"),
                                              new Guid("BE7D20D9-4533-45C9-8B78-929EA7498BAE"),
                                              new Guid("C88163C1-9ADA-4A8B-8496-93076A7D4318"),
                                              new Guid("8EEB12B4-1EFA-421B-A1AF-C2570C57344F"),
                                              new Guid("F2FD2A43-BFC7-4ED9-AC0E-D9E728FD2BCE"),
                                              new Guid("15A6EE4C-6F35-4038-BA2A-C3A238A23B90"),
                                              new Guid("9A2A7293-1C2F-48F8-8FD6-DF441F76F40E"),
                                              new Guid("89314961-5E7D-460E-9597-C982A11DB803"),
                                              new Guid("46DD18D1-97D9-45DF-91D8-C78DEDC477B8"),
                                              new Guid("E2E87A10-56E1-4106-A707-739F228862A2"),
                                              new Guid("394400E4-8E59-4C0C-B1D5-0FDB52706D55"),
                                              new Guid("28BF579C-205E-4C56-9D6D-F7AD01538221")
                                          };
            this._topLevelFields = new List<Guid>
            {
                new Guid("15F07382-D310-46E0-848D-732EDA6262A9"), new Guid("2E3BB48A-B316-4A9C-9E7B-07431ABE58D4"), new Guid("0C028167-AFD5-44E9-8D79-8CF9CBF73F00"), new Guid("021FB0B8-910E-4F57-8D84-15838B201262"), new Guid("56A88773-C55C-4066-8629-15148C8F8E90"), new Guid("77FADE9B-0BD9-4A76-8BFB-AEE8ADCD4812")
            };

            switch (userCurrentActiveModule)
            {
                case Modules.contracts:
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.ESR:
                case Modules.Greenlight:
                case Modules.CorporateDiligence:
                case Modules.GreenlightWorkforce:
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesCurrentRefNumber));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesCurrentClaimNumber ));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesApplicantActiveStatusFlag ));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesEsrEffectiveEndDate));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesEsrEffectiveStartDate ));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesCountry));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesApplicantNumber ));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesEsrPersonType));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesGetEmployeeDepartmentNameFromEmployeeId));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesGetEmployeeJobTitleFromEmployeeId));
                    this._nonRequiredFields.Add(new Guid(ReportFields.EmployeesEsrPersonId));
                    break;
            }
        }

        /// <summary>
        /// Should this field be excluded from the tree?
        /// </summary>
        /// <param name="field">The <see cref="cField"/>to test</param>
        /// <param name="currentId">The current node id</param>
        /// <returns>True if the field should be excluded.</returns>
        public bool FilterField(cField field, string currentId)
        {
            if (this._nonRequiredFields.Contains(field.FieldID))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(currentId))
            {
                if (this._topLevelFields.Contains(field.FieldID))
                {
                    return true;
                }
            }

            return false;
        }
    }
}