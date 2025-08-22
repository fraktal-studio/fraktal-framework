using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    public class ByAnyAttribute : AutoDependencyAttribute
    {
        public ByAnyAttribute()
        {
            DependencyStrategy = typeof(AnyMatch);
        }
    }
}