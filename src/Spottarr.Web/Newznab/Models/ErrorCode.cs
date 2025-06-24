using System.ComponentModel.DataAnnotations;

namespace Spottarr.Web.Newznab.Models;

internal enum ErrorCode
{
    None = 0,

    [Display(Name = "Incorrect user credentials")]
    IncorrectUserCredentials = 100,

    [Display(Name = "Account suspended")] AccountSuspended = 101,

    [Display(Name = "Insufficient privileges/not authorized")]
    InsufficientPrivileges = 102,

    [Display(Name = "Registration denied")]
    RegistrationDenied = 103,

    [Display(Name = "Registrations are closed")]
    RegistrationsClosed = 104,

    [Display(Name = "Invalid registration (Email Address Taken)")]
    InvalidRegistrationEmailTaken = 105,

    [Display(Name = "Invalid registration (Email Address Bad Format)")]
    InvalidRegistrationEmailBadFormat = 106,

    [Display(Name = "Registration Failed (Data error)")]
    RegistrationFailedDataError = 107,

    [Display(Name = "Missing parameter")] MissingParameter = 200,

    [Display(Name = "Incorrect parameter")]
    IncorrectParameter = 201,

    [Display(Name = "No such function (Function not defined in this specification)")]
    NoSuchFunction = 202,

    [Display(Name = "Function not available. (Optional function is not implemented)")]
    FunctionNotAvailable = 203,

    [Display(Name = "No such item")] NoSuchItem = 300,

    [Display(Name = "Item already exists")]
    ItemAlreadyExists = 310,

    [Display(Name = "Failed to load NZB")] FailedToLoadNzb = 600,

    [Display(Name = "NZB is duplicate")] NzbDuplicate = 601,

    [Display(Name = "NZB is for a non-existent group")]
    NzbNonExistentGroup = 602,

    [Display(Name = "NZB failed to write to disk")]
    NzbFailedToWriteToDisk = 603,

    [Display(Name = "Unknown error")] UnknownError = 900,

    [Display(Name = "API Disabled")] ApiDisabled = 910
}