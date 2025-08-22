using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    public class ByTagAttribute : AutoDependencyAttribute
    {
        public string Tag { get; set; }
        
        public ByTagAttribute(string tag)
        {
            DependencyStrategy = typeof(TagMatch);
            Tag = tag;
        }
    }
}