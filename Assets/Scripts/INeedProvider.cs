using System.Collections.Generic;

public interface INeedProvider
{
    List<Need.NeedType> NeedTypes { get; }
    Task CreateSatisfyNeedTask(Unit unit, Need needToSatisfy);
}
