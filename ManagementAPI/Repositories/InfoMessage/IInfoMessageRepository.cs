namespace ManagementAPI.Repositories.InfoMessage
{
    using ManagementAPI.Models;
    using System.Collections.Generic;

    public interface IInfoMessageRepository
    {
        InformationMessage Get(int id);

        List<InformationMessage> GetAll();
        
        bool Save(InformationMessage infoMessage);

        bool Delete(int id);
    }
}