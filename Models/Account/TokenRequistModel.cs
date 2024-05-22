using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Models.Account
{
    public class TokenRequistModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
