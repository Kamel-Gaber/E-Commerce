using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Models
{
    public class AddRoleToUserModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
