using System;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;

namespace Fraktal.Framework.DI.Injector.FieldManagement
{
    /// <summary>
    /// Represents a field that participates in the dependency injection process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface abstracts field operations for dependency injection, allowing different 
    /// implementations to provide field access through various means (reflection, source generation, etc.).
    /// </para>
    /// <para>
    /// Fields implementing this interface can be processed by the injection pipeline to resolve 
    /// their dependencies based on the associated <see cref="IFieldStrategy"/>. The interface 
    /// provides both metadata access (type, name) and value manipulation capabilities.
    /// </para>
    /// <para>
    /// The default <see cref="Process(UnityEngine.Object, InjectionContext)"/> implementation 
    /// delegates to the field's strategy, providing a convenient entry point for dependency resolution.
    /// </para>
    /// </remarks>
    public interface IField
    {
        
        /// <summary>
        /// Gets the type of the field that will receive the injected dependency.
        /// </summary>
        /// <returns>The <see cref="Type"/> of the field.</returns>
        public Type GetFieldType();
        
        /// <summary>
        /// Gets the current value of the field.
        /// </summary>
        /// <returns>The current value of the field, or null if the field is unassigned.</returns>
        public object GetValue();
        
        /// <summary>
        /// Sets the value of the field.
        /// </summary>
        /// <param name="value">The value to assign to the field. Should be compatible with the field type.</param>
        public void SetValue(object value);
        
        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <returns>The name of the field as defined in the source code.</returns>
        public string GetFieldName();
        
        /// <summary>
        /// Determines whether the specified value can be assigned to this field.
        /// </summary>
        /// <param name="value">The value to test for compatibility with the field type.</param>
        /// <returns>
        /// <c>true</c> if the value can be assigned to the field; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAssignable(object value);
        
        /// <summary>
        /// Gets the Unity object instance that contains this field.
        /// </summary>
        /// <returns>The Unity object instance containing the field.</returns>
        public UnityEngine.Object GetInstance();

        /// <summary>
        /// Attempts to process and resolve the dependency for this field using the provided candidate object.
        /// </summary>
        /// <param name="value">The candidate object that might be injected into this field.</param>
        /// <param name="context">The injection context containing services and state information.</param>
        /// <returns>
        /// <c>true</c> if the dependency was successfully resolved and assigned to the field; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This is the default implementation that delegates to the field's associated strategy. 
        /// The strategy determines whether the provided value is suitable for this field based on 
        /// criteria such as type compatibility, object hierarchy relationships, and other custom logic.
        /// </remarks>
        public bool Process(UnityEngine.Object value, InjectionContext context)
        {
            return GetStrategy().Process(value, this,context);
        }
        
        /// <summary>
        /// Gets the strategy used to resolve dependencies for this field.
        /// </summary>
        /// <returns>The <see cref="IFieldStrategy"/> instance associated with this field.</returns>
        public IFieldStrategy GetStrategy();
    }
}