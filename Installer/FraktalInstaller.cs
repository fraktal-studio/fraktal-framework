using UnityEditor;

namespace Fraktal.Framework.Installer
{
    using System.Linq;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;
    using UnityEngine;

    
    
    [InitializeOnLoad]
    public static class FraktalInstaller
    {
        private static ListRequest listRequest;
        private static AddRequest addRequest;
        private static bool tried = false;
        private struct FraktalDependency
        {
            public string id, git;
        }
        
        static readonly FraktalDependency[] Dependencies = 
        {
            new()
            {
                git = "https://github.com/fraktal-studio/design-patterns.git",
                id = "com.fraktal.design-patterns"
            }
        };

        static FraktalInstaller()
        {
            
            EditorApplication.delayCall += StartList;
        }

        static void StartList()
        {
            listRequest = Client.List(true);
            EditorApplication.update += Poll;
        }

        static void Poll()
        {
            if (listRequest != null && listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                {
                    foreach (var dep in Dependencies)
                    {
                        bool installed = listRequest.Result.Any(p => p.name == dep.id);
                        if (!installed)
                        {
                            tried = true;
                            Debug.Log($"Fraktal: adding missing package {dep.id} from Git...");
                            addRequest = Client.Add(dep.git);
                            break; 
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Fraktal: package list failed: {listRequest.Error?.message}");
                    EditorApplication.update -= Poll;
                }

                listRequest = null;
            }

            if (addRequest != null && addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"Fraktal: successfully added {addRequest.Result.name}.");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning($"Fraktal: failed to add package: {addRequest.Error?.message}");
                    if (addRequest.Result.name == Dependencies[^1].id)
                    {
                        EditorApplication.update -= Poll;
                        return;
                    }
                }
                
                addRequest = null;
                tried = false;
                listRequest = Client.List(true);
            }
        }
    }

}