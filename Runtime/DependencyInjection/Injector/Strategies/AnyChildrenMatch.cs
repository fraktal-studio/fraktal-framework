using Fraktal.Framework.DI.Injector.Attributes;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Strategies
{
    /// <summary>
    /// Field strategy that resolves dependencies by searching for compatible objects within child GameObjects 
    /// of the current object's hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This strategy is used by the <see cref="ChildrenDependencyAttribute"/> to implement parent-to-child 
    /// dependency injection. It's useful for scenarios where parent objects need to access components 
    /// or services provided by their child GameObjects, such as:
    /// </para>
    /// <list type="bullet">
    /// <item>UI panels accessing child button or input components</item>
    /// <item>Vehicle controllers accessing child wheel or engine components</item>
    /// <item>Manager objects accessing child subsystem components</item>
    /// </list>
    /// <para>
    /// The strategy relies on the <see cref="IHierarchyTracker"/> service to determine hierarchy 
    /// relationships. It validates that the candidate object is a child of the field's containing GameObject 
    /// before performing type compatibility checks.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> The dependency injection system processes objects in hierarchy order, 
    /// so child objects must be processed before their parents for this strategy to find dependencies.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UIPanel : FraktalBehavior
    /// {
    ///     [ChildrenDependency]
    ///     private Button submitButton; // Will find Button in child GameObjects
    ///     
    ///     [ChildrenDependency]
    ///     private InputField[] inputFields; // Array/collection support depends on field type
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ChildrenDependencyAttribute"/>
    /// <seealso cref="IHierarchyTracker"/>
    /// <seealso cref="AnyParentMatch"/>
    public class AnyChildrenMatch : IFieldStrategy
    {
        /// <summary>
        /// Determines whether the specified object should be injected into the given field based on 
        /// child hierarchy relationship and type compatibility.
        /// </summary>
        /// <param name="obj">The candidate object that might be injected into the field.</param>
        /// <param name="field">The field that requires dependency injection.</param>
        /// <param name="context">The injection context providing access to services and hierarchy information.</param>
        /// <returns>
        /// <c>true</c> if the object is a child component with compatible type and was successfully injected; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the child hierarchy matching algorithm:
        /// </para>
        /// <list type="number">
        /// <item>Validates that a hierarchy tracker is available in the context</item>
        /// <item>Ensures the field belongs to a Component (not a standalone GameObject)</item>
        /// <item>Checks that the field's GameObject is tracked as a parent in the hierarchy</item>
        /// <item>Verifies type compatibility using <see cref="Type.IsAssignableFrom(Type)"/></item>
        /// <item>Injects the object if all conditions are met</item>
        /// </list>
        /// <para>
        /// The method returns <c>false</c> early if any validation step fails, ensuring that 
        /// only appropriate child dependencies are injected.
        /// </para>
        /// <para>
        /// Type compatibility is checked using reflection to support inheritance, interface 
        /// implementations, and implicit conversions.
        /// </para>
        /// </remarks>
        public bool Process(Object obj, IField field, InjectionContext context, Object instance)
        {
            if (!context.Services.Get(out IHierarchyTracker tracker)) return false;
            if (instance is not Component component) return false;
            if (!tracker.IsParent(component.gameObject)) return false;
            if (!field.IsAssignable(obj)) return false;
            
            field.SetValue(obj, instance);
            
            return true;
        }
    }
}