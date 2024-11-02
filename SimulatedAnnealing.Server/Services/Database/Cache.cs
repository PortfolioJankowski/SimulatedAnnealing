using SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public static class Cache
{
    private static readonly object CacheLock = new object();
    private static IQueryable<Voivodeship>? _voivodeshipCache;
    private static IQueryable<County>? _countiesCache;
    private static IQueryable<Neighbor>? _neighborsCache;

    public static void SetVoivodeshipIQueryable(IQueryable<Voivodeship> voivodeship)
    {
        lock (CacheLock)
        {
            _voivodeshipCache = voivodeship;
        }
    }

    public static IQueryable<County>? GetCountiesIQueryable()
    {
        lock (CacheLock)
        {
            return _countiesCache?.AsQueryable();
        }
    }

    public static void SetCountiesIQueryable(IQueryable<County> counties)
    {
        lock (CacheLock)
        {
            _countiesCache = counties;
        }
    }

    public static IQueryable<Neighbor>? GetNeighborsIQueryable()
    {
        lock (CacheLock)
        {
            return _neighborsCache?.AsQueryable();
        }
    }

    public static void SetNeighborsIQueryable(IQueryable<Neighbor> neighbors)
    {
        lock (CacheLock)
        {
            _neighborsCache = neighbors;
        }
    }

    public static IQueryable<Voivodeship>? GetVoivodeshipQueryable() 
    {
        lock (CacheLock)
        {
            return _voivodeshipCache?.AsQueryable();
        }
    }
}