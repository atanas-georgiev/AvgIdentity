namespace AvgIdentity.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AvgIdentityUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public string PasswordAnswerHash { get; set; }

        [MaxLength(100)]
        public string PasswordQuestion { get; set; }
    }
}