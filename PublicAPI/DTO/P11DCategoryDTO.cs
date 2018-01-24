namespace PublicAPI.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class P11DCategoryDto
    {
        public int Id { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Too many characters, must not exceed: 50")]
        public string Name { get; set; }
    }
}