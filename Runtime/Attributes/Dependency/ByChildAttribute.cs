using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    /// <summary>
    /// Dependency injection attribute that resolves dependencies from child GameObjects in the hierarchy.
    /// </summary>
    /// <remarks>
    /// This attribute configures the dependency to use <see cref="AnyChildrenMatch"/> strategy, which searches
    /// for compatible objects only within the child GameObjects of the current object's hierarchy.
    /// 
    /// This is useful for parent objects that need to access components or services provided by their children,
    /// such as a UI panel accessing child button components or a vehicle accessing child wheel components.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UIPanel : FraktalBehavior
    /// {
    ///     [ChildrenDependency]
    ///     private Button submitButton; // Will find Button component in child GameObjects
    /// }
    /// </code>
    /// </example>
    public class ByChildAttribute : AutoDependencyAttribute
    {
        public ByChildAttribute()
        {
            DependencyStrategy = typeof(AnyChildrenMatch);
        }
    }
}