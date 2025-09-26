using Cysharp.Threading.Tasks;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Manager_Base : MonoBehaviour
{
    public virtual UniTask Initialize(string _Str)
    {
        Debug.Log($"Initializing {_Str}...");
        return UniTask.CompletedTask;
    }
    public void Initialized(string _Str) => Debug.Log($"Initialized {_Str}");
    public void RunTaggedFunctions(string _Tag)
    {
        foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            var methods = mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<FuncTagAttribute>();
                if (attr != null && attr.Tag == _Tag)
                {
                    method.Invoke(mono, null);
                    Debug.Log($"½ÇÇàµÊ: {mono.GetType().Name}.{method.Name} (ÅÂ±×: {attr.Tag})");
                }
            }
        }
    }
    public GameObject FindObject(GameObject[] _Objs, string _Name) => _Objs.FirstOrDefault(go => go.name == _Name);
}
