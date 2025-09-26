using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class Bundle : MonoBehaviour
{
    [SerializeField] List<string> bundleName;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ILoad());//Load
    }

    IEnumerator ILoad()
    {
        foreach (var bundle in bundleName)
        {
            AssetBundleCreateRequest async =
            AssetBundle.LoadFromFileAsync(Path.Combine
            (Application.streamingAssetsPath, $"{bundle}"));//"" <- Path(name)

            yield return async;

            AssetBundle local = async.assetBundle;

            if (local == null)
                yield break;

            AssetBundleRequest asset = local.LoadAssetAsync<GameObject>($"{bundle}");
            //bundle <- file name

            yield return asset;

            var prefab = asset.asset as GameObject;

            if (prefab != null)
            {
                local.Unload(true);
            }
        }
    }

    IEnumerator AtlrasILoad()//new
    {
        foreach (var bundle in bundleName)
        {
            AssetBundleCreateRequest async =
            AssetBundle.LoadFromFileAsync(Path.Combine
            (Application.streamingAssetsPath, $"AtlasItem"));//"" <- Path(name)

            yield return async;

            AssetBundle local = async.assetBundle;

            if (local == null)
                yield break;

            AssetBundleRequest asset = local.LoadAssetAsync<GameObject>($"Item");

            var obj = local.LoadAsset("Item");

            SpriteAtlas sa = asset.asset as SpriteAtlas;

            //bundle <- file name


            //yield return asset;

            //if (prefab != null)
            //{
            //    local.Unload(true);
            //}
        }
    }
}
