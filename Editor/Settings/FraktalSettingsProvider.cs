using Fraktal.Framework.Editor.Settings;
using UnityEditor;

namespace Fraktal.Framework.Editor.Settings
{
      public static class FraktalSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Fraktal", SettingsScope.Project)
            {
                label = "Fraktal",
                guiHandler = (_) =>
                {
                    var settings = FraktalSettings.GetOrCreate();
                    settings.autoInject =
                        EditorGUILayout.Toggle("Automatically Inject On Changes", settings.autoInject);
                    if (settings.autoInject)
                        settings.showResults = EditorGUILayout.Toggle("Show Results", settings.showResults);
                },
                keywords= new System.Collections.Generic.HashSet<string>(new[] { "Author", "Feature", "MyTool" })

            };
            
            return provider;
        }
    }
}