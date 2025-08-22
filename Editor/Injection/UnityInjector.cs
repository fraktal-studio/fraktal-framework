using System.Threading.Tasks;
using Fraktal.DesignPatterns;
using Fraktal.Framework.DI.Injector.Pipeline;
using Fraktal.Framework.DI.Injector.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fraktal.Framework.Editor.Injection
{
    
    public static class UnityInjector
    {
        
        public static InjectionContext Inject(InjectionPipeline pipeline, InjectionPipeline pipeline2, InjectionContext context, bool showResults =true)
        {
            Scene scene = SceneManager.GetActiveScene();
            Inject(pipeline, context, scene);

            if (context.Services.Get(out IEmptyFieldsService service) && service.GetFields().Count == 0)
            {
                if (showResults)
                    ShowResults(context);
                return context;
            }
            Inject(pipeline2, context, scene);
            if (showResults)
                ShowResults(context);
            return context;
        }

        private static void ShowResults(InjectionContext context)
        {
            EditorWindow.GetWindow<InjectionResultWindow>().context = context;
        }
        
        
        public static void Inject(Pipeline<InjectionContext> pipeline, InjectionContext context, Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject go in roots)
            {
                Inject(pipeline, context, go);
            }
        }
        
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