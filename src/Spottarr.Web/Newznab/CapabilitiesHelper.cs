using System.Collections.ObjectModel;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Helpers;
using Spottarr.Web.Helpers;
using Spottarr.Web.Newznab.Models;

namespace Spottarr.Web.Newznab;

internal static class CapabilitiesHelper
{
    // https://github.com/Prowlarr/Prowlarr/blob/develop/src/NzbDrone.Core/Indexers/IndexerCapabilities.cs
    public static Capabilities GetCapabilities(Uri uri, Uri imageUri, string name, string version, int limit)
    {
        var caps = new Capabilities
        {
            ServerInfo = new ServerInfo
            {
                Title = name,
                Version = version,
                Tagline = name,
                Email = string.Empty,
                Host = uri.ToString(),
                Image = imageUri.ToString(),
                Type = name,
            },
            Limits = new Limits
            {
                Max = limit,
                Default = limit,
            },
            Registration = new Registration
            {
                Available = "no",
                Open = "no"
            },
            Searching = new Searching
            {
                Search = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                },
                TvSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,season,ep,year,imdbid",
                },
                MovieSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,season,ep,year,imdbid",
                },
                AudioSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,year",
                },
                PcSearch = new Search()
                {
                    Available = "no",
                    SupportedParams = "",
                },
                BookSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,title",
                }
            },
        };
        caps.Categories.Replace(GetCategories());

        return caps;
    }

    private static Collection<MainCategory> GetCategories()
    {
        var mainCats = new Dictionary<NewznabCategory, HashSet<NewznabCategory>>();

        var key = NewznabCategory.None;
        foreach (var cat in Enum.GetValues<NewznabCategory>())
        {
            if ((int)cat % 1000 == 0)
            {
                key = cat;
                mainCats[key] = [];
                continue;
            }

            mainCats[key].Add(cat);
        }

        return new Collection<MainCategory>(mainCats.Select(kvp =>
        {
            var cat = new MainCategory
            {
                Id = (int)kvp.Key,
                Name = kvp.Key.GetDisplayName()
            };

            cat.SubCategories.Replace(kvp.Value.Select(v => new Category
            {
                Id = (int)v,
                Name = v.GetDisplayName()
            }));

            return cat;
        }).ToList());
    }
}