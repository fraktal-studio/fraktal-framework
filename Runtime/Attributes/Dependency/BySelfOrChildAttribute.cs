using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    public class BySelfOrChildAttribute : AutoDependencyAttribute
    {
        public BySelfOrChildAttribute()
        {
            DependencyStrategy = typeof(SelfOrChildrenMatch);
        }
    }
}