using System.Collections;
using Fraktal.Framework.Editor.Injection;
using Fraktal.Framework.Editor.Settings;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FraktalFramework.Editor.Automation
{
    [InitializeOnLoad]
    public class AutoInjector
    {
        private static bool inject = false;
        private static bool hasInjected = false;
        private static double nextCheckTime;
        
        static AutoInjector()
        {
            EditorApplication.hierarchyChanged += HieracrhyChanged;
            EditorApplication.update += Loop;
        }

        private static void HieracrhyChanged()
        {
            inject = true;
                
        }

        private static void Loop()
        {
            if (inject && hasInjected){
                hasInjected = false;
                inject = false;
            }
            
            // Time-based delay
            if (EditorApplication.timeSinceStartup < nextCheckTime)
                return;

            nextCheckTime = EditorApplication.timeSinceStartup + 1; // schedule next run

            if (inject && ! hasInjected)
            {
                Inject();
                inject = false;
                hasInjected = true;
                
                return;
            }
        }

        private static void Inject()
        {
            FraktalSettings settings = FraktalSettings.GetOrCreate();
            if (!settings.autoInject)
                return;
            
            
            UnityInjector.Inject(
                settings.pipelineBuilder.Create(0),
                settings.pipelineBuilder.Create(1),
                settings.contextBuilder.Create(),
                settings.showResults);
        }
    }
}