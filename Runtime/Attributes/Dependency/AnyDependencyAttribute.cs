using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    public class AnyDependencyAttribute : AutoDependencyAttribute
    {
        public AnyDependencyAttribute()
        {
            DependencyStrategy = typeof(AnyMatch);
        }
    }
}