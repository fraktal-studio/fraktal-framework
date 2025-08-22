using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    public class BySelfAttribute : AutoDependencyAttribute
    {
        public BySelfAttribute()
        {
            DependencyStrategy = typeof(SelfMatch);
        }
    }
}