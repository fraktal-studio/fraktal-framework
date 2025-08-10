using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Service interface for tracking GameObject hierarchy relationships during dependency injection processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service is used by dependency resolution strategies that need to understand the
    /// parent-child relationships between GameObjects, such as <see cref="AnyChildrenMatch"/>
    /// and <see cref="AnyParentMatch"/> strategies.
    /// </para>
    /// <para>
    /// The hierarchy tracker maintains state about the current processing context, allowing
    /// strategies to make decisions based on object relationships in the Unity scene hierarchy.
    /// </para>
    /// <para>
    /// Implementations should be registered in the <see cref="InjectionContext"/> and updated
    /// by pipeline steps like <see cref="TrackHierarchyStep"/> as different objects are processed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Typical usage in a dependency resolution strategy
    /// if (context.Get&lt;IHierarchyTracker&gt;(out var tracker))
    /// {
    ///     if (tracker.IsParent(candidateObject))
    ///     {
    ///         // This object is in the hierarchy, proceed with injection
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IHierarchyTracker
    {
        /// <summary>
        /// Determines whether the specified GameObject is part of the currently tracked hierarchy.
        /// </summary>
        /// <param name="obj">The GameObject to test for hierarchy membership.</param>
        /// <returns>
        /// <c>true</c> if the GameObject is part of the current hierarchy; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is used by dependency resolution strategies to determine if a potential
        /// dependency target has the correct hierarchical relationship to the object being injected into.
        /// The exact meaning of "parent" depends on the implementation and the direction of traversal.
        /// </remarks>
        public bool IsParent(GameObject obj);
        
        /// <summary>
        /// Gets or sets the current GameObject being tracked in the hierarchy.
        /// </summary>
        /// <value>The GameObject that represents the current processing context.</value>
        /// <remarks>
        /// <para>
        /// This property is typically updated by pipeline steps as they process different
        /// objects in the scene. Setting this property should update any internal tracking
        /// state to reflect the new current object.
        /// </para>
        /// <para>
        /// The current object serves as the reference point for hierarchy-based dependency
        /// resolution strategies.
        /// </para>
        /// </remarks>
        public GameObject Current { get; set; }
    }
}