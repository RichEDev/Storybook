

namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    using SpendManagementApi.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using SpendManagementLibrary;

    /// <summary>
    /// A representation of Journey Steps for expenses.
    /// </summary>
    public class JourneyStep : BaseExternalType, IApiFrontForDbObject<cJourneyStep, JourneyStep>
    {
        /// <summary>
        /// The expense id associated with 
        /// </summary>
        public int ExpenseId { get;  set; }

        /// <summary>
        /// The start location.
        /// </summary>
        public Address StartLocation { get; set; }

        /// <summary>
        /// The end location.
        /// </summary>
        public Address EndLocation { get; set; }

        /// <summary>
        /// The number of miles.
        /// </summary>
        public decimal NumberOfMiles { get; set; }

        /// <summary>
        /// The recorded amount of miles.
        /// </summary>
        public decimal RecordedMiles { get; set; }

        /// <summary>
        /// The number of passengers.
        /// </summary>
        [Range(0, byte.MaxValue)]
        public byte NumberOfPassengers { get; set; }

        /// <summary>
        /// The passenger details on the journey step
        /// </summary>
        public List<Passenger> Passengers { get; set;}
 
        /// <summary>
        /// 
        /// </summary>
        public byte StepNumber { get;  set; }

        /// <summary>
        /// The current step number of the journey.
        /// </summary>
        public bool ExceededRecommendedMileage { get;  set; }

        /// <summary>
        /// Comment for when the recommended mileage has been exceeded.
        /// </summary>
        public string ExceededRecommendedMileageComment { get;  set; }

        /// <summary>
        /// The number of miles entered by the user. Does not takes into account reduction for home to office rules etc
        /// </summary>
        public decimal NumberOfActualMiles { get;  set; }

        /// <summary>
        /// Check to see if any heavy bulky equipment mileage is associated
        /// </summary>
        public bool HeavyBulkyEquipment { get;  set; }


        /// <summary>
        /// Constructor via an expense id.
        /// </summary>
        /// <param name="expenseId"></param>
        public JourneyStep(int expenseId)
        {
            this.ExpenseId = expenseId;
        }

        /// <summary>
        /// Converts a spend management journey step to an api version.
        /// </summary>
        /// <param name="dbType">The spend management instance.</param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>An api version of the journey step.</returns>
        public JourneyStep From(cJourneyStep dbType, IActionContext actionContext)
        {
            this.EndLocation = new Address().From(dbType.endlocation, actionContext);
            this.StartLocation = new Address().From(dbType.startlocation, actionContext);
            this.ExpenseId = dbType.expenseid;
            this.HeavyBulkyEquipment = dbType.heavyBulkyEquipment;
            this.NumberOfActualMiles = dbType.NumActualMiles;
            this.NumberOfMiles = dbType.nummiles;
            this.NumberOfPassengers = dbType.numpassengers;
            this.RecordedMiles = dbType.recmiles;
            this.StepNumber = dbType.stepnumber;

            var passengers = new List<Passenger>();

            foreach (var passenger in dbType.passengers)
            {
                var journeyPassenger = new Passenger().From(passenger, actionContext);
                passengers.Add(journeyPassenger);
            }

            this.Passengers = passengers;

            return this;
        }

        /// <summary>
        /// Converts an api journey step to a spendmanagement version.
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="actionContext">The action context.</param>
        /// <returns>A spend management journey step.</returns>
        public cJourneyStep To(IActionContext actionContext)
        {
            var journeyPassengerList = new List<SpendManagementLibrary.Mileage.Passenger>();

            if (this.Passengers != null)
            {
                foreach (var journeyPassenger in Passengers)
                {
                    journeyPassengerList.Add(journeyPassenger.To(actionContext));
                }
            }

            var passengersArray = journeyPassengerList.ToArray();

            SpendManagementLibrary.Addresses.Address startLocation = null;
            SpendManagementLibrary.Addresses.Address endLocation = null;

            if (this.StartLocation != null)
            {
               startLocation = this.StartLocation.To(actionContext);
            }

            if (this.EndLocation != null)
            {
                endLocation = this.EndLocation.To(actionContext);
            }

            return new cJourneyStep(
              this.ExpenseId,
              startLocation,
              endLocation,
              this.NumberOfMiles,
              this.RecordedMiles,
              this.NumberOfPassengers,
              this.StepNumber,
              this.NumberOfActualMiles,
              this.HeavyBulkyEquipment)
            {
                passengers = passengersArray
            };
        }
    }
}