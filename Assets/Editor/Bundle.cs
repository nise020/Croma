using System.IO;
using UnityEditor;
using UnityEngine;

public class Bundle : MonoBehaviour
{
    [MenuItem("Assets/AssetsBundle")]

    static void BuildIdAssetsBundle()
    {
        string dir = "Assets/StreamingAssets";

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);
    }
}
