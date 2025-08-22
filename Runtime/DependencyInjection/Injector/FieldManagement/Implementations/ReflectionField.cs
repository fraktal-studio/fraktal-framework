using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.FieldManagement.Implementations
{
    /// <summary>
    /// Reflection-based implementation of <see cref="IField"/> that wraps a <see cref="FieldInfo"/> 
    /// for dependency injection operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a concrete implementation for accessing and manipulating fields during 
    /// the dependency injection process. It uses .NET reflection to get and set field values on 
    /// Unity objects, while maintaining the associated injection strategy.
    /// </para>
    /// <para>
    /// Each <see cref="ReflectionField"/> represents a single field that has been marked with a 
    /// dependency attribute and is participating in the injection pipeline. The field maintains 
    /// references to the original <see cref="FieldInfo"/>, the object instance containing the field, 
    /// and the strategy used to resolve dependencies for this field.
    /// </para>
    /// <para>
    /// Equality is determined by comparing the field, instance, and strategy references, making it 
    /// safe to use in collections like <see cref="HashSet{T}"/> for tracking injection state.
    /// </para>
    /// </remarks>
    public class ReflectionField : IField
    {
        /// <summary>
        /// The <see cref="FieldInfo"/> representing the field to be injected.
        /// </summary>
        private readonly FieldInfo field;
        /// <summary>
        /// The strategy used to resolve dependencies for this field.
        /// </summary>
        private readonly IFieldStrategy strategy;

        private readonly DependencyAttribute attributes;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionField"/> class.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> representing the field to be injected.</param>
        /// <param name="instance">The Unity object instance that contains the field.</param>
        /// <param name="strategy">The strategy used to resolve dependencies for this field.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="field"/>, <paramref name="instance"/>, or <paramref name="strategy"/> is null.
        /// </exception>
        public ReflectionField(FieldInfo field, IFieldStrategy strategy)
        {
            this.field = field;
            this.strategy = strategy;
            this.attributes = field.GetCustomAttribute<DependencyAttribute>();
        }
        
        /// <summary>
        /// Gets the type of the field that will receive the injected dependency.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the field.</returns>
        public Type GetFieldType()
        {
            return field.FieldType;
        }
        
        /// <summary>
        /// Gets the current value of the field from the target instance.
        /// </summary>
        /// <returns>The current value of the field, or null if the field is unassigned.</returns>
        /// <remarks>
        /// This method uses reflection to access the field value. If the field is a value type 
        /// and uninitialized, it will return the default value for that type.
        /// </remarks>
        public object GetValue(object instance)
        {
            
            return field.GetValue(instance);
        }

        /// <summary>
        /// Sets the value of the field on the target instance.
        /// </summary>
        /// <param name="value">The value to assign to the field. Must be assignable to the field type.</param>
        /// <remarks>
        /// This method uses reflection to set the field value. The caller should verify type compatibility 
        /// using <see cref="IsAssignable(object)"/> before calling this method to avoid runtime exceptions.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided value is not assignable to the field type.
        /// </exception>
        public void SetValue(object value, object instance)
        {
            field.SetValue(instance, value);
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <returns>The name of the field as defined in the source code.</returns>
        public string GetFieldName()
        {
            return field.Name;
        }

        /// <summary>
        /// Determines whether the specified value can be assigned to this field.
        /// </summary>
        /// <param name="value">The value to test for compatibility with the field type.</param>
        /// <returns>
        /// <c>true</c> if the value can be assigned to the field; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method checks type compatibility using <see cref="Type.IsAssignableFrom(Type)"/>, 
        /// which handles inheritance relationships, interface implementations, and type conversions.
        /// A null value will return <c>false</c> to prevent null reference exceptions.
        /// </remarks>
        public bool IsAssignable(object value)
        {
            return field.FieldType.IsAssignableFrom(value.GetType());
        }

        /// <summary>
        /// Gets the strategy used to resolve dependencies for this field.
        /// </summary>
        /// <returns>The <see cref="IFieldStrategy"/> instance associated with this field.</returns>
        public IFieldStrategy GetStrategy()
        {
            return strategy;
        }

        public Attribute GetAttribute()
        {
            return attributes;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ReflectionField"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="ReflectionField"/> to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ReflectionField"/> has the same field, instance, and strategy; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool Equals(ReflectionField other)
        {
            return Equals(field, other.field) && Equals(strategy, other.strategy);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified object is a <see cref="ReflectionField"/> and is equal to the current instance; 
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ReflectionField)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <remarks>
        /// The hash code is computed from the field and instance references, excluding the strategy 
        /// to maintain consistency with the equality comparison logic.
        /// </remarks>
        public override int GetHashCode()
        {
            return HashCode.Combine(field);
        }
    }
}