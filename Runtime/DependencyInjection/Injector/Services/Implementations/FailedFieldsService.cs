using System.Collections.Generic;
using Fraktal.Framework.DI.Injector.FieldManagement;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Specialized field service implementation for managing fields that failed dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service extends <see cref="FieldsService"/> to provide a concrete implementation 
    /// for tracking fields where dependency injection attempts have failed. It implements 
    /// <see cref="IFailedFieldsService"/> to provide type-safe service registration and retrieval.
    /// </para>
    /// <para>
    /// Fields are typically added to this service when their associated <see cref="IFieldStrategy"/> 
    /// returns <c>false</c> during dependency resolution attempts, indicating that no suitable 
    /// dependency could be found or injected.
    /// </para>
    /// <para>
    /// This service is essential for debugging and troubleshooting dependency injection issues, 
    /// as it provides a centralized location to track and analyze injection failures.
    /// </para>
    /// </remarks>
    /// <seealso cref="FieldsService"/>
    /// <seealso cref="IFailedFieldsService"/>
    public class FailedFieldsService : FieldsService, IFailedFieldsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedFieldsService"/> class with the specified field collection.
        /// </summary>
        /// <param name="fields">The collection to use for storing failed fields.</param>
        /// <remarks>
        /// This constructor allows for custom collection implementations to be used for storing 
        /// failed fields, which may be useful for specialized tracking or reporting requirements.
        /// </remarks>
        public FailedFieldsService(IDictionary<Object, ICollection<IField>> fields) : base(fields)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedFieldsService"/> class with a default <see cref="HashSet{IField}"/> collection.
        /// </summary>
        /// <remarks>
        /// This constructor provides a convenient way to create a failed fields service with 
        /// standard behavior, using a hash set for efficient field management.
        /// </remarks>
        public FailedFieldsService() : base(new Dictionary<Object, ICollection<IField>>())
        {
            
        }
    }
}