namespace ManagementAPI.Models
{
    using System;
    using ManagementAPI.Interface.InformationMessage;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class InformationMessage : IInformationMessage
    {
        public int InformationId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string MobileMessage { get; set; }

        public int AdministratorId { get; set; }

        public DateTime DateAdded { get; set; }

        public int DisplayOrder { get; set; }

        public bool Deleted { get; set; }
    }
}