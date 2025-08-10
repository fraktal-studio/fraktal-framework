namespace Fraktal.Framework.DI.Injector.Services
{
    
    /// <summary>
    /// Service interface for managing fields that successfully completed dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service tracks fields where dependency injection was successfully completed. Fields 
    /// in this collection represent resolved dependencies where the appropriate objects were 
    /// found and injected according to their configured strategies.
    /// </para>
    /// <para>
    /// Successfully injected fields serve as a record of the injection process and can be 
    /// useful for debugging, validation, and reporting purposes. The <see cref="InjectionResultWindow"/> 
    /// displays these fields to provide feedback about successful dependency resolutions.
    /// </para>
    /// <para>
    /// Fields typically move to this service from <see cref="IEmptyFieldsService"/> when 
    /// their associated <see cref="IFieldStrategy"/> successfully processes and injects 
    /// a dependency.
    /// </para>
    /// </remarks>
    /// <seealso cref="IFieldsService"/>
    /// <seealso cref="IEmptyFieldsService"/>
    /// <seealso cref="IFailedFieldsService"/>
    public interface ISucceededFieldsService : IFieldsService
    {
        // This interface inherits all functionality from IFieldsService
        // and serves as a marker for fields that failed dependency resolution
    }
}