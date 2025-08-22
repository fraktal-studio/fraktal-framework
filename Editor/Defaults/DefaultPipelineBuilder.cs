using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.DI.Injector.Steps;
using FraktalFramework.Editor.Interfaces;

namespace Fraktal.Framework.Editor.Defaults
{
    public class DefaultPipelineBuilder : 
        IInjectionPipelineFactory,
        IInjectionContextFactory
    {

        /// </para>
        /// </remarks>
        InjectionContext IFactory<InjectionContext>.Create()
        {
            InjectionContext context = new InjectionContext();
            context.Services.Register<IFieldFactory>(new ReflectionFieldFactory());
            context.Services.Register<IHierarchyTracker>(new HashSetHierarchyTracker());
            context.Services.Register<IFailedFieldsService>(new FailedFieldsService());
            context.Services.Register<IChangesTracker>(new ChangesTracker());
            context.Services.Register<IEmptyFieldsService>(new EmptyFieldsService());
            context.Services.Register<ISucceededFieldsService>(new SucceededFieldsService());
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