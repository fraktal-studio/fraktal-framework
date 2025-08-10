using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Factory interface for creating service instances by type during dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface provides a generic mechanism for creating service instances when they
    /// are requested but not yet registered in the <see cref="ServiceLocator"/>. It enables
    /// lazy instantiation of services and supports the "GetOrRegister" pattern used throughout
    /// the dependency injection system.
    /// </para>
    /// <para>
    /// Implementations might use various creation strategies such as reflection-based instantiation,
    /// dependency injection containers, or custom factory logic.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Usage in InjectionContext.GetOrRegister method
    /// if (!Services.Get&lt;IMyService&gt;(out var service))
    /// {
    ///     if (Services.Get&lt;IServiceFactory&gt;(out var factory))
    ///     {
    ///         service = factory.Create&lt;IMyService&gt;();
    ///         Services.Register(service);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="InjectionContext.GetOrRegister{T}()"/>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates an instance of the specified service type.
        /// </summary>
        /// <typeparam name="T">The type of service to create.</typeparam>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/>, or the default value if creation fails.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Implementations should handle type instantiation logic, which may include:
        /// </para>
        /// <list type="bullet">
        /// <item>Reflection-based constructor invocation</item>
        /// <item>Dependency injection for constructor parameters</item>
        /// <item>Factory method invocation</item>
        /// <item>Singleton pattern enforcement</item>
        /// </list>
        /// <para>
        /// If the type cannot be instantiated, implementations should return <c>default(T)</c>
        /// rather than throwing exceptions to maintain pipeline stability.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// May be thrown by implementations if the type cannot be instantiated due to
        /// missing constructors, abstract types, or other creation constraints.
        /// </exception>
        public T Create<T>();
    }
}