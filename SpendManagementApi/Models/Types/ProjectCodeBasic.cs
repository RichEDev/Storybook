namespace SpendManagementApi.Models.Responses
{
    using System.ComponentModel.DataAnnotations;
    using BusinessLogic.ProjectCodes;


    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    /// <summary>
    /// The project code basic class. 
    /// </summary>
    public class ProjectCodeBasic
    {
        /// <summary>
        /// The Id of this object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name / label for this project code object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this project code object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }
        
        public ProjectCodeBasic From(IProjectCodeWithUserDefinedFields original)
        {
            this.Id = original.Id;
            this.Label = original.Name;
            this.Description = original.Description;
            
            return this;
        }

    }
}