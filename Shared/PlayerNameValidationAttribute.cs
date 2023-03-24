using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceTo21_Blazor.Shared
{
    public class PlayerNameValidationAttribute : ValidationAttribute
    {
        //private List<string> _invalidNames = new List<string>();
        //public List<string> InvalidNames { get { return _invalidNames; } private set { _invalidNames = value; } }

        //public string InvalidName { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            /*foreach (string name in InvalidNames)
            {
                if (name.ToLower() == value.ToString().ToLower())
                {
                    return new ValidationResult(ErrorMessage, new[] { context.MemberName });
                }
            }
            return null;*/
            /*if (InvalidName.ToLower() == value.ToString().ToLower())
                return new ValidationResult(ErrorMessage, new[] { context.MemberName });
            return null;*/
            foreach (string name in Pages.Index.InvalidNames)
            {
                if (name.ToLower() == value.ToString().ToLower())
                {
                    return new ValidationResult(ErrorMessage, new[] { context.MemberName });
                }
            }
            return null;
        }
    }
}
