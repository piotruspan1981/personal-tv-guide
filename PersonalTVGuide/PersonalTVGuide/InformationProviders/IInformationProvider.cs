using System.Collections.Generic;
using PersonalTVGuide.TVShowObjects;

namespace PersonalTVGuide.InformationProviders
{
    public interface IInformationProvider
    {
        List<TVRageShow> GetShows(string search);
        TVRageShow GetFullDetails(int showId);
    }
}