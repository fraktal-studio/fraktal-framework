using System;
using Fraktal.Framework.DI.Injector.Strategies;

namespace Fraktal.Framework.DI.Injector.Attributes
{
    /// <summary>
    /// Base attribute class for automatic dependency injection configuration.
    /// Allows specification of the strategy used to resolve field dependencies during injection.
    /// </summary>
    /// <remarks>
    /// This attribute extends <see cref="DependencyAttribute"/> to provide automatic dependency resolution
    /// capabilities. The <see cref="DependencyStrategy"/> property determines how the dependency will be
    /// resolved from available objects in the injection context.
    /// </remarks>
    public class AutoDependencyAttribute : DependencyAttribute
    {
        /// <summary>
        /// Gets or sets the type of strategy used to resolve this dependency.
        /// </summary>
        /// <value>
        /// A <see cref="Type"/> that implements <see cref="IFieldStrategy"/> and defines how this dependency
        /// should be resolved. Defaults to <see cref="SelfMatch"/>, which searches for dependencies on the same GameObject.
        /// </value>
        /// <remarks>
        /// The strategy type must have a parameterless constructor and implement the <see cref="IFieldStrategy"/> interface.
        /// Common strategy types include:
        /// <list type="bullet">
        /// <item><see cref="SelfMatch"/> - Finds dependencies on the same GameObject</item>
        /// <item><see cref="AnyMatch"/> - Accepts any compatible type found during injection</item>
        /// <item><see cref="AnyChildrenMatch"/> - Searches for dependencies in child GameObjects</item>
        /// <item><see cref="AnyParentMatch"/> - Searches for dependencies in parent GameObjects</item>
        /// </list>
        /// </remarks>
        public Type DependencyStrategy { get; set; } = typeof(SelfMatch);
    }
}