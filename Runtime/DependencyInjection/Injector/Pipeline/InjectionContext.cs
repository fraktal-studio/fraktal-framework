using System;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.OdinSerializer;
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
        [OdinSerialize]
        public ServiceLocator Services { get; private set; } = new();
        /// <summary>
        /// The current Unity object being processed in the injection pipeline.
        /// </summary>
        /// <remarks>
        /// This property is updated as the pipeline processes different objects in the hierarchy.
        /// It's used by injection steps to determine the context for dependency resolution.
        /// </remarks>
        public UnityEngine.Object currentObject;
        
        /// <summary>
        /// Attempts to retrieve a registered service of type T.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <param name="result">Output parameter for the retrieved service</param>
        /// <returns>True if the service was found and retrieved; otherwise false</returns>
        /// <remarks>
        /// This method provides read-only access to registered services without creating new instances.
        /// Use GetOrRegister if you want to automatically create and register services that don't exist.
        /// </remarks>
        
        public bool Get<T>(out T result) => Services.Get<T>(out result);

        /// <summary>
        /// Registers a service instance with the container.
        /// </summary>
        /// <typeparam name="T">The type to register the service as</typeparam>
        /// <param name="service">The service instance to register</param>
        /// <param name="overwrite">Whether to overwrite an existing registration of the same type</param>
        /// <returns>True if registration succeeded; false if a service of the same type already exists and overwrite is false</returns>
        /// <remarks>
        /// Services are typically registered during pipeline initialization. Overwriting should be used
        /// cautiously as it may affect the behavior of subsequent pipeline steps.
        /// </remarks>
        public bool Register<T>(T service, bool overwrite = false) => Services.Register<T>(service, overwrite);
        
        /// <summary>
        /// Retrieves an existing service or creates and registers a new one if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve or create</typeparam>
        /// <returns>The service instance of type T</returns>
        /// <remarks>
        /// This method first attempts to retrieve an existing service using <see cref="ServiceLocator.Get{T}"/>..
        /// If not found, it uses the registered IServiceFactory to create a new instance,
        /// registers it, and returns it.
        /// 
        /// Requires an IServiceFactory to be registered in the Services container.
        /// Returns default(T) if no service is found and no factory is available.
        /// </remarks>
        public T GetOrRegister<T>()
        {
            if (Services.Get<T>(out var val))
                return val;

            if (!Services.Get<IServiceFactory>(out var serviceFactory))
            {
                return default;
            }

            var result = serviceFactory.Create<T>();
            Services.Register(result);
            return result;
        }
    }
}