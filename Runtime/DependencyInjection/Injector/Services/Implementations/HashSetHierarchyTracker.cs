using System.Collections.Generic;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Services
{
    /// <summary>
    /// Implementation of <see cref="IHierarchyTracker"/> that uses a <see cref="HashSet{GameObject}"/> 
    /// to track GameObject hierarchy relationships during dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This tracker maintains a collection of GameObjects that represent the current hierarchy chain
    /// being processed. It's used by dependency resolution strategies to determine parent-child
    /// relationships when resolving dependencies that depend on hierarchy position.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> The current implementation has a potential bug in <see cref="SetCurrent(GameObject)"/>
    /// where accessing <c>obj.transform.parent</c> without null checking may cause NullReferenceException
    /// for root GameObjects.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var tracker = new HashSetHierarchyTracker();
    /// tracker.Current = someGameObject;
    /// 
    /// // Check if another object is in the current hierarchy
    /// bool isInHierarchy = tracker.IsParent(parentGameObject);
    /// </code>
    /// </example>
    public class HashSetHierarchyTracker : IHierarchyTracker
    {
        /// <summary>
        /// Set containing all GameObjects in the current hierarchy chain.
        /// </summary>
        private HashSet<GameObject> hierarchy = new ();

        /// <summary>
        /// The most recently set GameObject in the hierarchy.
        /// </summary>
        private GameObject last = null;
        
        /// <summary>
        /// Determines whether the specified GameObject is part of the current tracked hierarchy.
        /// </summary>
        /// <param name="obj">The GameObject to check for hierarchy membership.</param>
        /// <returns>
        /// <c>true</c> if the GameObject is part of the current hierarchy; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is typically used by dependency resolution strategies like 
        /// <see cref="AnyChildrenMatch"/> and <see cref="AnyParentMatch"/> to determine
        /// whether a candidate dependency object has the correct hierarchy relationship.
        /// </remarks>
        public bool IsParent(GameObject obj)
        {
            return hierarchy.Contains(obj);
        }

        /// <summary>
        /// Gets or sets the current GameObject being tracked in the hierarchy.
        /// </summary>
        /// <value>The currently active GameObject in the hierarchy chain.</value>
        /// <remarks>
        /// Setting this property updates the internal hierarchy tracking state by calling
        /// <see cref="SetCurrent(GameObject)"/>. This property is typically updated as
        /// the dependency injection pipeline processes different objects in the scene.
        /// </remarks>
        public GameObject Current { get => last; set => SetCurrent(value); }

        /// <summary>
        /// Updates the current GameObject and maintains the hierarchy chain tracking.
        /// </summary>
        /// <param name="obj">The GameObject to set as the current object in the hierarchy.</param>
        /// <remarks>
        /// <para>
        /// This method updates the hierarchy tracking state by adding the new GameObject
        /// to the tracked hierarchy. If the new object's parent is not already in the
        /// hierarchy, the entire hierarchy is reset.
        /// </para>
        /// </remarks>
        public void SetCurrent(GameObject obj)
        {
            if (obj == null)
            {
                last = null;
                hierarchy = new HashSet<GameObject>();
                return;
            }

            if (obj.transform.parent == null || !hierarchy.Contains(obj.transform.parent.gameObject))
                hierarchy = new HashSet<GameObject>();

            last = obj;
            hierarchy.Add(obj);
        }
    }
}