using Dotnet.Customers.Api.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Domain
{
    public class Customer
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
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
