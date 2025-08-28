using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    
    /// <summary>
    /// Field strategy that resolves dependencies by searching for compatible objects within parent GameObjects 
    /// of the current object's hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy is used by the <see cref="ParentDependencyAttribute"/> to implement child-to-parent 
    /// dependency injection. It's useful for scenarios where child objects need to access components 
    /// or services provided by their parent GameObjects, such as:
    /// </para>
    /// <list type="bullet">
    /// <item>UI elements accessing their parent canvas or layout group</item>
    /// <item>Child objects accessing parent controller components</item>
    /// <item>Nested UI components accessing parent dialog or panel managers</item>
    /// </list>
    /// <para>
    /// The strategy relies on the <see cref="IHierarchyTracker"/> service to determine hierarchy 
    /// relationships. It validates that the candidate object is NOT currently tracked as a parent
    /// (meaning it's from a different branch or level) before performing type compatibility checks.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> The current logic checks that the field's GameObject is NOT in the 
    /// hierarchy tracker, which may need verification against the intended parent-matching behavior.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UIButton : FraktalBehavior
    /// {
    ///     [ParentDependency]
    ///     private Canvas parentCanvas; // Will find Canvas component in parent GameObjects
    ///     
    ///     [ParentDependency]
    ///     private DialogManager dialogManager; // Will find DialogManager in parent hierarchy
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ParentDependencyAttribute"/>
    /// <seealso cref="IHierarchyTracker"/>
    /// <seealso cref="AnyChildrenMatch"/>
    public class AnyParentMatch : IFieldStrategy
    {
        /// <summary>
        /// Determines whether the specified object should be injected into the given field based on 
        /// parent hierarchy relationship and type compatibility.
        /// </summary>
        /// <param name="obj">The candidate object that might be injected into the field.</param>
        /// <param name="field">The field that requires dependency injection.</param>
        /// <param name="context">The injection context providing access to services and hierarchy information.</param>
        /// <returns>
        /// <c>true</c> if the object is from a parent component with compatible type and was successfully injected; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the parent hierarchy matching algorithm:
        /// </para>
        /// <list type="number">
        /// <item>Validates that a hierarchy tracker is available in the context</item>
        /// <item>Ensures the field belongs to a Component (not a standalone GameObject)</item>
        /// <item>Checks that the field's GameObject is NOT currently tracked as a parent</item>
        /// <item>Verifies type compatibility using <see cref="Type.IsAssignableFrom(Type)"/></item>
        /// <item>Injects the object if all conditions are met</item>
        /// </list>
        /// <para>
        /// The method returns <c>false</c> early if any validation step fails, ensuring that 
        /// only appropriate parent dependencies are injected.
        /// </para>
        /// <para>
        /// <strong>Implementation Note:</strong> The logic currently excludes objects whose GameObjects 
        /// are tracked as parents, which may need review to ensure correct parent-matching behavior.
        /// </para>
        /// </remarks>
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            if (!context.Services.Get(out IHierarchyTracker tracker)) return false;
            if (instance is not Component component) return false;
            if (tracker.IsParent(component.gameObject)) return false;
            if (!field.IsAssignable(obj)) return false;
            
            field.SetValue(obj, instance);
            
            return true;
        }
    }
}