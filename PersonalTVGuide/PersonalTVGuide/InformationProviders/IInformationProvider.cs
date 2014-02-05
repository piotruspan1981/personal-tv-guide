using System.Collections.Generic;
using PersonalTVGuide.TVShowObjects;

namespace PersonalTVGuide.InformationProviders
{
    public interface IInformationProvider
    {
        List<Show> GetShows(string search);
        Show GetFullDetails(int showId);
    }
}