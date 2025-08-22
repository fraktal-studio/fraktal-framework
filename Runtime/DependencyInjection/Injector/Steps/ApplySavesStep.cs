using System.Linq;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEditor;

namespace Fraktal.Framework.DI.Injector.Steps
{
    /// <summary>
    /// Pipeline step that marks processed Unity objects as dirty to ensure changes are persisted by the Unity Editor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This step is typically placed at the end of the injection pipeline to ensure that any
    /// changes made during dependency injection are properly saved. It uses Unity's
    /// <see cref="EditorUtility.SetDirty(UnityEngine.Object)"/> method to mark objects as modified.
    /// </para>
    /// <para>
    /// This step is essential for editor-time dependency injection to ensure that injected
    /// dependencies are serialized and persist between editor sessions and play mode transitions.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> This step is editor-only and should not be used in runtime injection pipelines.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var pipeline = new InjectionPipeline();
    /// pipeline.Add(new CollectFieldStep());
    /// pipeline.Add(new ProcessFieldStep());
    /// pipeline.Add(new ApplySavesStep()); // Ensure changes are saved
    /// </code>
    /// </example>
    /// <seealso cref="EditorUtility.SetDirty(UnityEngine.Object)"/>
    public class ApplySavesStep : PipelineStep<InjectionContext>
    {
        /// <summary>
        /// Marks the current object in the injection context as dirty to ensure persistence of changes.
        /// </summary>
        /// <param name="input">The injection context containing the object to mark as dirty.</param>
        /// <returns>The same <see cref="InjectionContext"/> passed as input.</returns>
        /// <remarks>
        /// <para>
        /// This method calls <see cref="EditorUtility.SetDirty(UnityEngine.Object)"/> on the
        /// <see cref="InjectionContext.currentObject"/> to notify Unity's serialization system
        /// that the object has been modified and needs to be saved.
        /// </para>
        /// <para>
        /// The step passes through the context unchanged, making it safe to place anywhere
        /// in the pipeline without affecting subsequent processing steps.
        /// </para>
        /// </remarks>
        public override InjectionContext Process(InjectionContext input)
        {
            if (!input.Services.Get(out IChangesTracker changesTracker))
                return input;

            var changed = changesTracker.GetChanges().ToArray();
            foreach (UnityEngine.Object obj in changed)
            {
                changesTracker.RemoveChanges(obj);
                EditorUtility.SetDirty(obj);
            }
            return input;
        }
    }
}