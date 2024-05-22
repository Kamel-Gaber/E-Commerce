using E_CommerceApi.Models.Sales;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_CommerceApi.Models.DbContext
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [JsonIgnore]
        public virtual List<Order>? Order { get; set; }
    }
}
