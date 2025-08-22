using System;
using Fraktal.DesignPatterns;
namespace Fraktal.Framework.DI.Injector.Pipeline
{
    /// <summary>
    /// Context object that maintains state and services during the dependency injection process.
    /// Acts as a central container for all services and the current object being processed.
    /// </summary>
    /// <remarks>
    /// InjectionContext is passed through each step of the injection pipeline, providing:
    /// - Access to registered services via the Services property
    /// - Tracking of the current object being processed
    /// - Methods for service registration and retrieval
    /// 
    /// This class is serialized using OdinSerializer to maintain state between editor operations.
    /// </remarks>
    [Serializable]
    public class InjectionContext
    {
        /// <summary>
        /// Container for all registered services used during dependency injection.
        /// </summary>
        /// <remarks>
        /// This TypedRegistration instance manages the storage and retrieval of services
        /// by their types. Services can be registered with or without polymorphism support.
        /// </remarks>
        public ServiceLocator Services { get; private set; } = new();
        /// <summary>
        /// The current Unity object being processed in the injection pipeline.
        /// </summary>
        /// <remarks>
        /// This property is updated as the pipeline processes different objects in the hierarchy.
        /// It's used by injection steps to determine the context for dependency resolution.
        /// </remarks>
        public UnityEngine.Object currentObject;
    }
}