namespace ApiRpc.Classes.Crud
{
    using System.Collections.Generic;

    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiCrud;

    using global::ApiRpc.Interfaces;
    public class CarApi : IDataAccess<Car>
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The crud API.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public CarApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }

        public List<Car> Create(List<Car> entities)
        {
            return this.crudApi.CreateCars(this.metaBase, this.accountId.ToString(), entities);
        }

        public Car Read(int entityId)
        {
            return this.crudApi.ReadCar(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public Car Read(long entityId)
        {
            return this.crudApi.ReadCar(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<Car> ReadAll()
        {
            return this.crudApi.ReadAllCar(this.metaBase, this.accountId.ToString());
        }

        public List<Car> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadCarByEsrId(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<Car> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<Car> Update(List<Car> entities)
        {
            return this.crudApi.UpdateCars(this.metaBase, this.accountId.ToString(), entities);
        }

        public Car Delete(long entityId)
        {
            return this.crudApi.DeleteCar(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public Car Delete(Car entity)
        {
            throw new System.NotImplementedException();
        }
    }
}