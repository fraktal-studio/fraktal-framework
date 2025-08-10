using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.DI.Injector.Strategies;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Steps
{
    /// <summary>
    /// Pipeline step that maintains GameObject hierarchy tracking state for dependency resolution strategies 
    /// that depend on parent-child relationships.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This step is essential for hierarchy-aware dependency resolution strategies such as 
    /// <see cref="AnyChildrenMatch"/> and <see cref="AnyParentMatch"/>. It ensures that the 
    /// <see cref="IHierarchyTracker"/> service maintains accurate information about the current 
    /// GameObject being processed and its position in the scene hierarchy.
    /// </para>
    /// <para>
    /// The step automatically registers a <see cref="HashSetHierarchyTracker"/> if no hierarchy 
    /// tracker is already registered, ensuring that hierarchy-dependent strategies can function 
    /// properly regardless of the pipeline configuration.
    /// </para>
    /// <para>
    /// Only processes objects that are GameObjects directly. Component objects do not update 
    /// the hierarchy tracker since they inherit their hierarchy position from their containing GameObject.
    /// </para>
    /// <para>
    /// This step should typically be placed early in the pipeline, before field collection and 
    /// processing steps that may rely on hierarchy information.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Typical pipeline configuration with hierarchy tracking
    /// var pipeline = new InjectionPipeline();
    /// pipeline.Add(new TrackHierarchyStep()); // Must come first for hierarchy-aware strategies
    /// pipeline.Add(new CollectFieldStep());
    /// pipeline.Add(new ProcessFieldStep());
    /// </code>
    /// </example>
    /// <seealso cref="IHierarchyTracker"/>
    /// <seealso cref="HashSetHierarchyTracker"/>
    /// <seealso cref="AnyChildrenMatch"/>
    /// <seealso cref="AnyParentMatch"/>
    public class TrackHierarchyStep : IPipelineStep<InjectionContext>
    {
        /// <summary>
        /// Updates the hierarchy tracker with the current GameObject being processed in the injection context.
        /// </summary>
        /// <param name="input">
        /// The injection context containing the current object and services registry.
        /// </param>
        /// <returns>
        /// The same <see cref="InjectionContext"/> with updated hierarchy tracking state.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method performs the following operations:
        /// </para>
        /// <list type="number">
        /// <item>Ensures an <see cref="IHierarchyTracker"/> service is registered</item>
        /// <item>Updates the tracker's current GameObject if the current object is a GameObject</item>
        /// <item>Skips processing for non-GameObject objects (such as Components)</item>
        /// </list>
        /// <para>
        /// The method uses lazy service registration through <c>GetOrRegister</c> to ensure 
        /// a hierarchy tracker is always available, defaulting to <see cref="HashSetHierarchyTracker"/> 
        /// if none was explicitly configured.
        /// </para>
        /// <para>
        /// Component objects are ignored because their hierarchy position is determined by 
        /// their containing GameObject, which will be processed separately.
        /// </para>
        /// </remarks>
        public InjectionContext Proccess(InjectionContext input)
        {
            if (!input.Services.GetOrRegister<IHierarchyTracker>(out var result, () => new HashSetHierarchyTracker()))
            {
                return input;
            }
            if (input.currentObject is GameObject go)
                result.Current = go;

            return input;
        }
    }
}