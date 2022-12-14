namespace UnitTestTraining;

public interface ISystemClock
{
    DateTime Now();
}

public class Entry
{
    public int Id { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class Training
{
    private readonly ISystemClock _systemClock;

    public Training(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    public string GenerateName()
    {
        var today = _systemClock.Now();

        if (today.Year < 2000)
        {
            throw new Exception("This is to old for me");
        }

        return "name" + today.ToString("yyyyMMdd");
    }

    public Dictionary<int, Entry> Dictionary = new();

    public Entry GetOrAdd(int id)
    {
        if (Dictionary.ContainsKey(id))
        {
            var entryInCache = Dictionary[id];

            if (entryInCache.ExpiresAt > _systemClock.Now())
            {
                return entryInCache;
            }
            else
            {
                Dictionary.Remove(id);
            }
        }

        var entry = new Entry
        {
            Id = id,
            ExpiresAt = _systemClock.Now() + TimeSpan.FromMinutes(5)
        };

        Dictionary.Add(id, entry);
        return entry;
    }

    public void Remove(int id)
    {
        Dictionary.Remove(id);
    }
}