using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Drawers
{
      public class TypeDrawer
    {
        private Dictionary<Type, Type[]> cachedTypes;
        
        private Dictionary<Type, string[]> cachedNames;
        
        public Type[] GetOrRegisterTypes<T>()
        {
            if (cachedTypes == null)
                cachedTypes = new ();
            
            if (!cachedTypes.ContainsKey(typeof(T)))
            {
                cachedTypes[typeof(T)] = TypeCache.GetTypesDerivedFrom<T>().ToArray();
            }
            return cachedTypes[typeof(T)];
        }

        private string[] GetOrRegisterNames(Type type)
        {
            if (cachedNames == null)
                cachedNames = new ();
            if (cachedNames.ContainsKey(type))
                return cachedNames[type];
            Type[] types = GetOrRegisterTypes(type);
            cachedNames[type] = types.Select(t => t.Name).ToArray();
            return cachedNames[type];
        }

        /// <para>
        /// The filtering ensures that only plain C# types suitable for SerializeReference serialization 
        /// are included in the results.
        /// </para>
        /// </remarks>
        private Type[] GetOrRegisterTypes(Type type)
        {
            if (cachedTypes == null)
                cachedTypes = new ();
            
            if (!cachedTypes.ContainsKey(type))
            {
                cachedTypes[type] = TypeCache.GetTypesDerivedFrom(type).Where(t => !typeof(UnityEngine.Object).IsAssignableFrom(t)).ToArray();
            }
            return cachedTypes[type];
        }

        /// <summary>
        /// Renders a type selection dropdown for a runtime type constraint with automatic instantiation.
        /// </summary>
        /// <param name="type">The constraint type that defines which implementations can be selected.</param>
        /// <param name="current">The current instance, used to determine the selected type in the dropdown.</param>
        /// <returns>
        /// A new instance of the selected type, or the original instance if the selection 
        /// was invalid or the type lacks a parameterless constructor.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides runtime type selection capabilities by:
        /// </para>
        /// <list type="number">
        /// <item>Discovering all types that implement the specified constraint</item>
        /// <item>Presenting them in a dropdown with user-friendly names</item>
        /// <item>Attempting to instantiate the selected type via reflection</item>
        /// <item>Returning the new instance or falling back to the current value</item>
        /// </list>
        /// <para>
        /// The method requires selected types to have parameterless constructors and will 
        /// maintain the current selection if instantiation fails.
        /// </para>
        /// </remarks>
        public object TypeField(Type type, object current)
        {
            Type[] types = GetOrRegisterTypes(type);
            string[] names = GetOrRegisterNames(type);
            
            
            return TypeField(current, names, types);
        }

        /// <summary>
        /// Renders a type selection dropdown for a generic type constraint with automatic instantiation.
        /// </summary>
        /// <typeparam name="T">The generic type constraint (interface, abstract class, or base class).</typeparam>
        /// <param name="factory">The current instance, used to determine the selected type in the dropdown.</param>
        /// <returns>
        /// A new instance of the selected type cast to <typeparamref name="T"/>, or the original 
        /// instance if the selection was invalid or instantiation failed.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This generic overload provides compile-time type safety while maintaining the same 
        /// functionality as the runtime version. It's preferred when the constraint type is 
        /// known at compile time.
        /// </para>
        /// <para>
        /// The method leverages the cached type discovery system and delegates to the 
        /// core TypeField implementation for consistency.
        /// </para>
        /// </remarks>
        public T TypeField<T>(T factory)
        {

            Type[] types = GetOrRegisterTypes<T>();

            return (T) TypeField(factory, GetOrRegisterNames(typeof(T)), types);
        }

        /// <summary>
        /// Core implementation for rendering type selection dropdown with automatic instantiation logic.
        /// </summary>
        /// <param name="current">The current instance, used to determine the selected index in the dropdown.</param>
        /// <param name="names">Array of user-friendly names to display in the dropdown.</param>
        /// <param name="types">Array of types corresponding to the names array.</param>
        /// <returns>
        /// A new instance of the selected type, or the original instance if the selection 
        /// was invalid, unchanged, or the type lacks a parameterless constructor.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the core dropdown rendering and instantiation logic:
        /// </para>
        /// <list type="number">
        /// <item>Determines the current selection index based on the current instance type</item>
        /// <item>Renders a popup dropdown with calculated width based on available space</item>
        /// <item>Detects selection changes and attempts instantiation via reflection</item>
        /// <item>Falls back to the current instance if instantiation fails</item>
        /// </list>
        /// <para>
        /// The method uses Unity's EditorGUILayout.Popup with calculated width to ensure 
        /// proper layout within inspector contexts. Width calculation considers label 
        /// space and applies a small margin for visual clarity.
        /// </para>
        /// <para>
        /// Instantiation requires types to have public parameterless constructors. Types 
        /// without suitable constructors will maintain their current selection without 
        /// creating new instances.
        /// </para>
        /// </remarks>
        private object TypeField(object current, string[] names, Type[] types)
        {
            Type currentType = current?.GetType();
            int currentIndex = -1;
            if (currentType != null)
                for (int i = 0; i < types.Length; i++)
                {
                    if (currentType == types[i])
                    {
                        currentIndex = i;
                        
                    }
                }
            float fieldWidth = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth-10;
            int newIndex = EditorGUILayout.Popup(currentIndex, names, GUILayout.Width(fieldWidth));
            
            if (newIndex == -1)
                return default;
            
            Type newType = types[newIndex];
            var ctor = newType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                return current;
            } 
            
            return ctor.Invoke(null);
        }
    }
}