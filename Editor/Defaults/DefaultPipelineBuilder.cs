using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.DI.Injector.Steps;

namespace Fraktal.Framework.Editor.Defaults
{
    /// <summary>
    /// Factory implementation for creating dependency injection pipelines and contexts with default configurations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This builder provides default implementations for both injection pipelines and injection contexts,
    /// implementing the factory pattern to allow for different pipeline configurations based on phase parameters.
    /// It serves as the standard configuration provider for the Fraktal Framework dependency injection system.
    /// </para>
    /// <para>
    /// The builder supports two different pipeline configurations:
    /// </para>
    /// <list type="bullet">
    /// <item><strong>Phase 0 (Collection Phase):</strong> Includes field collection for discovering dependencies</item>
    /// <item><strong>Phase 1 (Processing Phase):</strong> Skips collection, focuses on processing existing fields</item>
    /// </list>
    /// <para>
    /// The injection context is pre-configured with all standard services required for dependency injection,
    /// including field factories, hierarchy tracking, and field state management services.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> There's a duplicate registration of <see cref="IHierarchyTracker"/> in the 
    /// context creation method that should be addressed.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = new DefaultPipelineBuilder();
    /// 
    /// // Create context with all required services
    /// var context = builder.Create();
    /// 
    /// // Create collection pipeline (phase 0)
    /// var collectionPipeline = builder.Create(0);
    /// 
    /// // Create processing-only pipeline (phase 1)  
    /// var processingPipeline = builder.Create(1);
    /// </code>
    /// </example>
    /// <seealso cref="InjectionPipeline"/>
    /// <seealso cref="InjectionContext"/>
    /// <seealso cref="IFactory{TInput, TOutput}"/>
    /// <seealso cref="IFactory{T}"/>
    public class DefaultPipelineBuilder : 
        IFactory<int, InjectionPipeline>,
        IFactory<InjectionContext>
    {

        /// <summary>
        /// Creates a new <see cref="InjectionContext"/> with all required services pre-registered for dependency injection.
        /// </summary>
        /// <returns>
        /// A fully configured <see cref="InjectionContext"/> with standard dependency injection services registered.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates and configures an injection context with the following services:
        /// </para>
        /// <list type="bullet">
        /// <item><see cref="IFieldFactory"/> - For creating field wrappers from reflection data</item>
        /// <item><see cref="IHierarchyTracker"/> - For tracking GameObject hierarchy relationships</item>
        /// <item><see cref="IFailedFieldsService"/> - For tracking failed dependency injections</item>
        /// <item><see cref="IEmptyFieldsService"/> - For managing fields awaiting injection</item>
        /// <item><see cref="ISucceededFieldsService"/> - For tracking successful dependency injections</item>
        /// </list>
        /// <para>
        /// All services use their standard implementations (<see cref="ReflectionFieldFactory"/>, 
        /// <see cref="HashSetHierarchyTracker"/>, etc.) providing reliable default behavior.
        /// </para>
        /// </remarks>
        InjectionContext IFactory<InjectionContext>.Create()
        {
            InjectionContext context = new InjectionContext();
            context.Register<IFieldFactory>(new ReflectionFieldFactory());
            context.Register<IHierarchyTracker>(new HashSetHierarchyTracker());
            context.Register<IFailedFieldsService>(new FailedFieldsService());
            context.Register<IEmptyFieldsService>(new EmptyFieldsService());
            context.Register<ISucceededFieldsService>(new SucceededFieldsService());
            return context;
        }

        /// <summary>
        /// Creates a new <see cref="InjectionPipeline"/> configured for the specified processing phase.
        /// </summary>
        /// <param name="input">
        /// The phase identifier determining pipeline configuration:
        /// <list type="bullet">
        /// <item><c>0</c> - Collection phase: includes field discovery and processing</item>
        /// <item><c>Other</c> - Processing phase: processes existing fields without collection</item>
        /// </list>
        /// </param>
        /// <returns>
        /// A configured <see cref="InjectionPipeline"/> with appropriate steps for the specified phase.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates pipelines with the following standard step sequence:
        /// </para>
        /// <list type="number">
        /// <item><see cref="TrackHierarchyStep"/> - Always included for hierarchy state management</item>
        /// <item><see cref="CollectFieldStep"/> - Only in phase 0 for discovering dependency fields</item>
        /// <item><see cref="ProcessFieldStep"/> - Always included for dependency resolution</item>
        /// <item><see cref="ApplySavesStep"/> - Always included for Unity asset persistence</item>
        /// </list>
        /// <para>
        /// The phase-based approach allows for optimized pipelines where field collection can be
        /// skipped when processing pre-collected fields, improving performance in multi-pass scenarios.
        /// </para>
        /// <para>
        /// Pipeline steps are added in dependency order, ensuring proper initialization and processing
        /// flow throughout the injection process.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Full pipeline with field collection
        /// var fullPipeline = builder.Create(0);
        /// // Steps: TrackHierarchy -> CollectField -> ProcessField -> ApplySaves
        /// 
        /// // Processing-only pipeline
        /// var processingPipeline = builder.Create(1);
        /// // Steps: TrackHierarchy -> ProcessField -> ApplySaves
        /// </code>
        /// </example>
        public InjectionPipeline Create(int input)
        {
            InjectionPipeline pipeline = new InjectionPipeline();
            pipeline.Add(new TrackHierarchyStep());
            if (input == 0)
                pipeline.Add(new CollectFieldStep());
            pipeline.Add(new ProcessFieldStep());
            pipeline.Add(new ApplySavesStep());
            return pipeline;
        }
    }
}