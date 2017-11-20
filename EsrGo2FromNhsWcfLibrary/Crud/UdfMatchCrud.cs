namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The UDF MATCH crud.
    /// </summary>
    public class UdfMatchCrud : EntityBase, IDataAccess<UserDefinedMatchField>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UdfMatchCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public UdfMatchCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        public List<UserDefinedMatchField> Create(List<UserDefinedMatchField> entities)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadByEsrId(long esrId)
        {
            throw new System.NotImplementedException();
        }

        public List<UserDefinedMatchField> ReadSpecial(string reference)
        {
            return this.EsrApiHandler.Execute<UserDefinedMatchField>(DataAccessMethod.ReadSpecial, reference);
        }

        public List<UserDefinedMatchField> Update(List<UserDefinedMatchField> entities)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedMatchField Delete(UserDefinedMatchField entity)
        {
            throw new System.NotImplementedException();
        }
    }
}