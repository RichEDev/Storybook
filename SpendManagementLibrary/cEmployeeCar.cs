
namespace SpendManagementLibrary
{
    using Infragistics.Web.UI;

    /// <summary>
    /// The employee car.
    /// </summary>
    public class cEmployeeCar
    {
        /// <summary>
        /// Gets or sets the make.
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        public string Registration { get; set; }

        /// <summary>
        /// Gets or sets the car type.
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// Gets or sets the enginesize.
        /// </summary>
        public int Enginesize { get; set; }

        /// <summary>
        /// Gets or sets the defaultunit.
        /// </summary>
        public string Defaultunit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="cEmployeeCar"/> class.
        /// </summary>
        /// <param name="make">
        /// The make.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <param name="carType">
        /// The car type.
        /// </param>
        /// <param name="enginesize">
        /// The enginesize.
        /// </param>
        /// <param name="defaultunit">
        /// The defaultunit.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        public cEmployeeCar(string make, string model, string registration, string carType, int enginesize, string defaultunit, bool approved, bool active)
        {
            this.Make = make;
            this.Registration = registration;
            this.Model = model;
            this.CarType = carType;
            this.Enginesize = enginesize;
            this.Defaultunit = defaultunit;
            this.Approved = approved;
            this.Active = active;
        }
    }
}
