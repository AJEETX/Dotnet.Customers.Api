using System;
using System.ComponentModel.DataAnnotations;

namespace Dotnet.Customers.Api.Dtos
{
    public class CustomerDto
    {
        /// <summary>
        /// First Name
        /// </summary>
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please use only Letters")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "First Name has to be between 3 and 250 Characters only")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please use only Letters")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Last Name has to be between 3 and 250 Characters only")]
        public string LastName { get; set; }

        /// <summary>
        /// Date Of Birth
        /// </summary
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }
    }
}