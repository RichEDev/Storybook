using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EsrFramework.Models.Mapping;

namespace EsrFramework.Models
{
    public partial class EsrContext : DbContext
    {
        static EsrContext()
        {
            Database.SetInitializer<EsrContext>(null);
        }

        public EsrContext()
            : base("Name=EsrContext")
        {
        }

        public DbSet<Address> addresses { get; set; }
        public DbSet<AddressEsrAllocation> AddressEsrAllocations { get; set; }
        public DbSet<CarAssignmentNumberAllocation> CarAssignmentNumberAllocations { get; set; }
        public DbSet<Car> cars { get; set; }
        public DbSet<EmployeeRole> employee_roles { get; set; }
        public DbSet<EmployeeAccessRole> employeeAccessRoles { get; set; }
        public DbSet<EmployeeHomeAddress> employeeHomeAddresses { get; set; }
        public DbSet<EmployeeHomeLocation> employeeHomeLocations { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<EmployeeWorkAddress> EmployeeWorkAddresses { get; set; }
        public DbSet<EmployeeWorkLocation> employeeWorkLocations { get; set; }
        public DbSet<EsrAssignment> esr_assignments { get; set; }
        public DbSet<EsrAddress> ESRAddresses { get; set; }
        public DbSet<EsrAssignmentCosting> ESRAssignmentCostings { get; set; }
        public DbSet<EsrElementField> ESRElementFields { get; set; }
        public DbSet<EsrElement> ESRElements { get; set; }
        public DbSet<EsrElementSubcat> ESRElementSubcats { get; set; }
        public DbSet<EsrLocation> ESRLocations { get; set; }
        public DbSet<EsrOrganisation> ESROrganisations { get; set; }
        public DbSet<EsrPerson> ESRPersons { get; set; }
        public DbSet<EsrPhone> ESRPhones { get; set; }
        public DbSet<EsrPosition> ESRPositions { get; set; }
        public DbSet<EsrTrust> esrTrusts { get; set; }
        public DbSet<EsrVehicle> ESRVehicles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AddressEsrAllocationMap());
            modelBuilder.Configurations.Add(new CarAssignmentNumberAllocationMap());
            modelBuilder.Configurations.Add(new CarMap());
            modelBuilder.Configurations.Add(new EmployeeRoleMap());
            modelBuilder.Configurations.Add(new EmployeeAccessRoleMap());
            modelBuilder.Configurations.Add(new EmployeeHomeAddressMap());
            modelBuilder.Configurations.Add(new EmployeeHomeLocationMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new EmployeeWorkAddressMap());
            modelBuilder.Configurations.Add(new EmployeeWorkLocationMap());
            modelBuilder.Configurations.Add(new EsrAssignmentMap());
            modelBuilder.Configurations.Add(new EsrAddressMap());
            modelBuilder.Configurations.Add(new EsrAssignmentCostingMap());
            modelBuilder.Configurations.Add(new EsrElementFieldMap());
            modelBuilder.Configurations.Add(new EsrElementMap());
            modelBuilder.Configurations.Add(new EsrElementSubcatMap());
            modelBuilder.Configurations.Add(new EsrLocationMap());
            modelBuilder.Configurations.Add(new EsrOrganisationMap());
            modelBuilder.Configurations.Add(new EsrPersonMap());
            modelBuilder.Configurations.Add(new EsrPhoneMap());
            modelBuilder.Configurations.Add(new EsrPositionMap());
            modelBuilder.Configurations.Add(new EsrTrustMap());
            modelBuilder.Configurations.Add(new EsrVehicleMap());
        }
    }
}
