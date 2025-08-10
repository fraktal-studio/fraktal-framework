using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Services;
using UnityEngine;

namespace Fraktal.Framework.DependencyInjection.Injector.FieldManagement.Abstract
{
    /// <summary>
    /// Marker interface for field strategies that perform immediate dependency resolution during field collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One-shot field strategies differ from regular <see cref="IFieldStrategy"/> implementations in that 
    /// they attempt to resolve dependencies immediately when a field is discovered during the collection phase, 
    /// rather than waiting for the processing phase of the injection pipeline.
    /// </para>
    /// <para>
    /// This is useful for strategies that can determine and resolve dependencies without needing to wait 
    /// for the complete object graph to be processed. Examples might include:
    /// </para>
    /// <list type="bullet">
    /// <item>Self-injection (injecting the component into its own field)</item>
    /// <item>Static service injection (from a known service locator)</item>
    /// <item>Immediate parent/child resolution when the hierarchy is already established</item>
    /// </list>
    /// <para>
    /// Fields using one-shot strategies are processed during the <see cref="CollectFieldStep"/> and 
    /// are immediately moved to either the succeeded or failed collections, bypassing the normal 
    /// processing phase entirely.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class SelfInjectStrategy : IOneShotFieldStrategy
    /// {
    ///     public bool Process(UnityEngine.Object obj, IField field, InjectionContext context)
    ///     {
    ///         // Inject the component into its own field if types are compatible
    ///         var instance = field.GetInstance();
    ///         if (instance == obj && field.IsAssignable(obj)))
    ///         {
    ///             field.SetValue(obj);
    ///             return true;
    ///         }
    ///         return false;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IOneShotFieldStrategy : IFieldStrategy{}
}