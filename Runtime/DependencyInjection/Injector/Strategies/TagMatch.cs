using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    public class TagMatch : IFieldStrategy
    {
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            var attr = field.GetAttribute();
            if (attr is not ByTagAttribute tagAttr)
                return false;

            if (obj is GameObject go)
            {
                if (!go.CompareTag(tagAttr.Tag))
                    return false;
                if (!field.IsAssignable(go))
                    return false;
                
                field.SetValue(go, instance);
                return true;
            }

            if (obj is not Component comp)
                return false;

            if (!comp.CompareTag(tagAttr.Tag))
                return false;

            if (!field.IsAssignable(comp))
                return false;
            
            field.SetValue(comp, instance);
            return true;
        }
    }
}