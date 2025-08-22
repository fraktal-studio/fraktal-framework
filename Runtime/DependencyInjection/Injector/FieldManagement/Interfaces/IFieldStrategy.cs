using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Defines a strategy for resolving dependencies for specific fields during the injection process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Field strategies implement the core logic for determining whether a particular object should 
    /// be injected into a specific field. Different strategies can implement various resolution 
    /// approaches, such as:
    /// </para>
    /// <list type="bullet">
    /// <item>Type-based matching (exact type or assignable types)</item>
    /// <item>Hierarchy-based matching (same GameObject, parent, or child objects)</item>
    /// <item>Name-based matching</item>
    /// <item>Custom attribute-based matching</item>
    /// <item>Composite strategies combining multiple criteria</item>
    /// </list>
    /// <para>
    /// Strategies are typically associated with dependency attributes and are instantiated by the 
    /// <see cref="IFieldFactory"/> during field discovery. Each strategy instance can maintain 
    /// state if needed for complex resolution logic.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class NameMatchStrategy : IFieldStrategy
    /// {
    ///     private readonly string targetName;
    ///     
    ///     public NameMatchStrategy(string targetName)
    ///     {
    ///         this.targetName = targetName;
    ///     }
    ///     
    ///     public bool Process(UnityEngine.Object obj, IField field, InjectionContext context)
    ///     {
    ///         return obj.name == targetName && 
    ///                field.GetFieldType().IsAssignableFrom(obj.GetType());
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IFieldStrategy
    {
        /// <summary>
        /// Determines whether the specified object should be injected into the given field.
        /// </summary>
        /// <param name="obj">The candidate object that might be injected into the field.</param>
        /// <param name="field">The field that requires dependency injection.</param>
        /// <param name="context">
        /// The injection context providing access to services, hierarchy information, 
        /// and other state needed for resolution decisions.
        /// </param>
        /// <returns>
        /// <c>true</c> if the object should be injected into the field; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Implementations should perform all necessary validation, including type compatibility 
        /// checking, before returning <c>true</c>. When this method returns <c>true</c>, the 
        /// injection system will call <see cref="IField.SetValue(object)"/> to assign the object 
        /// to the field.
        /// </para>
        /// <para>
        /// The method should be idempotent and should not have side effects beyond the injection 
        /// itself. Complex strategies may use the <paramref name="context"/> to access services 
        /// like hierarchy trackers or custom resolution services.
        /// </para>
        /// </remarks>
        public bool Process(UnityEngine.Object obj, IField field, InjectionContext context, Object instance);
    }
}