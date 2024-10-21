using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services.Creational;
public class StateBuilder
{
    private readonly CacheService _cacheService;
    public StateBuilder(CacheService cacheService)
    {
        _cacheService = cacheService;
    }
    //TODO -> ZASTANOWIĆ SIĘ NAD IMPLEMENTACJĄ STATE BUILDERA I CLONEBUILDERA
}

