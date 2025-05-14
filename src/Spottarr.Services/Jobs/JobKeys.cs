using Quartz;

namespace Spottarr.Services.Jobs;

public static class JobKeys
{
    public static readonly JobKey ImportSpots = new("import-spots");
    public static readonly JobKey CleanUpSpots = new("clean-up-spots");
}