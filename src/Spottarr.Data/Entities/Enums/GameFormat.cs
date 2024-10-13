using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory B
/// See: https://github.com/spotweb/spotweb/blob/develop/lib/SpotCategories.php
/// </summary>
public enum GameFormat
{
    [Display(Name = "ISO")]
    Iso = 0,
    [Display(Name = "Rip")]
    Rip = 1,
    [Display(Name = "Retail")]
    Retail = 2,
    [Display(Name = "DLC")]
    Dlc = 3,
    [Display(Name = "Unknown"), Obsolete("Unused")]
    Unused1 = 4,
    [Display(Name = "Patch")]
    Patch = 5,
    [Display(Name = "Crack")]
    Crack = 6
}