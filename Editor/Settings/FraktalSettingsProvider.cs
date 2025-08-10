using Fraktal.Framework.Editor.Settings;
using UnityEditor;

namespace Fraktal.Framework.Editor.Settings
{
    /// <summary>
    /// Provides Unity Project Settings integration for Fraktal Framework configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This static class creates a settings provider that integrates the Fraktal Framework configuration
    /// into Unity's Project Settings window. Users can access and modify framework settings through
    /// the standard Unity settings interface at "Project/Fraktal".
    /// </para>
    /// </remarks>
    public static class FraktalSettingsProvider
    {
        /// <summary>
        /// Creates and configures the Unity Project Settings provider for Fraktal Framework.
        /// </summary>
        /// <returns>
        /// A configured <see cref="SettingsProvider"/> that displays Fraktal Framework settings
        /// in Unity's Project Settings window.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is called automatically by Unity's settings system due to the 
        /// <see cref="SettingsProviderAttribute"/>. The returned provider:
        /// </para>
        /// <list type="bullet">
        /// <item>Appears under "Project/Fraktal" in the Project Settings window</item>
        /// <item>Provides an ObjectField for selecting the active PipelineSettings asset</item>
        /// <item>Automatically saves changes to the FraktalSettings asset</item>
        /// <item>Includes searchable keywords for discoverability</item>
        /// </list>
        /// <para>
        /// The GUI handler automatically creates or loads the singleton <see cref="FraktalSettings"/>
        /// instance and provides a simple interface for pipeline configuration selection.
        /// </para>
        /// </remarks>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Fraktal", SettingsScope.Project)
            {
                label = "Fraktal",
                guiHandler = (_) =>
                {
                    var settings = FraktalSettings.GetOrCreate();
                    
                },
                keywords= new System.Collections.Generic.HashSet<string>(new[] { "Author", "Feature", "MyTool" })

            };
            
            return provider;
        }
    }
}