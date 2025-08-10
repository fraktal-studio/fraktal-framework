using System;
using Fraktal.DesignPatterns;


namespace Fraktal.Framework.DI.Injector.Pipeline
{
    /// <summary>
    /// Specialized pipeline implementation for dependency injection operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="InjectionPipeline"/> extends the generic <see cref="Pipeline{T}"/> class 
    /// to provide a strongly-typed pipeline specifically designed for processing 
    /// <see cref="InjectionContext"/> objects through a series of injection steps.
    /// </para>
    /// <para>
    /// This pipeline is the core orchestration mechanism for the dependency injection system, 
    /// managing the flow of processing through various stages such as:
    /// </para>
    /// <list type="number">
    /// <item>Hierarchy tracking and context setup</item>
    /// <item>Field discovery and collection</item>
    /// <item>Dependency resolution and injection</item>
    /// <item>Result tracking and persistence</item>
    /// </list>
    /// <para>
    /// The pipeline is typically configured with a series of <see cref="IPipelineStep{InjectionContext}"/> 
    /// implementations that each perform a specific aspect of the injection process. The pipeline 
    /// can be customized by adding, removing, or reordering steps to accommodate different 
    /// injection requirements.
    /// </para>
    /// <para>
    /// Being serializable, pipeline configurations can be saved as assets and reused across 
    /// different scenes or projects, providing consistent dependency injection behavior.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Creating and configuring an injection pipeline
    /// var pipeline = new InjectionPipeline();
    /// pipeline.Add(new TrackHierarchyStep());
    /// pipeline.Add(new CollectFieldStep());
    /// pipeline.Add(new ProcessFieldStep());
    /// pipeline.Add(new ApplySavesStep());
    /// 
    /// // Processing a context through the pipeline
    /// var context = new InjectionContext();
    /// context.currentObject = someGameObject;
    /// var result = pipeline.Process(context);
    /// </code>
    /// </example>
    [Serializable]
    public class InjectionPipeline : Pipeline<InjectionContext>
    {
        
    }
}