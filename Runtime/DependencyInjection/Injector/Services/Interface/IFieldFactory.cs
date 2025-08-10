using System.Reflection;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.FieldManagement;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Factory interface for creating <see cref="IField"/> instances from field reflection data and Unity components.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface extends the generic <see cref="IFactory{TInput1, TInput2, TOutput}"/> pattern
    /// to provide a specialized factory for dependency injection field creation. Implementations
    /// are responsible for analyzing field metadata and creating appropriate field wrappers
    /// for the dependency injection system.
    /// </para>
    /// <para>
    /// The factory is typically used during the field collection phase of dependency injection
    /// to discover and wrap fields marked with dependency attributes.
    /// </para>
    /// </remarks>
    /// <seealso cref="ReflectionFieldFactory"/>
    /// <seealso cref="IField"/>
    public interface IFieldFactory : IFactory<FieldInfo,UnityEngine.Object, IField>
    {
        // Interface inherits Create method from base factory interface:
        // IField Create(FieldInfo field, UnityEngine.Object component);
    }
}