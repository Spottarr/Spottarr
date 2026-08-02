namespace Spottarr.Services.Models;

public enum SpotOperation
{
    /// <summary>
    /// Flags a spot to be reread from usenet.
    /// </summary>
    Reimport,

    /// <summary>
    /// Flags a spot to have its indexable attributes derived again.
    /// </summary>
    Reindex,
}
