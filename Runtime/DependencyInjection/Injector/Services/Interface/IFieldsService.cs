using System.Collections.Generic;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Core service interface for managing collections of fields during dependency injection processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface defines the fundamental operations for field collection management within 
    /// the dependency injection system. It provides methods for adding, removing, and querying 
    /// fields that are participating in the injection process.
    /// </para>
    /// <para>
    /// The interface is implemented by specialized services that track fields in different 
    /// states of the injection lifecycle:
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="IEmptyFieldsService"/> - Fields awaiting dependency resolution</item>
    /// <item><see cref="ISucceededFieldsService"/> - Fields with successfully resolved dependencies</item>
    /// <item><see cref="IFailedFieldsService"/> - Fields where dependency resolution failed</item>
    /// </list>
    /// <para>
    /// Services implementing this interface are typically registered in the <see cref="InjectionContext"/> 
    /// and used by pipeline steps to coordinate field processing and track injection results.
    /// </para>
    /// </remarks>
    /// <seealso cref="IField"/>
    /// <seealso cref="InjectionContext"/>
    public interface IFieldsService
    {
        /// <summary>
        /// Gets a collection containing all fields managed by this service.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection{IField}"/> containing all fields currently managed by this service.
        /// The returned collection is a snapshot and modifications to it will not affect the underlying service.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The method name "GetEmptyFields" is maintained for historical compatibility, but the method 
        /// actually returns all fields managed by the service regardless of their state.
        /// </para>
        /// <para>
        /// The returned collection is safe to iterate over and will not be modified by concurrent 
        /// operations on the service.
        /// </para>
        /// </remarks>
        public ICollection<IField> GetFields();
        
        /// <summary>
        /// Gets the number of fields currently managed by this service.
        /// </summary>
        /// <value>The total count of fields in the service's collection.</value>
        /// <remarks>
        /// This property provides an efficient way to check the size of the field collection 
        /// without needing to enumerate all fields.
        /// </remarks>
        public int FieldCount { get; }
        
        /// <summary>
        /// Adds a field to the service's managed collection.
        /// </summary>
        /// <param name="field">The field to add to the collection.</param>
        /// <remarks>
        /// <para>
        /// If the service uses a <see cref="HashSet{IField}"/> internally, duplicate fields 
        /// will be automatically prevented. The behavior for duplicate additions depends on 
        /// the underlying collection implementation.
        /// </para>
        /// <para>
        /// Fields added to the service become eligible for processing by the associated 
        /// pipeline steps and services.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="field"/> is null.
        /// </exception>
        public void AddField(IField field);
        
        /// <summary>
        /// Removes a field from the service's managed collection.
        /// </summary>
        /// <param name="field">The field to remove from the collection.</param>
        /// <returns>
        /// <c>true</c> if the field was successfully removed; <c>false</c> if the field 
        /// was not found in the collection.
        /// </returns>
        /// <remarks>
        /// This method is typically used by pipeline steps to move fields between different 
        /// service collections as their injection state changes (e.g., from empty to succeeded or failed).
        /// </remarks>
        public bool RemoveField(IField field);
    }
}