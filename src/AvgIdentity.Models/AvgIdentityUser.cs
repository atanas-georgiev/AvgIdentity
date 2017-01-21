namespace AvgIdentity.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AvgIdentityUser : IdentityUser
    {
        public const int FirstNameMaxLen = 100;
        public const int LastNameMaxLen = 100;
        public const int PasswordQuestionMaxLen = 100;

        [MaxLength(FirstNameMaxLen)]
        public string FirstName { get; set; }

        [MaxLength(LastNameMaxLen)]
        public string LastName { get; set; }

        public string PasswordAnswerHash { get; set; }

        [MaxLength(PasswordQuestionMaxLen)]
        public string PasswordQuestion { get; set; }
    }
}