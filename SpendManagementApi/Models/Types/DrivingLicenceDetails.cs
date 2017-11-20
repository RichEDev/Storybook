namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using Responses;
    using SpendManagementLibrary.DVLA;

    /// <summary>
    /// Gives driving licence details
    /// </summary>
    public class DrivingLicenceDetails : BaseExternalType, IApiFrontForDbObject<DrivingLicenceList, EmployeesToPopulateDrivingLicence>
    {
        /// <summary>
        /// Gives employees information to populate driving licence
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeesToPopulateDrivingLicence"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public EmployeesToPopulateDrivingLicence From(DrivingLicenceList dbType, IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gives list of driving licence
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="DrivingLicenceList"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public DrivingLicenceList To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The driving licence information.
        /// </summary>
        /// <param name="drivingLicenceList">
        /// The driving licence list.
        /// </param>
        /// <returns>
        /// The <see cref="DrivingLicenceResponse"/>.
        /// Driving licence details
        /// </returns>
        public DrivingLicenceResponse Data(DrivingLicenceList drivingLicenceList)
        {
            var details = new DrivingLicenceResponse
                                                 {
                                                     DrivingLicenceDetails =
                                                     drivingLicenceList.DrivingLicenceInformationList
                                                 };
            return details;
        }
    }
}