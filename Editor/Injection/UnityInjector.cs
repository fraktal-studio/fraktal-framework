using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using Fraktal.Framework.DI.Injector.Steps;
using Fraktal.Framework.DI.Injector.Strategies;
using Fraktal.Framework.Editor.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fraktal.Framework.Editor.Injection
{
    
    /// <summary>
    /// Static utility class providing Unity Editor menu commands and functionality for executing
    /// dependency injection across Unity scenes and GameObjects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class serves as the primary interface for triggering dependency injection operations
    /// within the Unity Editor. It provides menu commands accessible through the Unity menu bar
    /// and includes utilities for performance testing and scene-wide injection processing.
    /// </para>
    /// <para>
    /// <strong>Important:</strong> This class is designed for editor-time dependency injection only.
    /// The injection process involves reflection, asset manipulation, and editor-specific APIs that
    /// are not suitable for runtime use. Runtime dependency injection should use different mechanisms.
    /// </para>
    /// <para>
    /// The injection process operates recursively through GameObject hierarchies, processing both
    /// GameObjects and their attached Components through the configured injection pipeline.
    /// </para>
    /// </remarks>
    /// <seealso cref="InjectionPipeline"/>
    /// <seealso cref="InjectionContext"/>
    /// <seealso cref="FraktalSettings"/>
    public static class UnityInjector
    {
        /// <summary>
        /// Executes dependency injection across the active Unity scene using the configured pipeline and context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method provides the main entry point for dependency injection through Unity's menu system.
        /// It performs the following operations:
        /// </para>
        /// <list type="number">
        /// <item>Loads the global Fraktal Framework settings</item>
        /// <item>Creates injection pipeline and context from configured factories</item>
        /// <item>Processes the entire active scene recursively</item>
        /// <item>Displays results in the InjectionResultWindow</item>
        /// </list>
        /// <para>
        /// The method uses two different pipeline phases:
        /// </para>
        /// <list type="bullet">
        /// <item>Phase 0: Initial collection pipeline</item>
        /// <item>Phase 1: Processing pipeline (overrides phase 0 pipeline)</item>
        /// </list>
        /// <para>
        /// <strong>Performance Warning:</strong> This operation can be expensive on large scenes
        /// due to reflection-based field discovery and hierarchy traversal. Avoid running during
        /// runtime or on performance-critical paths.
        /// </para>
        /// <para>
        /// <strong>Editor Only:</strong> This method is only available in the Unity Editor and
        /// should not be called from runtime code.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Manually trigger injection (equivalent to menu command)
        /// UnityInjector.Inject();
        /// 
        /// // Results will be displayed in the InjectionResultWindow
        /// </code>
        /// </example>
        /// <seealso cref="InjectionResultWindow"/>
        public static void Inject(InjectionPipeline pipeline, InjectionPipeline pipeline2, InjectionContext context)
        {
            
            FraktalSettings settings = FraktalSettings.GetOrCreate();
            if (pipeline == null || pipeline2 == null || context == null)
            {
                EditorUtility.DisplayDialog(
                    "Injection Failed!",
                    "Assign the fields of pipeline correctly or create a pipeline settings (Fraktal Framework/Create/Unity Injection Pipeline)!",
                    "OK");
                return;
            }
            Inject(pipeline, context, SceneManager.GetActiveScene());
            
            if (context.Get(out IEmptyFieldsService service))
            {
                
                if (service.FieldCount == 0)
                {
                    EditorWindow.GetWindow<InjectionResultWindow>("Result").context = context;
                    return;
                }
            }
            
            Inject(pipeline2, context, SceneManager.GetActiveScene());
            
            EditorWindow.GetWindow<InjectionResultWindow>("Result").context = context;
        }
        
        /// <summary>
        /// Executes dependency injection across all root GameObjects in the specified Unity scene.
        /// </summary>
        /// <param name="pipeline">The injection pipeline to use for processing objects and components.</param>
        /// <param name="context">The injection context containing services and state for the injection process.</param>
        /// <param name="scene">The Unity scene to process for dependency injection.</param>
        /// <remarks>
        /// <para>
        /// This method provides scene-level dependency injection by:
        /// </para>
        /// <list type="number">
        /// <item>Retrieving all root GameObjects in the scene</item>
        /// <item>Processing each root GameObject and its hierarchy recursively</item>
        /// <item>Maintaining injection context state across all objects</item>
        /// </list>
        /// <para>
        /// The method delegates the actual injection work to <see cref="Inject(Pipeline{InjectionContext}, InjectionContext, GameObject)"/>
        /// for each root GameObject, ensuring that the entire scene hierarchy is processed consistently.
        /// </para>
        /// <para>
        /// Only root GameObjects are directly enumerated; child objects are processed recursively
        /// by the GameObject-specific injection method.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="pipeline"/>, <paramref name="context"/>, or <paramref name="scene"/> is null.
        /// </exception>
        /// <seealso cref="Inject(Pipeline{InjectionContext}, InjectionContext, GameObject)"/>
        public static void Inject(Pipeline<InjectionContext> pipeline, InjectionContext context, Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject go in roots)
            {
                Inject(pipeline, context, go);
            }
        }

        /// <summary>
    /// Executes dependency injection on the specified GameObject, its components, and all child GameObjects recursively.
    /// </summary>
    /// <param name="pipeline">The injection pipeline to use for processing the GameObject and its components.</param>
    /// <param name="context">The injection context containing services and state for the injection process.</param>
    /// <param name="go">The root GameObject to process along with its entire hierarchy.</param>
    /// <remarks>
    /// <para>
    /// This method implements the core recursive injection algorithm:
    /// </para>
    /// <list type="number">
    /// <item>Sets the GameObject as the current object in the injection context</item>
    /// <item>Processes the GameObject through the injection pipeline</item>
    /// <item>Iterates through all attached Components and processes each one</item>
    /// <item>Recursively processes all child GameObjects</item>
    /// </list>
    /// <para>
    /// The processing order ensures that:
    /// </para>
    /// <list type="bullet">
    /// <item>GameObjects are processed before their Components</item>
    /// <item>Parent objects are processed before their children</item>
    /// <item>The injection context maintains proper state throughout the hierarchy</item>
    /// </list>
    /// <para>
    /// This recursive approach allows strategies like <see cref="AnyParentMatch"/> and 
    /// <see cref="AnyChildrenMatch"/> to work correctly by maintaining hierarchy context
    /// throughout the injection process.
    /// </para>
    /// <para>
    /// <strong>Performance Note:</strong> Deep hierarchies may impact performance due to
    /// recursive processing. Consider scene organization to minimize excessive nesting.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown when <paramref name="pipeline"/>, <paramref name="context"/>, or <paramref name="go"/> is null.
    /// </exception>
    /// <example>
    /// <code>
    /// // Process a specific GameObject hierarchy
    /// var pipeline = settings.GetInjectionPipeline(1);
    /// var context = settings.GetInjectionContext();
    /// var rootObject = GameObject.FindGameObjectWithTag("Player");
    /// 
    /// UnityInjector.Inject(pipeline, context, rootObject);
    /// </code>
    /// </example>
    /// <seealso cref="TrackHierarchyStep"/>
    /// <seealso cref="AnyParentMatch"/>
    /// <seealso cref="AnyChildrenMatch"/>
        public static void Inject(Pipeline<InjectionContext> pipeline, InjectionContext context, GameObject go)
        {
            context.currentObject = go;
            pipeline.Process(context);
            foreach (Component component in go.GetComponents<Component>())
            {
                context.currentObject = component;
                pipeline.Process(context);
            }

            foreach (Transform child in go.transform)
            {
                Inject(pipeline, context, child.gameObject);
            }
        }
    }
}