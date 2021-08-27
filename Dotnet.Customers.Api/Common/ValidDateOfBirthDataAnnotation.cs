using System;
using System.ComponentModel.DataAnnotations;

namespace Dotnet.Customers.Api.Common
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