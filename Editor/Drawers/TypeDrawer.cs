using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Drawers
{
    /// <summary>
    /// Utility class for rendering type selection dropdown controls in Unity Editor interfaces.
    /// Provides cached type discovery and user-friendly type selection capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class serves as a specialized GUI utility for creating type selection interfaces 
    /// in Unity Editor windows and inspectors. It leverages Unity's TypeCache system for 
    /// performance and provides caching mechanisms to avoid repeated type discovery operations.
    /// </para>
    /// <para>
    /// Key features include:
    /// </para>
    /// <list type="bullet">
    /// <item>Cached type discovery using Unity's TypeCache for performance</item>
    /// <item>Automatic filtering of Unity Object types to prevent serialization issues</item>
    /// <item>Support for both generic and non-generic type constraints</item>
    /// <item>Automatic instantiation of selected types via parameterless constructors</item>
    /// <item>User-friendly dropdown interfaces with type name display</item>
    /// </list>
    /// <para>
    /// The class is particularly useful for configuring polymorphic fields marked with 
    /// SerializeReference, where users need to select from available implementations 
    /// of an interface or abstract class.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var drawer = new TypeDrawer();
    /// 
    /// // For generic type selection
    /// IMyService service = drawer.TypeField&lt;IMyService&gt;(currentService);
    /// 
    /// // For runtime type selection
    /// object factory = drawer.TypeField(typeof(IFactory), currentFactory);
    /// </code>
    /// </example>
    public class TypeDrawer
    {
        /// <summary>
        /// Cache of discovered types organized by their base type or interface.
        /// </summary>
        /// <remarks>
        /// This dictionary maps constraint types (interfaces, abstract classes, base classes) 
        /// to arrays of their implementing or derived types. The cache prevents repeated 
        /// TypeCache queries, which can be expensive for frequently rendered GUI elements.
        /// </remarks>
        private Dictionary<Type, Type[]> cachedTypes;
        
        /// <summary>
        /// Cache of user-friendly type names corresponding to cached types.
        /// </summary>
        /// <remarks>
        /// This dictionary maps constraint types to arrays of display names for their 
        /// implementing types. The names are derived from Type.Name and used for 
        /// dropdown display purposes.
        /// </remarks>
        private Dictionary<Type, string[]> cachedNames;
        
        /// <summary>
        /// Discovers and caches all types that derive from or implement the specified generic type constraint.
        /// </summary>
        /// <typeparam name="T">The type constraint (interface, abstract class, or base class) to search for implementations.</typeparam>
        /// <returns>An array of <see cref="Type"/> objects representing all discovered implementations.</returns>
        /// <remarks>
        /// <para>
        /// This method uses Unity's TypeCache.GetTypesDerivedFrom&lt;T&gt;() for efficient type discovery. 
        /// The results are cached to avoid repeated expensive reflection operations during GUI rendering.
        /// </para>
        /// <para>
        /// The method is thread-safe and handles lazy initialization of the cache dictionary. 
        /// Subsequent calls for the same type constraint will return cached results immediately.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Retrieves or generates user-friendly display names for types associated with the specified constraint.
        /// </summary>
        /// <param name="type">The constraint type for which to retrieve display names.</param>
        /// <returns>An array of strings containing user-friendly names for each implementing type.</returns>
        /// <remarks>
        /// <para>
        /// This method generates display names from Type.Name for use in dropdown controls. 
        /// Names are cached to avoid repeated string operations during GUI rendering.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Discovers and caches all non-Unity Object types that derive from or implement the specified type constraint.
        /// </summary>
        /// <param name="type">The constraint type (interface, abstract class, or base class) to search for implementations.</param>
        /// <returns>An array of <see cref="Type"/> objects representing all discovered non-Unity Object implementations.</returns>
        /// <remarks>
        /// <para>
        /// This overload provides runtime type constraint support and automatically filters out Unity Object types 
        /// to prevent serialization issues. Unity Objects cannot be properly serialized with SerializeReference 
        /// and should be handled through standard Unity serialization mechanisms.
        /// </para>
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