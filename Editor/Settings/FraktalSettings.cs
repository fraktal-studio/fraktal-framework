using System;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.Utilities;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Settings
{
    /// <summary>
    /// Singleton ScriptableObject that manages global Fraktal Framework configuration and provides
    /// access to injection pipelines and contexts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class serves as the main entry point for accessing dependency injection configuration
    /// throughout the Fraktal Framework. It maintains a reference to the active <see cref="PipelineSettings"/>
    /// and provides convenient methods for creating injection pipelines and contexts.
    /// </para>
    /// <para>
    /// The class implements a singleton pattern through the <see cref="GetOrCreate"/> method,
    /// ensuring a single instance exists in the project and is automatically created if missing.
    /// The singleton instance is stored as an asset at "Assets/FraktalSettings.asset".
    /// </para>
    /// <para>
    /// This settings object is used by editor tools, menu commands, and other framework components
    /// to maintain consistent configuration across the entire project.
    /// </para>
    /// </remarks>
    [Serializable]
    public class FraktalSettings : ScriptableObject
    {
        
        /// <summary>
        /// Gets the existing FraktalSettings instance or creates a new one if it doesn't exist.
        /// Implements the singleton pattern for project-wide settings management.
        /// </summary>
        /// <returns>
        /// The singleton <see cref="FraktalSettings"/> instance for the current project.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements lazy initialization of the settings singleton:
        /// </para>
        /// <list type="number">
        /// <item>Attempts to load existing settings from "Assets/FraktalSettings.asset"</item>
        /// <item>If not found, creates a new instance</item>
        /// <item>Automatically finds and assigns the first available PipelineSettings asset</item>
        /// <item>Creates and saves the asset to the project</item>
        /// </list>
        /// <para>
        /// The method ensures that a valid settings instance always exists and is properly
        /// initialized with default configuration. If multiple PipelineSettings assets exist,
        /// the first one found will be used automatically.
        /// </para>
        /// <para>
        /// <strong>Editor Only:</strong> This method uses AssetDatabase APIs and is only
        /// available in the Unity Editor.
        /// </para>
        /// </remarks>
        /// <exception cref="UnityException">
        /// May throw if asset creation fails due to project structure or permission issues.
        /// </exception>
        public static FraktalSettings GetOrCreate()
        {
            const string assetPath = "Assets/FraktalSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<FraktalSettings>(assetPath);
            if (settings == null)
            {
                settings = CreateInstance<FraktalSettings>();
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
    
   
}