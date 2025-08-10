using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    /// <summary>
    /// Dependency injection attribute that resolves dependencies from parent GameObjects in the hierarchy.
    /// </summary>
    /// <remarks>
    /// This attribute configures the dependency to use a parent-matching strategy, which searches
    /// for compatible objects only within the parent GameObjects of the current object's hierarchy.
    /// 
    /// This is useful for child objects that need to access components or services provided by their parents,
    /// such as UI elements accessing their parent canvas or child objects accessing parent controllers.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UIButton : FraktalBehavior
    /// {
    ///     [ParentDependency]
    ///     private Canvas parentCanvas; // Will find Canvas component in parent GameObjects
    /// }
    /// </code>
    /// </example>
    public class ParentDependencyAttribute : AutoDependencyAttribute
    {
        public ParentDependencyAttribute()
        {
            DependencyStrategy = typeof(AnyParentMatch);
        }
    }
}