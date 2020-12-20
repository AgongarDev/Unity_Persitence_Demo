using System.Collections.Generic;

namespace Persistence
{
    internal interface IEntity
    {
        Dictionary<string, object> Save();
        void Load(Dictionary<string, object> data);
    }
}