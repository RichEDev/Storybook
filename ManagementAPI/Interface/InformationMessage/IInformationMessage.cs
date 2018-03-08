namespace ManagementAPI.Interface.InformationMessage
{
    using System;

    public interface IInformationMessage
    {
        int InformationId { get; set; }

        string Title { get; set; }
        
        string Message { get; set; }
        
        int AdministratorId { get; set; }
        
        DateTime DateAdded { get; set; }
        
        int DisplayOrder { get; set; }

        bool Deleted { get; set; }
    }
}
