using System;
using System.Reflection;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.FieldManagement.Implementations;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Reflection-based factory implementation for creating <see cref="IField"/> instances
    /// from <see cref="FieldInfo"/> and Unity component data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This factory discovers and instantiates all available <see cref="IFieldStrategy"/> implementations
    /// at construction time using Unity's <see cref="TypeCache"/>. It then uses these strategies
    /// to create appropriate <see cref="ReflectionField"/> instances based on field dependency attributes.
    /// </para>
    /// <para>
    /// The factory automatically registers all parameterless constructible strategy types,
    /// making the system easily extensible by simply adding new strategy implementations.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var factory = new ReflectionFieldFactory();
    /// 
    /// // Create field wrapper for a dependency-marked field
    /// FieldInfo fieldInfo = typeof(SomeComponent).GetField("dependency", BindingFlags.NonPublic | BindingFlags.Instance);
    /// Component component = GetComponent&lt;SomeComponent&gt;();
    /// IField field = factory.Create(fieldInfo, component);
    /// </code>
    /// </example>
    public class ReflectionFieldFactory : IFieldFactory
    {
        /// <summary>
        /// Service locator containing all discovered and instantiated field strategies.
        /// </summary>
        private ServiceLocator registration = new ServiceLocator();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionFieldFactory"/> class.
        /// Discovers and registers all available <see cref="IFieldStrategy"/> implementations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The constructor uses Unity's <see cref="TypeCache.GetTypesDerivedFrom{T}()"/> to find
        /// all types that implement <see cref="IFieldStrategy"/>. For each type that has a
        /// parameterless constructor, it creates an instance and registers it in the internal
        /// service locator.
        /// </para>
        /// <para>
        /// This approach enables automatic discovery of strategy implementations without
        /// requiring explicit registration, making the system easily extensible.
        /// </para>
        /// </remarks>
        public ReflectionFieldFactory()
        {

            TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom<IFieldStrategy>();
            foreach (Type type in collection)
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    continue;
                var instantiated = ctor.Invoke(Array.Empty<object>());
                registration.Register(type, instantiated);
            }
        }
        
        /// <summary>
        /// Creates an <see cref="IField"/> instance for the specified field and component
        /// if the field has a valid dependency attribute.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> representing the field to create an IField for.</param>
        /// <param name="component">The Unity component instance that contains the field.</param>
        /// <returns>
        /// A new <see cref="ReflectionField"/> instance if the field has a valid <see cref="AutoDependencyAttribute"/>
        /// and an appropriate strategy is available; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method checks if the field has an <see cref="AutoDependencyAttribute"/> and attempts
        /// to resolve the specified <see cref="AutoDependencyAttribute.DependencyStrategy"/> from
        /// the registered strategies. If successful, it creates a <see cref="ReflectionField"/>
        /// that combines the field metadata, component instance, and resolution strategy.
        /// </para>
        /// <para>
        /// Fields without dependency attributes or with unresolvable strategies are ignored
        /// and return <c>null</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Field with [AnyDependency] attribute will create a ReflectionField
        /// var field1 = factory.Create(dependencyFieldInfo, component); // Returns ReflectionField
        /// 
        /// // Field without dependency attribute will return null
        /// var field2 = factory.Create(regularFieldInfo, component); // Returns null
        /// </code>
        /// </example>
        public IField Create(FieldInfo field, UnityEngine.Object component)
        {
            AutoDependencyAttribute attr = field.GetCustomAttribute<AutoDependencyAttribute>();
            if (attr == null) return null;
            if (!registration.Get(attr.DependencyStrategy, out var strategy))
            {
                Debug.LogWarning($"The provided type is not a strategy: {component.GetType().Name}: {field.FieldType.Name} {field.Name}");
                return null;
            }

            var strat = (IFieldStrategy)strategy;
            
            return new ReflectionField(field, component, strat);
        }
    }
}