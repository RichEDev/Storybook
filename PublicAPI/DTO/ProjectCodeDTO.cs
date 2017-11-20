using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicAPI.DTO
{
    public class ProjectCodeDto
    {
        public int Id { get; set; }

        [Required]
        public bool Archived { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Too many characters, must not exceed: 50")]
        public string Name { get; set; }

        [Required, MaxLength(4000, ErrorMessage = "Too many characters, must not exceed: 4000")]
        public string Description { get; set; }

        public IEnumerable<KeyValuePair<int, object>> UserDefinedFieldValues { get; set; }
    }
}