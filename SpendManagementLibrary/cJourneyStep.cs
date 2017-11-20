namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Addresses;
    using Mileage;

    [Serializable()]
    public class cJourneyStep
    {
        private int nExpenseid;
        private Address clsStartLocation;
        private Address clsEndLocation;
        private decimal dNumMiles;
        private decimal dRecMiles;
        private byte bNumPassengers;
        private byte bStepNumber;
        private bool bExceededRecommendedMileage;
        private string sExceededRecommendedMileageComment;
        private decimal dNumActualMiles;
        private bool bHeavyBulkyEquipment;
        private Passenger[] _passengers;

        public cJourneyStep(int expenseid, Address startlocation, Address endlocation, decimal nummiles, decimal recmiles, byte numpassengers, byte stepnumber, decimal numactualmiles, bool heavyBulkyEquipment)
        {
            this.nExpenseid = expenseid;
            this.clsStartLocation = startlocation;
            this.clsEndLocation = endlocation;
            this.dNumMiles = nummiles;
            this.dRecMiles = recmiles;
            this.bNumPassengers = numpassengers;
            this.bStepNumber = stepnumber;
            this.dNumActualMiles = numactualmiles;
            this.bHeavyBulkyEquipment = heavyBulkyEquipment;
        }

        #region properties

        /// <summary>
        /// Gets the ID of the expense item that the journey step is associated to
        /// </summary>
        public int expenseid
        {
            get { return this.nExpenseid; }
        }

        /// <summary>
        /// Gets or sets the start location address for the journey step
        /// </summary>
        public Address startlocation
        {
            get { return this.clsStartLocation; }
            set { this.clsStartLocation = value; }
        }

        /// <summary>
        /// Gets or sets the enbd location address for the journey step.
        /// </summary>
        public Address endlocation
        {
            get { return this.clsEndLocation; }
            set { this.clsEndLocation = value; }
        }

        /// <summary>
        /// Gets or sets the number of miles for the journey step.
        /// </summary>
        public decimal nummiles
        {
            get { return this.dNumMiles; }
            set { this.dNumMiles = value; }
        }

        /// <summary>
        /// Gets or sets the recorded miles for the journey step.
        /// </summary>
        public decimal recmiles
        {
            get { return this.dRecMiles; }
            set { this.dRecMiles = value; }
        }

        /// <summary>
        /// Gets or sets the number of passenger for the journey step.
        /// </summary>
        public byte numpassengers
        {
            get { return this.bNumPassengers; }
            set { this.bNumPassengers = value; }
        }

        /// <summary>
        /// Gets the step number for the entire journey.
        /// </summary>
        public byte stepnumber
        {
            get { return this.bStepNumber; }
        }

        /// <summary>
        /// The number of miles entered by the user. Does not takes into account reduction for home to office rules etc
        /// </summary>
        public decimal NumActualMiles
        {
            get { return this.dNumActualMiles; }
            set { this.dNumActualMiles = value; }
        }

        /// <summary>
        /// Check to see if any heavy bulky equipment mileage is associated
        /// </summary>
        public bool heavyBulkyEquipment
        {
            get { return this.bHeavyBulkyEquipment; }
            internal set
            {
                this.bHeavyBulkyEquipment = value;
            }
        }

        public Passenger[] passengers { get; set; }

        /// <summary>
        /// Gets and sets the Official journey flag used for Junior Doctor Rotation home to office rule.
        /// </summary>
        public bool OfficialJourney { get; set; }

        #endregion

        /// <summary>
        /// Copy an instance of the object setting any ID's to zero.
        /// </summary>
        /// <param name="officeAddresses">
        /// The office Addresses for the employee.
        /// </param>
        /// <param name="workAdressId">
        /// The work Adress Id stored against the expense item.
        /// </param>
        /// <param name="item">
        /// The cloned expense item.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="cJourneyStep"/>
        /// </returns>
        public cJourneyStep Clone(List<Address> officeAddresses, int workAdressId, ref cExpenseItem item)
        {
            // validate if the jouney step containing work address is valid for current date - if not set it to default address for that date
            if (this.startlocation.Identifier == workAdressId
                && !officeAddresses.Any(x => x.Identifier == this.startlocation.Identifier))
            {
                this.startlocation = officeAddresses[0];
                item.WorkAddressId = this.startlocation.Identifier;
            }

            if (this.endlocation.Identifier == workAdressId
                && !officeAddresses.Any(x => x.Identifier == this.endlocation.Identifier))
            {
                this.endlocation = officeAddresses[0];
                item.WorkAddressId = this.endlocation.Identifier;
            }

            return new cJourneyStep(
                0,
                this.startlocation,
                this.endlocation,
                this.nummiles,
                this.recmiles,
                this.numpassengers,
                this.stepnumber,
                this.NumActualMiles,
                this.heavyBulkyEquipment);
        }
    }
}