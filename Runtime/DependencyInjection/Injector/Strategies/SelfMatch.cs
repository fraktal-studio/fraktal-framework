using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    /// <summary>
    /// Field strategy that resolves dependencies by matching components on the same GameObject 
    /// as the field being injected.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy implements self-injection, allowing components to access other components 
    /// attached to the same GameObject. It's the default strategy for <see cref="AutoDependencyAttribute"/> 
    /// and is useful for scenarios such as:
    /// </para>
    /// <list type="bullet">
    /// <item>Accessing Rigidbody, Collider, or other Unity components on the same GameObject</item>
    /// <item>Cross-component communication within a single GameObject</item>
    /// <item>Modular component design where each component handles specific functionality</item>
    /// </list>
    /// <para>
    /// The strategy requires both the field instance and the candidate object to be Components, 
    /// and validates that they belong to the same GameObject before performing type compatibility checks.
    /// </para>
    /// <para>
    /// This is one of the most commonly used strategies as it enables clean component composition 
    /// without requiring manual GetComponent calls.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class PlayerController : FraktalBehavior
    /// {
    ///     [AutoDependency] // Uses SelfMatch by default
    ///     private Rigidbody playerRigidbody;
    ///     
    ///     [AutoDependency]
    ///     private AudioSource audioSource;
    ///     
    ///     void Start()
    ///     {
    ///         // Dependencies are automatically resolved from the same GameObject
    ///         playerRigidbody.velocity = Vector3.forward;
    ///         audioSource.Play();
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="AutoDependencyAttribute"/>
    /// <seealso cref="AnyMatch"/>
    /// <seealso cref="Component.GetComponent{T}()"/>
    public class SelfMatch : IFieldStrategy
    {
        /// <summary>
        /// Determines whether the specified object should be injected into the given field based on 
        /// same-GameObject relationship and type compatibility.
        /// </summary>
        /// <param name="obj">The candidate object that might be injected into the field.</param>
        /// <param name="field">The field that requires dependency injection.</param>
        /// <param name="context">The injection context (not used by this strategy).</param>
        /// <returns>
        /// <c>true</c> if both objects are Components on the same GameObject with compatible types 
        /// and injection was successful; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the same-GameObject matching algorithm:
        /// </para>
        /// <list type="number">
        /// <item>Validates that the field instance is a Component</item>
        /// <item>Validates that the candidate object is a Component</item>
        /// <item>Checks that both Components belong to the same GameObject</item>
        /// <item>Verifies type compatibility using <see cref="Type.IsAssignableFrom(Type)"/></item>
        /// <item>Injects the object if all conditions are met</item>
        /// </list>
        /// <para>
        /// The method performs early returns on any validation failure to ensure only valid 
        /// same-GameObject dependencies are injected.
        /// </para>
        /// <para>
        /// The <paramref name="context"/> parameter is not used by this strategy since GameObject 
        /// relationship can be determined directly from the Component references.
        /// </para>
        /// <para>
        /// Type compatibility supports inheritance relationships, interface implementations, 
        /// and implicit reference conversions.
        /// </para>
        /// </remarks>
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            if (instance is not Component dependencyComponent) return false;
            if (obj is not Component targetComponent) return false;
            if (dependencyComponent.gameObject != targetComponent.gameObject) return false;
            if (!field.IsAssignable(obj)) return false;
            field.SetValue(obj, instance);
            return true;
        }
    }
}