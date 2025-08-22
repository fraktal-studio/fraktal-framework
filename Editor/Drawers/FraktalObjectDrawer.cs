using System;
using System.Collections.Generic;
using System.Reflection;
using Fraktal.Framework.Core;
using Fraktal.Framework.DI.Injector.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fraktal.Framework.Editor.Drawers
{
    
    public class FraktalObjectDrawer : UnityEditor.Editor
    {
        private Dictionary<Type, List<FieldInfo>> fields = new ();

        private List<FieldInfo> GetFields(Type type)
        {
            
            if (this.fields.TryGetValue(type, out var output))
                return output;

            List<FieldInfo> fields = new List<FieldInfo>();
            foreach (
                FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DependencyAttribute dep = field.GetCustomAttribute<DependencyAttribute>();
                if (dep == null)
                    continue;
                
                fields.Add(field);
            }

            this.fields[type] = fields;
            return fields;
        }
        
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

        private void DrawDependencies()
        {
            List<FieldInfo> fields = GetFields(this.serializedObject.targetObject.GetType());
            foreach (FieldInfo field in fields)
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
            
                DrawDependency(info);
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
        private void DrawDependency(FieldInfo field)
        {
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
            if (value != null && value is not UnityEngine.Object)
                return null;
            return EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(field.Name) ,(Object)value, field.FieldType, true);
        }
    }
}