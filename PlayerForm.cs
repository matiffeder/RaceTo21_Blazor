using RaceTo21_Blazor.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceTo21_Blazor
{
    //the model for editform
    public class PlayerForm
    {
        //the variable bind in editform that use PlayerForm model
        public int? Num { get; set; } = 2;

        //if the field is empty
        [Required(ErrorMessage = "Name is required")]
        //charachters num should not bigger than 20 smaller than 1
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        //the component that I wrote in the app to check invalid name and show error in editform
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        //the variable bind in the editform that use PlayerForm model and for checking error
        public string Name1 { get; set; }
        //don't know how to use a array for the names
        //public string[] Name { get; set; } = new string[8];
        //public List<string> NameL = new();

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name2 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name3 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name4 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name5 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name6 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name7 { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name8 { get; set; }

    }
}
