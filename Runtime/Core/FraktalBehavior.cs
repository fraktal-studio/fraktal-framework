using Fraktal.Framework.DI.Injector.Attributes;
using UnityEngine;

namespace Fraktal.Framework.Core
{
    /// <summary>
    /// Base class for all MonoBehaviour components that participate in the Fraktal Framework dependency injection system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class extends <see cref="SerializedMonoBehaviour"/> from the Odin serialization system to provide
    /// enhanced serialization capabilities while integrating with the Fraktal Framework's dependency injection pipeline.
    /// </para>
    /// <para>
    /// Components that inherit from <see cref="FraktalBehavior"/> can use dependency injection attributes 
    /// (such as <see cref="ByAnyAttribute"/>, <see cref="ChildrenDependencyAttribute"/>, etc.) 
    /// on their fields to automatically resolve dependencies during the injection process.
    /// </para>
    /// <para>
    /// The dependency injection system will automatically discover and process all <see cref="FraktalBehavior"/> 
    /// components during scene injection, scanning their fields for dependency attributes and attempting 
    /// to resolve them based on the configured strategies.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class PlayerController : FraktalBehavior
    /// {
    ///     [AnyDependency]
    ///     private IInputService inputService;
    ///     
    ///     [ChildrenDependency]
    ///     private Rigidbody playerRigidbody;
    ///     
    ///     void Start()
    ///     {
    ///         // Dependencies will be automatically injected before Start is called
    ///         inputService.Initialize();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class FraktalBehavior : MonoBehaviour, IFraktalObject, IFraktalSerializable, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private UnityEngine.Object[] _fraktalSerializedComponents;

        public Object[] FraktalSerializedObjects
        {
            get => _fraktalSerializedComponents; 
            set => _fraktalSerializedComponents = value;
        }


        public void OnBeforeSerialize()
        {
            FraktalSerialization.Serialize(this);
        }

        public void OnAfterDeserialize()
        {
            FraktalSerialization.Deserialize(this);
        }
    }

}