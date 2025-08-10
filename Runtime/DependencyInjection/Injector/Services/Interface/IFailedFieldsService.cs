namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Service interface for managing fields that failed dependency injection resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service tracks fields where dependency injection attempts have failed. Fields in this 
    /// collection represent unresolved dependencies that could not be satisfied during the 
    /// injection process, either due to missing dependencies, type mismatches, or strategy 
    /// resolution failures.
    /// </para>
    /// <para>
    /// Failed fields are useful for debugging dependency injection issues and can be displayed 
    /// in editor tools like <see cref="InjectionResultWindow"/> to help developers identify 
    /// and resolve dependency problems.
    /// </para>
    /// <para>
    /// Fields typically move to this service from <see cref="IEmptyFieldsService"/> when 
    /// their associated <see cref="IFieldStrategy"/> returns <c>false</c> during processing 
    /// attempts.
    /// </para>
    /// </remarks>
    /// <seealso cref="IFieldsService"/>
    /// <seealso cref="IEmptyFieldsService"/>
    /// <seealso cref="ISucceededFieldsService"/>
    public interface IFailedFieldsService : IFieldsService
    {
        // This interface inherits all functionality from IFieldsService
        // and serves as a marker for fields that failed dependency resolution
    }
}