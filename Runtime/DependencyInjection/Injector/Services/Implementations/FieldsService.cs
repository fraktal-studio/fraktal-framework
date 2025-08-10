using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.OdinSerializer;
using Unity.VisualScripting;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Base implementation of <see cref="IFieldsService"/> that provides core field collection management functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a concrete implementation of the <see cref="IFieldsService"/> interface 
    /// using an internal collection to store and manage fields. It serves as the base class for 
    /// specialized field service implementations that track fields in different injection states.
    /// </para>
    /// <para>
    /// The class uses Odin serialization to persist field collections, making it suitable for 
    /// use in Unity editor tools and serialized contexts. The internal collection can be 
    /// customized through constructor injection to support different collection behaviors.
    /// </para>
    /// <para>
    /// By default, the service uses a <see cref="HashSet{IField}"/> to prevent duplicate field 
    /// entries and provide efficient add/remove operations.
    /// </para>
    /// </remarks>
    public class FieldsService : IFieldsService
    {
        /// <summary>
        /// The internal collection used to store managed fields.
        /// </summary>
        /// <remarks>
        /// This field is marked with <see cref="OdinSerializeAttribute"/> to ensure proper 
        /// serialization when the service is used in Unity editor contexts or saved as part 
        /// of serialized assets.
        /// </remarks>
        [OdinSerialize]
        private ICollection<IField> fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsService"/> class with the specified field collection.
        /// </summary>
        /// <param name="fields">The collection to use for storing managed fields.</param>
        /// <remarks>
        /// This constructor allows for custom collection implementations to be used, enabling 
        /// different behaviors such as ordered collections, concurrent collections, or collections 
        /// with specific performance characteristics.
        /// </remarks>
        public FieldsService(ICollection<IField> fields)
        {
            this.fields = fields;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsService"/> class with a default <see cref="HashSet{IField}"/> collection.
        /// </summary>
        /// <remarks>
        /// This constructor provides a convenient way to create a field service with standard 
        /// behavior, using a hash set for efficient field management and automatic duplicate prevention.
        /// </remarks>
        public FieldsService() : this(new HashSet<IField>())
        {
            
        }

        /// <summary>
        /// Gets a snapshot of all fields currently managed by this service.
        /// </summary>
        /// <returns>
        /// A new <see cref="List{IField}"/> containing copies of all fields in the service's collection.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns a defensive copy of the internal collection to prevent external 
        /// modification of the service's state. The returned list is safe to modify without 
        /// affecting the service's internal collection.
        /// </para>
        /// </remarks>
        public ICollection<IField> GetFields()
        {
            return fields.ToList();
        }

        /// <summary>
        /// Gets the number of fields currently managed by this service.
        /// </summary>
        /// <value>The total count of fields in the internal collection.</value>
        public int FieldCount => fields.Count;

        /// <summary>
        /// Adds a field to the service's managed collection.
        /// </summary>
        /// <param name="field">The field to add to the collection.</param>
        /// <remarks>
        /// The behavior of duplicate additions depends on the underlying collection type. 
        /// If using a <see cref="HashSet{IField}"/>, duplicates will be automatically prevented.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// May be thrown by the underlying collection if <paramref name="field"/> is null, 
        /// depending on the collection implementation.
        /// </exception>
        public void AddField(IField field)
        {
            fields.Add(field);
        }

        /// <summary>
        /// Removes a field from the service's managed collection.
        /// </summary>
        /// <param name="field">The field to remove from the collection.</param>
        /// <returns>
        /// <c>true</c> if the field was successfully removed; <c>false</c> if the field 
        /// was not found in the collection.
        /// </returns>
        public bool RemoveField(IField field)
        {
            return fields.Remove(field);
        }
    }
}