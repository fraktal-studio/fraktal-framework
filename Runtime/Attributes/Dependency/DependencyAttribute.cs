using System;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    /// <summary>
    /// Base attribute class for marking fields as dependencies to be resolved during dependency injection.
    /// </summary>
    /// <remarks>
    /// <see cref="ByAnyAttribute"/>, <see cref="ByChildAttribute"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class DependencyAttribute : Attribute
    {
        
    }
}