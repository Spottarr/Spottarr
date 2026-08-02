namespace Spottarr.Services.Spotnet;

public enum SpotReadOutcome
{
    /// <summary>
    /// The article was read and the spot was updated.
    /// </summary>
    Read,

    /// <summary>
    /// The article can not be read, retrying will not help.
    /// </summary>
    Unavailable,

    /// <summary>
    /// The article could not be read right now, retrying may help.
    /// </summary>
    Failed,
}
