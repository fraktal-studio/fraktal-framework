using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    public class SelfOrChildrenMatch : IFieldStrategy
    {
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            if (!field.IsAssignable(obj))
                return false;
            
            if (instance is not Component component)
                return false;

            if (!context.Services.Get(out IHierarchyTracker tracker))
                return false;
            
            if (tracker.Current == component.gameObject)
            {
                field.SetValue(component, instance);
                return true;
            }

            if (tracker.IsParent(component.gameObject))
            {
                field.SetValue(component, instance);
            }
            

            return false;
        }
    }
}