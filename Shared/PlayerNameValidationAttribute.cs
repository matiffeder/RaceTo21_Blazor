using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceTo21_Blazor.Shared
{
    //attribute for validation in PlayerForm model, need using System.ComponentModel.DataAnnotations
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
            //check the name if invalid
            foreach (string name in Pages.Index.InvalidNames)
            {
                //use same case to check  the name
                if (name.ToLower() == value.ToString().ToLower())
                {
                    //if found invalid then return error message
                    return new ValidationResult(ErrorMessage, new[] { context.MemberName });
                }
            }
            //if no match then return nothing (no errormessage)
            return null;
        }
    }
}
