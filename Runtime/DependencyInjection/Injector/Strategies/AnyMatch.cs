using System;
using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    /// <summary>
    /// Field strategy that accepts any object whose type is compatible with the field type, 
    /// regardless of hierarchy relationships or other contextual factors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the most permissive dependency resolution strategy in the framework. It performs 
    /// only type compatibility checking using <see cref="Type.IsAssignableFrom(Type)"/>, making it 
    /// suitable for scenarios where the source of the dependency is not important.
    /// </para>
    /// <para>
    /// The strategy is used by the <see cref="ByAnyAttribute"/> and is ideal for:
    /// </para>
    /// <list type="bullet">
    /// <item>Global services that can come from anywhere in the scene</item>
    /// <item>Singleton-style dependencies</item>
    /// <item>Interface-based dependencies where implementation location doesn't matter</item>
    /// <item>Fallback dependency resolution when more specific strategies fail</item>
    /// </list>
    /// <para>
    /// <strong>Warning:</strong> This strategy will accept the first compatible object found during 
    /// the injection process. If multiple compatible objects exist, the resolution result depends 
    /// on the order in which objects are processed by the pipeline.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class PlayerController : FraktalBehavior
    /// {
    ///     [AnyDependency]
    ///     private IInputService inputService; // Accepts any IInputService implementation
    ///     
    ///     [AnyDependency]
    ///     private AudioSource audioSource; // Accepts any AudioSource component
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ByAnyAttribute"/>
    /// <seealso cref="SelfMatch"/>
    /// <seealso cref="AnyChildrenMatch"/>
    public class AnyMatch : IFieldStrategy
    {

        /// <summary>
        /// Determines whether the specified object can be injected into the given field based solely on type compatibility.
        /// </summary>
        /// <param name="obj">The candidate object that might be injected into the field.</param>
        /// <param name="field">The field that requires dependency injection.</param>
        /// <param name="context">The injection context (not used by this strategy).</param>
        /// <returns>
        /// <c>true</c> if the object's type is assignable to the field type and injection was successful; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the simplest possible dependency resolution algorithm:
        /// </para>
        /// <list type="number">
        /// <item>Checks type compatibility using <see cref="Type.IsAssignableFrom(Type)"/></item>
        /// <item>Injects the object if types are compatible</item>
        /// <item>Returns success/failure status</item>
        /// </list>
        /// <para>
        /// The type compatibility check supports:
        /// </para>
        /// <list type="bullet">
        /// <item>Exact type matches</item>
        /// <item>Inheritance relationships (derived to base class assignment)</item>
        /// <item>Interface implementations</item>
        /// <item>Implicit reference conversions</item>
        /// </list>
        /// <para>
        /// The <paramref name="context"/> parameter is ignored by this strategy since it doesn't 
        /// require any contextual information for decision making.
        /// </para>
        /// </remarks>
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            if (!field.GetFieldType().IsAssignableFrom(obj.GetType()))
                return false;
            field.SetValue(obj, instance);
            return true;
        }
    }
}