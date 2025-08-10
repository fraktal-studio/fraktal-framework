using System;
using System.Collections.Generic;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DI.Injector.Steps
{
    /// <summary>
    /// Pipeline step that processes collected dependency fields by attempting to resolve their dependencies 
    /// using the current object as a potential injection candidate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This step represents the main dependency resolution phase of the injection pipeline. It takes 
    /// fields that were collected by <see cref="CollectFieldStep"/> and attempts to resolve their 
    /// dependencies by testing whether the current object being processed can satisfy each field's requirements.
    /// </para>
    /// <para>
    /// The step processes only fields that are in the "empty" state (awaiting resolution). For each 
    /// empty field, it delegates to the field's associated <see cref="IFieldStrategy"/> to determine 
    /// if the current object is suitable for injection.
    /// </para>
    /// <para>
    /// Successfully resolved fields are moved from the <see cref="IEmptyFieldsService"/> to the 
    /// <see cref="ISucceededFieldsService"/>, providing a clear audit trail of the injection process.
    /// </para>
    /// <para>
    /// This step is typically placed after <see cref="CollectFieldStep"/> and before <see cref="ApplySavesStep"/> 
    /// in the pipeline configuration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Typical pipeline configuration
    /// var pipeline = new InjectionPipeline();
    /// pipeline.Add(new TrackHierarchyStep());
    /// pipeline.Add(new CollectFieldStep());
    /// pipeline.Add(new ProcessFieldStep()); // Resolve dependencies
    /// pipeline.Add(new ApplySavesStep());
    /// </code>
    /// </example>
    /// <seealso cref="CollectFieldStep"/>
    /// <seealso cref="IFieldStrategy"/>
    /// <seealso cref="IEmptyFieldsService"/>
    [Serializable]
    public class ProcessFieldStep : IPipelineStep<InjectionContext>
    {
        /// <summary>
        /// Processes the injection context to attempt dependency resolution for all collected empty fields.
        /// </summary>
        /// <param name="input">
        /// The injection context containing the current object to test for injection and the field collections.
        /// </param>
        /// <returns>
        /// The same <see cref="InjectionContext"/> with updated field collections reflecting any successful resolutions.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements the core dependency resolution algorithm:
        /// </para>
        /// <list type="number">
        /// <item>Retrieves the current object from the injection context</item>
        /// <item>Gets a snapshot of all empty fields awaiting resolution</item>
        /// <item>For each field, tests if the current object can satisfy its dependency</item>
        /// <item>Moves successfully resolved fields to the succeeded collection</item>
        /// </list>
        /// <para>
        /// The method creates a defensive copy of the empty fields collection before iteration 
        /// to avoid collection modification exceptions when fields are removed during processing.
        /// </para>
        /// <para>
        /// Fields that fail to resolve during this pass remain in the empty fields collection 
        /// and may be resolved by subsequent objects processed through the pipeline.
        /// </para>
        /// </remarks>
        public InjectionContext Proccess(InjectionContext input)
        {
            var fieldsService = input.GetOrRegister<IEmptyFieldsService>();
            var succeededFields = input.GetOrRegister<ISucceededFieldsService>();
            
            UnityEngine.Object obj = input.currentObject;
            ICollection<IField> emptyFields = fieldsService.GetFields();
            foreach (IField field in emptyFields)
            {
                if (field.Process(obj, input))
                {
                    fieldsService.RemoveField(field);
                    succeededFields.AddField(field);
                }
            }
            
            return input;
        }
    }
}