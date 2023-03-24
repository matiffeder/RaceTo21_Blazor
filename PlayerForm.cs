using RaceTo21_Blazor.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceTo21_Blazor
{
    public class PlayerForm
    {
        public int? Num { get; set; } = 2;
        //public List<string> NameL = new();
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Amount of characters must between 1 to 20.")]
        [PlayerNameValidation(ErrorMessage = "Name has been taken")]
        public string Name1 { get; set; }
        //public string[] Name { get; set; } = new string[8];

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
