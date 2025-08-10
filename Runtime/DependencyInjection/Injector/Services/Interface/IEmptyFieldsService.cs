using System;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Service interface for managing fields that have not yet been processed for dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service tracks fields that have been discovered during the field collection phase but 
    /// have not yet undergone dependency resolution processing. Fields in this collection are 
    /// candidates for injection and will be processed by the <see cref="ProcessFieldStep"/> during 
    /// the injection pipeline execution.
    /// </para>
    /// <para>
    /// Fields move from this service to either <see cref="ISucceededFieldsService"/> or 
    /// <see cref="IFailedFieldsService"/> based on whether their dependencies can be successfully resolved.
    /// </para>
    /// <para>
    /// This service is typically registered in the <see cref="InjectionContext"/> and used by 
    /// pipeline steps to coordinate the dependency injection process.
    /// </para>
    /// </remarks>
    /// <seealso cref="IFieldsService"/>
    /// <seealso cref="ISucceededFieldsService"/>
    /// <seealso cref="IFailedFieldsService"/>
    public interface IEmptyFieldsService : IFieldsService
    {
        // This interface inherits all functionality from IFieldsService
        // and serves as a marker for fields that failed dependency resolution
    }
}