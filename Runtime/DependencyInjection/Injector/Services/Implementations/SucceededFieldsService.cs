using System.Collections.Generic;
using Fraktal.Framework.DI.Injector.FieldManagement;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Specialized field service implementation for managing fields that successfully completed dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service extends <see cref="FieldsService"/> to provide a concrete implementation 
    /// for tracking fields where dependency injection was successfully completed. It implements 
    /// <see cref="ISucceededFieldsService"/> to provide type-safe service registration and retrieval.
    /// </para>
    /// <para>
    /// Fields are added to this service when their associated <see cref="IFieldStrategy"/> 
    /// successfully processes and injects a dependency, indicating that the field now contains 
    /// the appropriate resolved object.
    /// </para>
    /// <para>
    /// This service provides valuable feedback about injection success rates and can be used 
    /// for validation, debugging, and reporting purposes. The <see cref="InjectionResultWindow"/> 
    /// uses this service to display successfully resolved dependencies to developers.
    /// </para>
    /// </remarks>
    /// <seealso cref="FieldsService"/>
    /// <seealso cref="ISucceededFieldsService"/>
    public class SucceededFieldsService : FieldsService, ISucceededFieldsService
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SucceededFieldsService"/> class with a default <see cref="HashSet{IField}"/> collection.
        /// </summary>
        /// <remarks>
        /// This constructor provides a convenient way to create a succeeded fields service with 
        /// standard behavior, using a hash set for efficient field management.
        /// </remarks>
        public SucceededFieldsService() : base(new Dictionary<Object, ICollection<IField>>())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SucceededFieldsService"/> class with the specified field collection.
        /// </summary>
        /// <param name="fields">The collection to use for storing successfully injected fields.</param>
        /// <remarks>
        /// This constructor allows for custom collection implementations to be used for storing 
        /// succeeded fields, which may be useful for ordered tracking or specialized reporting requirements.
        /// </remarks>
        public SucceededFieldsService(IDictionary<Object, ICollection<IField>> fields) : base(fields)
        {
            
        }
    }
}