using System.Collections.Generic;

namespace Fraktal.Framework.DI.Injector.Services
{
    public interface IChangesTracker
    {
        public void AddChanges(UnityEngine.Object obj);
        public void RemoveChanges(UnityEngine.Object obj);
        public bool HasMadeChanges(UnityEngine.Object obj);
        public ICollection<UnityEngine.Object> GetChanges();
    }
}