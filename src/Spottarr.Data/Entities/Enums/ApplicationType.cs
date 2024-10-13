using System.ComponentModel.DataAnnotations;

namespace Spottarr.Data.Entities.Enums;

/// <summary>
/// Subcategory Z
/// See: https://github.com/spotweb/spotweb/blob/develop/lib/SpotCategories.php
/// </summary>
public enum ApplicationType
{
    [Display(Name = "Everything")]
    Everything = 0,
}