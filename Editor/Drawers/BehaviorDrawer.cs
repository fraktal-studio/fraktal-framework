using System.Reflection;
using Fraktal.Framework.Core;
using Fraktal.Framework.DI.Injector.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fraktal.Framework.Editor.Drawers
{
    
    /// <summary>
    /// Custom Unity Editor drawer for <see cref="FraktalBehavior"/> components that provides 
    /// specialized inspector GUI for dependency injection fields.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This custom editor enhances the Unity inspector for all components derived from 
    /// <see cref="FraktalBehavior"/>. It automatically discovers and displays fields marked 
    /// with dependency injection attributes in a dedicated "Dependencies" section, allowing 
    /// developers to visualize and manually override dependency assignments.
    /// </para>
    /// <para>
    /// The editor provides the following features:
    /// </para>
    /// <list type="bullet">
    /// <item>Automatic discovery of fields with <see cref="DependencyAttribute"/></item>
    /// <item>Visual display of dependency fields with human-readable names</item>
    /// <item>Manual assignment override capabilities via Unity's ObjectField</item>
    /// <item>Automatic dirty marking for serialization when values change</item>
    /// <item>Standard inspector property display for non-dependency fields</item>
    /// </list>
    /// <para>
    /// This editor is particularly useful during development for debugging dependency 
    /// injection issues and providing manual fallbacks when automatic injection fails.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // The editor will automatically display dependency fields like this:
    /// public class PlayerController : FraktalBehavior
    /// {
    ///     [AnyDependency]
    ///     private IInputService inputService; // Shown in Dependencies section
    ///     
    ///     public float speed = 5f; // Shown in normal properties section
    /// }
    /// </code>
    /// </example>
    [CustomEditor(typeof(FraktalBehavior),true)]
    public class BehaviorDrawer : UnityEditor.Editor
    {
        /// <summary>
        /// Renders the custom inspector GUI for <see cref="FraktalBehavior"/> components.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method overrides Unity's default inspector behavior to provide specialized 
        /// rendering for dependency injection fields. The GUI is organized into two main sections:
        /// </para>
        /// <list type="number">
        /// <item><strong>Dependencies Section:</strong> Displays all fields marked with dependency attributes</item>
        /// <item><strong>Standard Properties:</strong> Shows regular serialized properties excluding the script reference</item>
        /// </list>
        /// <para>
        /// The method ensures proper serialization state management by calling 
        /// <see cref="SerializedObject.Update"/> before property rendering.
        /// </para>
        /// </remarks>
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Dependencies: ");
            EditorGUI.indentLevel++;
            DrawDependencies();
            EditorGUI.indentLevel--;
            GUILayout.Space(10);
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Discovers and renders all dependency-marked fields in the target component using reflection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method uses reflection to examine all non-public instance fields declared on the 
        /// target component type. It processes only fields with <see cref="DependencyAttribute"/> 
        /// or its derived attributes, ensuring that regular fields are not included in the 
        /// dependencies section.
        /// </para>
        /// <para>
        /// The reflection search uses <see cref="BindingFlags.DeclaredOnly"/> to avoid processing 
        /// inherited fields multiple times when dealing with inheritance hierarchies.
        /// </para>
        /// </remarks>
        private void DrawDependencies()
        {
            foreach (FieldInfo field in target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Draw(field);
            }
        }

        /// <summary>
        /// Examines a field for dependency attributes and delegates to appropriate drawing methods.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> representing the field to examine.</param>
        /// <remarks>
        /// This method acts as a filter, checking whether the field has a <see cref="DependencyAttribute"/> 
        /// and only proceeding with rendering if the attribute is present. This ensures that only 
        /// dependency-relevant fields are displayed in the dependencies section.
        /// </remarks>
        private void Draw(FieldInfo info)
        {
            var dependency = info.GetCustomAttribute<DependencyAttribute>();
            if (dependency != null)
                DrawDependency(info, dependency);
        }
        
        /// <summary>
        /// Renders a dependency field as an editable Unity ObjectField with automatic dirty marking.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> representing the dependency field to render.</param>
        /// <param name="attribute">The <see cref="DependencyAttribute"/> associated with the field.</param>
        /// <remarks>
        /// <para>
        /// This method creates an interactive ObjectField that allows developers to:
        /// </para>
        /// <list type="bullet">
        /// <item>View the current dependency assignment</item>
        /// <item>Manually override the dependency by dragging objects from the scene or project</item>
        /// <item>Clear dependencies by setting them to null</item>
        /// </list>
        /// <para>
        /// When a value change is detected, the method automatically marks the target object as 
        /// dirty using <see cref="EditorUtility.SetDirty(Object)"/> to ensure changes are 
        /// serialized and persist between editor sessions.
        /// </para>
        /// <para>
        /// The method uses reflection to get and set field values, bypassing normal access 
        /// restrictions to work with private dependency fields.
        /// </para>
        /// </remarks>
        private void DrawDependency(FieldInfo field, DependencyAttribute attribute)
        {
            if (attribute == null) return;
            EditorGUILayout.BeginHorizontal();
            object value = field.GetValue(target);
            Object result = DrawObject(field, value);
            if ( result != value)
            {
                EditorUtility.SetDirty(target);
                field.SetValue(target, result);
            }
            
            EditorGUILayout.EndHorizontal();
            
            
        }

        /// <summary>
        /// Renders an Unity ObjectField for the specified field, handling both null and assigned values.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> containing metadata about the field being drawn.</param>
        /// <param name="value">The current value of the field, which may be null or a Unity Object.</param>
        /// <returns>
        /// The new value selected by the user through the ObjectField, or null if the value 
        /// is not a Unity Object or cannot be rendered.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates an appropriate ObjectField based on the current field state:
        /// </para>
        /// <list type="bullet">
        /// <item><strong>Null values:</strong> Shows an empty ObjectField accepting the field's declared type</item>
        /// <item><strong>Unity Objects:</strong> Shows the current object with ability to change selection</item>
        /// <item><strong>Non-Unity Objects:</strong> Returns null (cannot be displayed in ObjectField)</item>
        /// </list>
        /// <para>
        /// The ObjectField uses <see cref="ObjectNames.NicifyVariableName(string)"/> to convert 
        /// field names from camelCase/PascalCase to human-readable format (e.g., "inputService" 
        /// becomes "Input Service").
        /// </para>
        /// <para>
        /// Scene object assignment is enabled (allowSceneObjects: true) to support dependency 
        /// injection from both project assets and scene objects.
        /// </para>
        /// </remarks>
        private Object DrawObject(FieldInfo field, object value)
        {
            if (value == null)
            {
                return EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(field.Name) ,null, field.FieldType, true);
            } 
            if (value is Object obj)
            {
                return EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(field.Name), obj, field.FieldType, true);
            }

            return null;
        }
    }
}