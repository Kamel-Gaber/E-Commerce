﻿using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Models.Account
{
    public class SignUpModel
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }
        [Required]
        [StringLength(256)]
        public string Password { get; set; }

    }
}
