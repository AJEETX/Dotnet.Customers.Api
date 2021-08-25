using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Helpers
{
    public class ValidDateOfBirthDataAnnotation : ValidationAttribute
    {
        public ValidDateOfBirthDataAnnotation()
        {
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var dt = (DateTime)value;
                if (dt < new DateTime(1920, 01, 01) || dt > DateTime.Now)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
