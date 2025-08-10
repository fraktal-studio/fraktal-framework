using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Injection
{
    /// <summary>
    /// Editor window that displays the results of dependency injection operations, showing succeeded and failed field injections
    /// with visual feedback and navigation capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This window provides a visual debugging interface for the Fraktal Framework's dependency injection system.
    /// It displays fields in different states (empty/failed and succeeded) with color-coded backgrounds and 
    /// allows developers to navigate directly to GameObjects containing dependency fields.
    /// </para>
    /// <para>
    /// The window is typically opened automatically after running the dependency injection process through 
    /// the <see cref="UnityInjector.Inject()"/> method, providing immediate feedback about injection results.
    /// </para>
    /// <para>
    /// Fields are organized into two main categories:
    /// </para>
    /// <list type="bullet">
    /// <item>Empty/Failed fields - displayed with red background indicating unresolved dependencies</item>
    /// <item>Succeeded fields - displayed with green background indicating successfully resolved dependencies</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Window is typically opened programmatically after injection
    /// var window = EditorWindow.GetWindow&lt;InjectionResultWindow&gt;("Injection Results");
    /// window.context = injectionContext;
    /// window.Show();
    /// </code>
    /// </example>
    public class InjectionResultWindow : EditorWindow
    {
        /// <summary>
        /// The injection context containing the results of the dependency injection process.
        /// </summary>
        /// <remarks>
        /// This context contains all the field services that track the state of dependency injection,
        /// including empty fields, succeeded fields, and failed fields. The window queries these
        /// services to display the current injection status.
        /// </remarks>
        public InjectionContext context;
        
        /// <summary>
        /// The current scroll position for the scrollable area of the window.
        /// </summary>
        /// <remarks>
        /// This maintains the user's scroll position when the window content extends beyond the visible area,
        /// providing a better user experience when reviewing large numbers of dependency fields.
        /// </remarks>
        private Vector2 scrollPosition;

        /// <summary>
        /// Cached GUIStyle for displaying succeeded dependency fields with green background.
        /// </summary>
        /// <remarks>
        /// This style is lazily initialized to avoid unnecessary texture creation and provides
        /// visual feedback for successfully resolved dependencies.
        /// </remarks>
        private GUIStyle succeed;
        
        /// <summary>
        /// Cached GUIStyle for displaying failed dependency fields with red background.
        /// </summary>
        /// <remarks>
        /// This style is lazily initialized to avoid unnecessary texture creation and provides
        /// visual feedback for unresolved dependencies that require attention.
        /// </remarks>
        private GUIStyle failed;

        /// <summary>
        /// Gets or creates the GUIStyle used for displaying failed dependency fields.
        /// </summary>
        /// <returns>A GUIStyle with red background for indicating failed dependency resolution.</returns>
        /// <remarks>
        /// This method implements lazy initialization to create the style only when needed.
        /// The style uses a red background texture to provide clear visual feedback about
        /// injection failures that need developer attention.
        /// </remarks>
        private GUIStyle GetFailed()
        {
            if (failed == null)
            {
                failed = new GUIStyle(GUI.skin.box);
                failed.normal.background = GetTex(Color.red);
            }

            return failed;
        }
        
        /// <summary>
        /// Gets or creates the GUIStyle used for displaying succeeded dependency fields.
        /// </summary>
        /// <returns>A GUIStyle with green background for indicating successful dependency resolution.</returns>
        /// <remarks>
        /// This method implements lazy initialization to create the style only when needed.
        /// The style uses a green background texture to provide clear visual feedback about
        /// successful dependency injections.
        /// </remarks>
        private GUIStyle GetSucceed()
        {
            if (succeed == null)
            {
                succeed = new GUIStyle(GUI.skin.box);
                succeed.normal.background = GetTex(Color.lawnGreen);
            }

            return succeed;
        }

        /// <summary>
        /// Creates a 1x1 pixel texture with the specified color for use in GUIStyles.
        /// </summary>
        /// <param name="col">The color to use for the texture.</param>
        /// <returns>A Texture2D with the specified color, marked as HideAndDontSave.</returns>
        /// <remarks>
        /// <para>
        /// This utility method creates small textures used for background colors in custom GUIStyles.
        /// The texture is marked with <see cref="HideFlags.HideAndDontSave"/> to prevent it from
        /// appearing in the project assets and being saved with the scene.
        /// </para>
        /// <para>
        /// The texture is applied immediately using <see cref="Texture2D.Apply()"/> to ensure
        /// the pixel data is uploaded to the GPU for rendering.
        /// </para>
        /// </remarks>
        private Texture2D GetTex(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.Apply();
            return tex;
        }
        
        /// <summary>
        /// Handles the GUI rendering for the injection result window.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method renders the complete user interface for the injection results window,
        /// including:
        /// </para>
        /// <list type="bullet">
        /// <item>A scrollable area for viewing all dependency fields</item>
        /// <item>Empty/failed fields displayed with red styling at the top</item>
        /// <item>A visual separator between failed and succeeded sections</item>
        /// <item>Succeeded fields displayed with green styling at the bottom</item>
        /// </list>
        /// <para>
        /// The method queries the injection context for field services and displays their
        /// contents using color-coded styling to provide immediate visual feedback about
        /// the injection process results.
        /// </para>
        /// <para>
        /// Note: The <see cref="IFailedFieldsService"/> section is currently commented out,
        /// as empty fields are being displayed as failed fields instead.
        /// </para>
        /// </remarks>
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (context.Get<IEmptyFieldsService>(out var fieldsService))
            {
                DrawFields(fieldsService, GetFailed());
            }

            EditorGUILayout.Space(8);  

            
            if (context.Get<ISucceededFieldsService>(out var succeed))
            {
                DrawFields(succeed, GetSucceed());
            }
            EditorGUILayout.EndScrollView();
            
        }

        /// <summary>
        /// Draws all fields from the specified field service using the provided GUI style.
        /// </summary>
        /// <param name="service">The field service containing the fields to display.</param>
        /// <param name="style">The GUIStyle to use for rendering the fields.</param>
        /// <remarks>
        /// This method iterates through all fields managed by the provided service and
        /// renders each one using the <see cref="Draw(IField, GUIStyle)"/> method with
        /// the specified styling. This allows for consistent presentation of fields
        /// regardless of their source service.
        /// </remarks>
        private void DrawFields(IFieldsService service, GUIStyle style)
        {
            foreach (var field in service.GetFields())
            {
                Draw(field, style);
            }
        }

        /// <summary>
        /// Renders an individual dependency field with the specified styling and interactive navigation.
        /// </summary>
        /// <param name="field">The field to display in the interface.</param>
        /// <param name="style">The GUIStyle to use for the field's visual appearance.</param>
        /// <remarks>
        /// <para>
        /// This method creates an interactive display for a dependency field that includes:
        /// </para>
        /// <list type="bullet">
        /// <item>Color-coded background based on injection success/failure status</item>
        /// <item>Formatted text showing component name, field type, and field name</item>
        /// <item>Clickable interface that selects and highlights the containing GameObject</item>
        /// </list>
        /// <para>
        /// The field information is formatted as: "ComponentName FieldType FieldName" to provide
        /// comprehensive context about the dependency. Clicking on the field will automatically
        /// select the GameObject in the hierarchy and ping it for easy navigation.
        /// </para>
        /// <para>
        /// The GUI background color is set based on the style type to provide immediate visual
        /// feedback about the field's injection status.
        /// </para>
        /// </remarks>
        private void Draw(IField field, GUIStyle style)
        {
            if (style == failed)
                GUI.backgroundColor = Color.red;
            if (style == succeed)
                GUI.backgroundColor = Color.lawnGreen;
            EditorGUILayout.BeginVertical(style);
            string labelText = $"{field.GetInstance().GetType().Name} " +
                               $"{field.GetFieldType().Name} " +
                               $"{field.GetFieldName()}";

            
            if (GUILayout.Button(labelText, GUI.skin.label))
            {
                GameObject targetGameObject = GetGameObjectFromField(field);
                if (targetGameObject != null)
                {
                    Selection.activeGameObject = targetGameObject;
                    EditorGUIUtility.PingObject(targetGameObject);
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Extracts the GameObject associated with the specified dependency field.
        /// </summary>
        /// <param name="field">The field to extract the GameObject from.</param>
        /// <returns>
        /// The GameObject containing the field, or <c>null</c> if the field is not associated 
        /// with a GameObject or Component.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method handles two primary scenarios for field-to-GameObject mapping:
        /// </para>
        /// <list type="bullet">
        /// <item>Component fields: Returns the GameObject that contains the Component</item>
        /// <item>GameObject fields: Returns the GameObject directly</item>
        /// </list>
        /// <para>
        /// This mapping is essential for the navigation functionality, allowing users to click
        /// on dependency fields in the results window and automatically select the relevant
        /// GameObject in the Unity hierarchy for further inspection or modification.
        /// </para>
        /// <para>
        /// Returns <c>null</c> for field instances that are neither Components nor GameObjects,
        /// which should be rare in typical dependency injection scenarios.
        /// </para>
        /// </remarks>
        private GameObject GetGameObjectFromField(IField field)
        {
            var instance = field.GetInstance();
            
            if (instance is Component component)
            {
                return component.gameObject;
            }
            if (instance is GameObject gameObject)
            {
                return gameObject;
            }
            
            return null;
        }

    }
}