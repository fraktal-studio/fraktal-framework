using System;
using System.Reflection;
using Fraktal.DesignPatterns;
using Fraktal.Framework.Core;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.DependencyInjection.Injector.FieldManagement.Abstract;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Steps
{
    /// <summary>
    /// Pipeline step responsible for discovering and collecting fields marked with dependency attributes from FraktalBehavior components.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This step performs the initial discovery phase of dependency injection by scanning all fields in 
    /// <see cref="FraktalBehavior"/> components for dependency attributes. It uses reflection to examine 
    /// field metadata and creates <see cref="IField"/> wrappers for fields that require injection.
    /// </para>
    /// <para>
    /// The step handles two types of field strategies:
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="IOneShotFieldStrategy"/>: Immediate resolution during collection phase</item>
    /// <item>Regular strategies: Deferred to the <see cref="ProcessFieldStep"/> for later resolution</item>
    /// </list>
    /// <para>
    /// Fields with existing values (non-null) are skipped to avoid overwriting manually assigned dependencies.
    /// One-shot strategies are processed immediately and their results are recorded in the appropriate 
    /// service collections.
    /// </para>
    /// <para>
    /// Only processes components that inherit from <see cref="FraktalBehavior"/> to ensure proper 
    /// serialization support through Odin serialization integration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Usage in pipeline configuration
    /// var pipeline = new InjectionPipeline();
    /// pipeline.Add(new TrackHierarchyStep());
    /// pipeline.Add(new CollectFieldStep()); // Discover dependency fields
    /// pipeline.Add(new ProcessFieldStep());
    /// </code>
    /// </example>
    /// <seealso cref="ProcessFieldStep"/>
    /// <seealso cref="IFieldFactory"/>
    /// <seealso cref="IOneShotFieldStrategy"/>
    [Serializable]
    public class CollectFieldStep :IPipelineStep<InjectionContext>
    {
        /// <summary>
        /// Processes the injection context to collect dependency-marked fields from the current object.
        /// </summary>
        /// <param name="input">
        /// The injection context containing the current object to process and registered services.
        /// </param>
        /// <returns>
        /// The same <see cref="InjectionContext"/> with updated field collections containing 
        /// discovered dependency fields.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method orchestrates the field collection process by:
        /// </para>
        /// <list type="number">
        /// <item>Retrieving or registering required services from the injection context</item>
        /// <item>Validating that the current object is a <see cref="FraktalBehavior"/></item>
        /// <item>Delegating to <see cref="Collect"/> for the actual field discovery</item>
        /// </list>
        /// <para>
        /// Objects that are not <see cref="FraktalBehavior"/> components are skipped entirely,
        /// as they cannot participate in the dependency injection system.
        /// </para>
        /// </remarks>
        public InjectionContext Proccess(InjectionContext input)
        {
            IEmptyFieldsService emptyFieldsService = input.GetOrRegister<IEmptyFieldsService>();
            IFailedFieldsService failedFieldsService = input.GetOrRegister<IFailedFieldsService>();
            IFieldFactory fieldFactory = input.GetOrRegister<IFieldFactory>();
            ISucceededFieldsService succeeded = input.GetOrRegister<ISucceededFieldsService>();

            UnityEngine.Object go = input.currentObject;
            if (go is not FraktalBehavior component) return input;
            Collect(component, input,emptyFieldsService, fieldFactory, failedFieldsService,succeeded);
            
            return input;

        }

        /// <summary>
        /// Discovers and processes all instance fields in the specified component using reflection.
        /// </summary>
        /// <param name="component">The Unity component to scan for dependency fields.</param>
        /// <param name="context">The injection context for accessing services and state.</param>
        /// <param name="service">Service for storing fields that need deferred processing.</param>
        /// <param name="fieldFactory">Factory for creating <see cref="IField"/> wrappers from reflection data.</param>
        /// <param name="failedFieldsService">Service for tracking fields that failed immediate resolution.</param>
        /// <param name="succeeded">Service for tracking fields that were successfully resolved immediately.</param>
        /// <remarks>
        /// <para>
        /// This method uses reflection with <see cref="BindingFlags.DeclaredOnly"/> to examine only 
        /// fields declared directly on the component's type, avoiding inherited fields that may 
        /// have already been processed or belong to base framework classes.
        /// </para>
        /// <para>
        /// The reflection flags include both public and non-public instance fields, allowing 
        /// dependency injection on private fields marked with dependency attributes.
        /// </para>
        /// </remarks>
        private void Collect(Component component,InjectionContext context, IFieldsService service, IFieldFactory fieldFactory,IFieldsService failedFieldsService,ISucceededFieldsService succeeded)
        {
            Type t = component.GetType();
            foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                ProcessField(component, field,context, service, fieldFactory, failedFieldsService,succeeded);
            }
        }

        /// <summary>
        /// Processes an individual field to determine if it requires dependency injection and handles it appropriately.
        /// </summary>
        /// <param name="component">The Unity component containing the field.</param>
        /// <param name="fieldInfo">Reflection metadata for the field being processed.</param>
        /// <param name="context">The injection context providing access to services and state.</param>
        /// <param name="service">Service for storing fields that need deferred processing.</param>
        /// <param name="fieldFactory">Factory for creating <see cref="IField"/> wrappers.</param>
        /// <param name="failedFieldsService">Service for tracking fields that failed immediate resolution.</param>
        /// <param name="succeeded">Service for tracking fields that were successfully resolved immediately.</param>
        /// <remarks>
        /// <para>
        /// This method implements the core field processing logic:
        /// </para>
        /// <list type="number">
        /// <item>Creates an <see cref="IField"/> wrapper using the field factory</item>
        /// <item>Skips fields that already have non-null values</item>
        /// <item>Processes <see cref="IOneShotFieldStrategy"/> fields immediately</item>
        /// <item>Queues regular strategy fields for deferred processing</item>
        /// </list>
        /// <para>
        /// One-shot strategies are executed immediately during collection, with results 
        /// recorded in either the succeeded or failed services based on the outcome.
        /// This allows certain types of dependencies (like self-injection) to be resolved 
        /// without waiting for the processing phase.
        /// </para>
        /// </remarks>
        private void ProcessField(Component component, FieldInfo fieldInfo,InjectionContext context, IFieldsService service, IFieldFactory fieldFactory,IFieldsService failedFieldsService,ISucceededFieldsService succeeded)
        {
            IField field = fieldFactory.Create(fieldInfo, component);
            if (field == null) return;

            if (field.GetValue() != null)
                return;
            
            if (field.GetStrategy() is not IOneShotFieldStrategy oneShotStrategy)
            {
                service.AddField(field);
                return;
            }

            if (oneShotStrategy.Process(component, field,context))
            {
                succeeded.AddField(field);
            } else failedFieldsService.AddField(field);
                
        }
    }
}