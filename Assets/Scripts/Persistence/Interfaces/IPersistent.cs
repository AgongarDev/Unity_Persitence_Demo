using System.Collections.Generic;

namespace Persistence
{
    public interface IPersistent<T> where T : class
    {
        object Insert(T Tpersistent);
        object Update(T Tpersistent);
        object InsertOrUpdate(T TPersistent);
        int GetID(object name);
        List<T> GetAll();
        void Delete(int id);
    }
}