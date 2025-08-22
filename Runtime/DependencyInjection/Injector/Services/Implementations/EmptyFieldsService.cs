using System.Collections.Generic;
using Fraktal.Framework.DI.Injector.FieldManagement;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Specialized field service implementation for managing fields that are awaiting dependency injection processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service extends <see cref="FieldsService"/> to provide a concrete implementation 
    /// for tracking fields that have been discovered but not yet processed for dependency injection. 
    /// It implements <see cref="IEmptyFieldsService"/> to provide type-safe service registration and retrieval.
    /// </para>
    /// <para>
    /// Fields are typically added to this service during the field collection phase when 
    /// dependency-marked fields are discovered through reflection. They remain in this service 
    /// until processing attempts move them to either <see cref="ISucceededFieldsService"/> 
    /// or <see cref="IFailedFieldsService"/>.
    /// </para>
    /// <para>
    /// This service acts as the primary queue for dependency injection processing and is 
    /// actively consumed by the <see cref="ProcessFieldStep"/> during pipeline execution.
    /// </para>
    /// </remarks>
    /// <seealso cref="FieldsService"/>
    /// <seealso cref="IEmptyFieldsService"/>
    public class EmptyFieldsService : FieldsService, IEmptyFieldsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyFieldsService"/> class with the specified field collection.
        /// </summary>
        /// <param name="fields">The collection to use for storing empty fields awaiting processing.</param>
        /// <remarks>
        /// This constructor allows for custom collection implementations to be used for storing 
        /// empty fields, which may be useful for ordered processing or other specialized behaviors.
        /// </remarks>
        public EmptyFieldsService(IDictionary<Object, ICollection<IField>> fields) : base(fields)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyFieldsService"/> class with a default <see cref="HashSet{IField}"/> collection.
        /// </summary>
        /// <remarks>
        /// This constructor provides a convenient way to create an empty fields service with 
        /// standard behavior, using a hash set for efficient field management.
        /// </remarks>
        public EmptyFieldsService() : base(new Dictionary<Object, ICollection<IField>>())
        {
            
        }
    }
}