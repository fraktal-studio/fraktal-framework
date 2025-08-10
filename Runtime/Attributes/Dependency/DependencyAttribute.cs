using System;
using Fraktal.Framework.DI.Injector.Strategies;
using Fraktal.Framework.OdinSerializer;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    /// <summary>
    /// Base attribute class for marking fields as dependencies to be resolved during dependency injection.
    /// </summary>
    /// <remarks>
    /// This attribute extends <see cref="OdinSerializeAttribute"/> to ensure that dependency fields
    /// are properly serialized by the Odin serialization system. Fields marked with this attribute
    /// or its derived classes will be processed during the dependency injection pipeline.
    /// 
    /// This is an abstract base class - use derived attributes like <see cref="AutoDependencyAttribute"/>,
    /// <see cref="AnyDependencyAttribute"/>, <see cref="ChildrenDependencyAttribute"/>, or 
    /// <see cref="ParentDependencyAttribute"/> for actual dependency configuration.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class DependencyAttribute : OdinSerializeAttribute
    {
        
    }
}