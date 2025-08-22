using Fraktal.Framework.DI.Injector.FieldManagement;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEditor;
using UnityEngine;

namespace Fraktal.Framework.Editor.Injection
{
        public class InjectionResultWindow : EditorWindow
    {
         public InjectionContext context;
        
           private Vector2 scrollPosition;

           private GUIStyle succeed;
        
           private GUIStyle failed;

          private GUIStyle GetFailed()
        {
            if (failed == null)
            {
                failed = new GUIStyle(GUI.skin.box);
                failed.normal.background = GetTex(Color.red);
            }

            return failed;
        }
        
          private GUIStyle GetSucceed()
        {
            if (succeed == null)
            {
                succeed = new GUIStyle(GUI.skin.box);
                succeed.normal.background = GetTex(Color.lawnGreen);
            }

            return succeed;
        }

         private Texture2D GetTex(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.Apply();
            return tex;
        }
        
          private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (context.Services.Get<IEmptyFieldsService>(out var fieldsService))
            {
                DrawFields(fieldsService, GetFailed());
            }

            EditorGUILayout.Space(8);  

            
            if (context.Services.Get<ISucceededFieldsService>(out var succeed))
            {
                
                DrawFields(succeed, GetSucceed());
            }
            else
            {
                Close();
                Debug.LogError("No ISucceededFieldsService found");
            }
            EditorGUILayout.EndScrollView();
            
        }

           private void DrawFields(IFieldsService service, GUIStyle style)
        {
            foreach (var kvp in service.GetFields())
            {
                foreach (var field in kvp.Value)
                {
                    Draw(kvp.Key, field, style);
                }
            }
        }

          private void Draw(Object instance,IField field, GUIStyle style)
        {
            if (style == failed)
                GUI.backgroundColor = Color.red;
            if (style == succeed)
                GUI.backgroundColor = Color.lawnGreen;
            EditorGUILayout.BeginVertical(style);
            string labelText = $"{instance.GetType().Name} " +
                               $"{field.GetFieldType().Name} " +
                               $"{field.GetFieldName()}";

            
            if (GUILayout.Button(labelText, GUI.skin.label))
            {
                GameObject targetGameObject = GetGameObjectFromInstance(instance);
                if (targetGameObject != null)
                {
                    Selection.activeGameObject = targetGameObject;
                    EditorGUIUtility.PingObject(targetGameObject);
                }
            }
            EditorGUILayout.EndVertical();
        }
        
           private GameObject GetGameObjectFromInstance(Object instance)
        {
            
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