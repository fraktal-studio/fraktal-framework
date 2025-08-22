using System;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;

namespace FraktalFramework.Editor.Interfaces
{
    
    public interface IInjectionPipelineFactory : IFactory<int, InjectionPipeline> { }
}