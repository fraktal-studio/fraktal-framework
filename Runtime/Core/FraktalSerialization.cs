using System;
using System.Collections.Generic;
using System.Reflection;
using Fraktal.Framework.DI.Injector.Attributes;

namespace Fraktal.Framework.Core
{
    public class FraktalSerialization
    {
        public static void Serialize(IFraktalSerializable target)
        {
            var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<int> indecies = new ();
            List<UnityEngine.Object> values = new();
            foreach (var info in fields)
            {
                var dependency = info.GetCustomAttribute<DependencyAttribute>();
                if (dependency == null)
                    continue;
                var value = info.GetValue(target);
                if (value is not UnityEngine.Object obj)
                    continue;
                values.Add(obj);
                indecies.Add(values.Count-1);
            }

            target.FraktalSerializedObjects = values.ToArray();
        }

        public static void Deserialize(IFraktalSerializable target)
        {
            if (target.FraktalSerializedObjects == null)
                return;
            var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            int index = 0;
            foreach (var info in fields)
            {
                var dependency = info.GetCustomAttribute<DependencyAttribute>();
                if (dependency == null)
                    continue;
                int currentIndex = index;
                index++;
                
                if (target.FraktalSerializedObjects.Length <= currentIndex)
                    return;
                if (target.FraktalSerializedObjects[currentIndex] == null)
                    continue;
                
                var value = target.FraktalSerializedObjects[currentIndex];
                if (!info.FieldType.IsAssignableFrom(value.GetType()))
                    continue;
                info.SetValue(target, value);

            }
        }
    }
}