using System.Collections.Generic;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    public class ChangesTracker : IChangesTracker
    {
        private HashSet<UnityEngine.Object> changedObjects = new HashSet<UnityEngine.Object>();

        

        public void AddChanges(Object obj)
        {
            changedObjects.Add(obj);
        }

        public void RemoveChanges(Object obj)
        {
            changedObjects.Remove(obj);
        }

        public bool HasMadeChanges(Object obj)
        {
            return changedObjects.Remove(obj);
        }

        public ICollection<Object> GetChanges()
        {
            return changedObjects;
        }
    }
}