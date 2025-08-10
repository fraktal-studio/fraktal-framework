using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.Editor.Defaults;
using Fraktal.Framework.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Injection
{
    /// <summary>
    /// Unity Editor window for configuring and executing dependency injection operations
    /// within the Fraktal Framework ecosystem.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="InjectionWindow"/> provides a graphical user interface for developers
    /// to configure and execute dependency injection processes through Unity's editor interface.
    /// It allows configuration of injection pipelines and contexts before performing
    /// automatic dependency resolution across scene objects.
    /// </para>
    /// <para>
    /// The window supports a two-phase injection pipeline system, enabling complex
    /// dependency resolution scenarios where different injection strategies may be
    /// required at different stages of the process.
    /// </para>
    /// <para>
    /// Key features include:
    /// </para>
    /// <list type="bullet">
    /// <item>Visual configuration of injection pipeline builders</item>
    /// <item>Context builder selection for dependency resolution scope</item>
    /// <item>Two-phase pipeline execution support</item>
    /// <item>Validation of required components before injection execution</item>
    /// <item>Integration with Unity's EditorWindow system</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Access the injection window through Unity's menu system:
    /// // Tools -> Fraktal Framework -> Injection
    /// 
    /// // Configure pipeline builders and context, then click "Inject" to execute
    /// // dependency injection across all applicable scene objects
    /// </code>
    /// </example>
    /// <seealso cref="DefaultPipelineBuilder"/>
    public class InjectionWindow : EditorWindow
    {
        /// <summary>
        /// Type drawer utility for rendering factory selection fields in the editor GUI.
        /// </summary>
        /// <remarks>
        /// This drawer provides specialized rendering capabilities for factory type selection,
        /// enabling users to choose appropriate factory implementations through dropdown
        /// interfaces or type selection controls.
        /// </remarks>
        private TypeDrawer drawer = new();
        
        /// <summary>
        /// Factory for creating the first phase injection pipeline.
        /// </summary>
        /// <remarks>
        /// The first phase pipeline typically handles primary dependency resolution,
        /// such as service location and basic component injection before more complex
        /// dependency relationships are established.
        /// </remarks>
        private IFactory<int, InjectionPipeline> pipelineBuilderStep;
        
        /// <summary>
        /// Factory for creating the injection context that defines the scope and
        /// parameters for dependency resolution operations.
        /// </summary>
        /// <remarks>
        /// The injection context provides metadata and configuration for the injection
        /// process, including scope boundaries, resolution strategies, and filtering
        /// criteria for target objects.
        /// </remarks>
        private IFactory<InjectionContext> contextBuilder;

        /// <summary>
        /// Opens the Injection Window through Unity's editor menu system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates and displays the injection configuration window, making it
        /// accessible through Unity's "Tools/Fraktal Framework/Injection" menu path.
        /// The window is implemented as a singleton, ensuring only one instance exists
        /// at any given time.
        /// </para>
        /// <para>
        /// The window title is set to "Auto Inject" to clearly indicate its purpose
        /// within the Unity editor interface.
        /// </para>
        /// </remarks>
        [MenuItem("Tools/Fraktal Framework/Injection")]
        public static void ShowWindow()
        {
            GetWindow<InjectionWindow>("Auto Inject");
        }
        
        /// <summary>
        /// Renders the injection window's graphical user interface using Unity's immediate mode GUI system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a vertical layout containing configuration controls for:
        /// </para>
        /// <list type="bullet">
        /// <item>First phase pipeline builder selection</item>
        /// <item>Second phase pipeline builder selection</item>
        /// <item>Injection context builder selection</item>
        /// <item>Execution button for triggering the injection process</item>
        /// </list>
        /// <para>
        /// The GUI uses Unity's EditorGUILayout system to provide a consistent editor
        /// experience that integrates seamlessly with other Unity editor windows.
        /// All configuration changes are immediately reflected in the interface and
        /// validated before injection execution.
        /// </para>
        /// </remarks>
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawPipelineBuilder(
               ref pipelineBuilderStep,
               "Injection Pipeline Builder"
            );
            
            DrawContext(
                ref contextBuilder,
                "Injection Context Builder");
            if (GUILayout.Button("Inject", GUILayout.ExpandWidth(true)))
                TryInject();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Attempts to execute the dependency injection process using the configured pipeline builders and context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs validation of all required components before proceeding with
        /// injection execution. If any required factories are missing or null, the injection
        /// process is aborted to prevent runtime errors.
        /// </para>
        /// <para>
        /// Upon successful validation, the method creates instances of the configured pipelines
        /// and context, then delegates to <see cref="UnityInjector.Inject"/> for actual
        /// dependency resolution execution.
        /// </para>
        /// <para>
        /// The two-phase pipeline approach allows for complex dependency resolution scenarios
        /// where certain dependencies must be resolved before others can be processed.
        /// </para>
        /// </remarks>
        private void TryInject()
        {
            if (HasMissing())
            {
                return;
            }
            
            UnityInjector.Inject(pipelineBuilderStep.Create(0), pipelineBuilderStep.Create(1), contextBuilder.Create());
            Close();
        }

        /// <summary>
        /// Validates that all required factory instances are properly configured before injection execution.
        /// </summary>
        /// <returns>
        /// <c>true</c> if any required factory is null or missing; otherwise, <c>false</c> 
        /// indicating all required components are available for injection.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This validation method ensures that the injection process cannot be executed
        /// in an incomplete or invalid state. It checks for the presence of all three
        /// required factory instances:
        /// </para>
        /// <list type="bullet">
        /// <item><see cref="contextBuilder"/> - Required for injection context creation</item>
        /// <item><see cref="pipelineBuilderStep"/> - Required for first phase pipeline creation</item>
        /// <item><see cref="pipelineBuilderStep2"/> - Required for second phase pipeline creation</item>
        /// </list>
        /// <para>
        /// This method serves as a safety mechanism to prevent runtime exceptions that
        /// could occur if injection is attempted with missing dependencies.
        /// </para>
        /// </remarks>
        private bool HasMissing()
        {
            if (
                contextBuilder == null ||
                pipelineBuilderStep == null
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Renders a pipeline builder configuration control with a descriptive label and factory selection field.
        /// </summary>
        /// <param name="fact">
        /// A reference to the <see cref="IFactory{InjectionPipeline}"/> that will be modified
        /// by user interaction with the rendered control.
        /// </param>
        /// <param name="label">
        /// The descriptive text label displayed alongside the factory selection control
        /// to identify the purpose of this pipeline builder.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method creates a horizontal layout containing a text label and a factory
        /// selection control. The factory selection is handled by the <see cref="drawer"/>
        /// instance, which provides appropriate UI controls for type selection and
        /// factory instantiation.
        /// </para>
        /// <para>
        /// The method modifies the factory reference directly through the ref parameter,
        /// ensuring that user selections are immediately reflected in the window's state
        /// and available for validation and execution.
        /// </para>
        /// </remarks>
        private void DrawPipelineBuilder(ref IFactory<int,InjectionPipeline> fact, string label)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label);
            fact = drawer.TypeField(fact);
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Renders an injection context builder configuration control with a descriptive label and factory selection field.
        /// </summary>
        /// <param name="fact">
        /// A reference to the <see cref="IFactory{InjectionContext}"/> that will be modified
        /// by user interaction with the rendered control.
        /// </param>
        /// <param name="label">
        /// The descriptive text label displayed alongside the factory selection control
        /// to identify the purpose of this context builder.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method provides the same functionality as <see cref="DrawPipelineBuilder"/>
        /// but is specifically typed for injection context factories. It creates a horizontal
        /// layout with a descriptive label and factory selection control.
        /// </para>
        /// <para>
        /// The context builder factory is crucial for defining the scope and parameters
        /// of the injection process, including which objects should be targeted for
        /// dependency injection and how dependencies should be resolved.
        /// </para>
        /// </remarks>
        private void DrawContext(ref IFactory<InjectionContext> fact, string label)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label);
            fact = drawer.TypeField(fact);
            EditorGUILayout.EndHorizontal();
        }
    }
}