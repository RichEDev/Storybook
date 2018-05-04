namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using System;
    using Interfaces;
    using Enumerators;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Linq;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The duty of care document.
    /// </summary>
    public class DutyOfCareDocuments
    {
        /// <summary>
        /// Checks whether all the documents related to the car is valid
        /// </summary>
        /// <param name="accountId">account Id of current user</param>
        /// <param name="employeeId">The employee ID of the user we're checking duty of care for</param>
        /// <param name="expenseItemDate">date of the expense item</param>
        /// <param name="activeCars">The cars to check duty of care results for</param>
        /// <returns>List of documents which are not valid with a bool value whether manual licence is valid or not</returns>
        public Dictionary<List<DocumentExpiryResult>, bool> PassesDutyOfCare(int accountId, List<cCar> activeCars, int employeeId, DateTime expenseItemDate, bool hasDvlaLookupKeyAndDvlaConnectLicenceElement, cAccountProperties accountProperties, IVehicleValidator vehicleValidator)
        {
            var listOfDofCDocuments = new List<DocumentExpiryResult>();
            var results = new Dictionary<List<DocumentExpiryResult>, bool>();
            var result = new DocumentExpiryResult();
            var documentsInformation = new DutyOfCareDocumentsInformation();
            var licenceDocuments = documentsInformation.GetLicenceExpiryInformation(accountId, employeeId, expenseItemDate, hasDvlaLookupKeyAndDvlaConnectLicenceElement);
            bool isManualDocumentValid = false;

            foreach (IDutyOfCareDocument document in licenceDocuments)
            {
                result = document.HasExpired(expenseItemDate.Date);
                if (result.HasExpired)
                {
                    listOfDofCDocuments.Add(result);
                }
                if (result.IsValidManualLicence)
                {
                    isManualDocumentValid = true;
                }
            }

            if (listOfDofCDocuments.Count > 0)
            {
                results.Add(listOfDofCDocuments, isManualDocumentValid);
                return results;
            }

            foreach (var car in activeCars)
            {
                if (!string.IsNullOrEmpty(car.registration))
                {

                    var documents = documentsInformation.GetCarExpiryInformation(accountId, car.carid, expenseItemDate);

                    if (accountProperties.PopulateDocumentsFromVehicleLookup)
                    {
                        var updatedCar = this.UpdateTaxAndMotExpiryOnVehicle(documents, expenseItemDate, car); //if expired update car
                        if ((car.IsMotValid && !updatedCar.IsMotValid) || (car.IsTaxValid && !updatedCar.IsTaxValid))
                        {
                            var validatedCar = vehicleValidator.ValidateCar(updatedCar);
                            if (validatedCar.IsTaxValid && validatedCar.IsMotValid) //if valid after update get docs
                            {
                                documents = documentsInformation.GetCarExpiryInformation(accountId, car.carid, expenseItemDate);
                            }
                        }
                    }
               
                    foreach (IDutyOfCareDocument document in documents)
                    {
                        result = document.HasExpired(expenseItemDate.Date);

                        if (result.HasExpired)
                        {
                            result.carId = car.carid;
                            listOfDofCDocuments.Add(result);
                        }
                    }
                }
            }

            results.Add(listOfDofCDocuments, isManualDocumentValid);
            return results;
        }

        /// <summary>
        /// Checks for class 1 business related to the car is valid
        /// </summary>
        /// <param name="accountId">account Id of current user</param>
        /// <param name="activeCars">The cars to check duty of care results for</param>
        /// <param name="expenseItemDate">date of the expense item</param>
        /// <returns>car registration details</returns>
        public List<ListItem> Class1BusinessInformation(int accountId, List<cCar> activeCars, DateTime expenseItemDate)
        {
            List<ListItem> classOneBusinessInfo = new List<ListItem>();
           
            var class1BusinessInfo = new DutyOfCareDocumentsInformation();

            foreach (var car in activeCars)
            {
                if (!string.IsNullOrEmpty(car.registration))
                {
                    var result = class1BusinessInfo.GetClass1BusinessInformation(accountId, car.carid, expenseItemDate);

                    if (result != null)
                    {
                        classOneBusinessInfo.Add(new ListItem(car.carid.ToString(), result));
                    }
                }
            }

            return classOneBusinessInfo;

        }

        /// <summary>
        /// Checks to see if any MOT or Tax documents have expired.
        /// </summary>
        /// <param name="documents">A <see cref="List{T}"/>of <seealso cref="IDutyOfCareDocument"/></param>
        /// <param name="expenseItemDate">The date to check against.</param>
        /// <param name="car">An instance of <see cref="cCar"/>That needs to have documments validated</param>
        /// <returns>A cloned instance of <see cref="cCar"/>with the IsTaxValid and IsMotValid properties re-set based on the given documents.</returns>
        private cCar UpdateTaxAndMotExpiryOnVehicle(List<IDutyOfCareDocument> documents, DateTime expenseItemDate, cCar car)
        {
            var result = car.Clone() as cCar;
            if (result != null)
            {
                foreach (IDutyOfCareDocument document in documents.Where(d => d.GetType() == typeof(TaxDocument) || d.GetType() == typeof(MOTDocument)))
                {
                    if (document.HasExpired(expenseItemDate).HasExpired)
                    {
                        if (document is TaxDocument)
                        {
                            result.IsTaxValid = false;
                        }
                        else
                        {
                            result.IsMotValid = false;
                        }
                    }
                }
            }

            return result;
        }
        
    }
}
