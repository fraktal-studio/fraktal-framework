using System;
using Fraktal.Framework.Editor.Defaults;
using FraktalFramework.Editor.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Settings
{
    public class FraktalSettings : ScriptableObject
    {
        [SerializeReference]
        public IInjectionPipelineFactory pipelineBuilder;
            
        [SerializeReference]
        public IInjectionContextFactory contextBuilder;

        public bool autoInject = false;
        public bool showResults = false;
        
        public static FraktalSettings GetOrCreate()
        {
            const string assetPath = "Assets/FraktalSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<FraktalSettings>(assetPath);
            if (settings == null)
            {
                settings = CreateInstance<FraktalSettings>();
                var defaultPipelineBuilder = new DefaultPipelineBuilder();
                settings.pipelineBuilder = defaultPipelineBuilder;
                settings.contextBuilder = defaultPipelineBuilder;
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
    
   
}