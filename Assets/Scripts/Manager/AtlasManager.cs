using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using static Enums;
public class AtlasManager : Manager_Base
{
    public Dictionary<CONFIG_ATLAS_TYPE, SpriteAtlas> atlasSprites = new();
    public async override UniTask Initialize(string _Str)
    {
        await base.Initialize(_Str);
        atlasSprites.Clear();

        await FindAtlas();
        Initialized(_Str);
    }
    async UniTask FindAtlas()
    {
        await FindAtlas("Atlas/Key_Icon", CONFIG_ATLAS_TYPE.Key_Icon);
        await FindAtlas("Atlas/Icon", CONFIG_ATLAS_TYPE.Icon);
        await FindAtlas("Atlas/Item", CONFIG_ATLAS_TYPE.Item);
        await FindAtlas("Atlas/Skill", CONFIG_ATLAS_TYPE.Skill);
        await FindAtlas("Atlas/Buff_Icon", CONFIG_ATLAS_TYPE.Buff_Icon);
        await FindAtlas("Atlas/Damage_Icon", CONFIG_ATLAS_TYPE.Damage_Icon);
    }
    //에셋 번들 적용 예정
    public async UniTask FindAtlas(string _Path, CONFIG_ATLAS_TYPE _List)
    {
        ResourceRequest request = Resources.LoadAsync<SpriteAtlas>(_Path);
        await UniTask.WaitUntil(() => request.isDone);

        SpriteAtlas atlas = request.asset as SpriteAtlas;

        if (atlas != null)
            atlasSprites.Add(_List, atlas);
        else 
            Debug.LogWarning($"Atlas '{_Path}' not found.");
    }

    public Sprite Get(CONFIG_ATLAS_TYPE _AtlasName, string _SpriteName)
    {
        if (atlasSprites.TryGetValue(_AtlasName, out SpriteAtlas atlas))
        {
            var sprite = atlas.GetSprite(_SpriteName);

            if (sprite != null) return sprite;
            Debug.LogWarning($"Sprite '{_SpriteName}' not found in atlas '{_AtlasName}'.");
        }

        else
            Debug.LogWarning($"Atlas '{_AtlasName}' not found.");

        return null;
    }

    //internal List<GameObject> ReplaceSpritesInObjects(List<GameObject> numberImages_1, object eDAMAGE_ICON)
    //{
    //    throw new NotImplementedException();
    //}
    //public List<GameObject> AtlasLoad_List(List<GameObject> _imageObjects, AtlasType _atlasType)
    //{
    //    for (int iNum = 0; iNum < AtlasName.Count; iNum++)
    //    {
    //        if (AtlasName[iNum].ToString() == _atlasType.ToString())
    //        {
    //            switch (_atlasType)
    //            {
    //                case AtlasType.Damage:
    //                    _imageObjects = loadImages_List(_imageObjects, iNum, DamageSpritName);
    //                    break;
    //            }
    //        }
    //    }

    //    return _imageObjects;
    //}
    //private List<GameObject> loadImages_List(List<GameObject> _imageObjects,
    //  int _number, List<string> _spriteName)
    //{
    //    if (_number >= AtlasName.Count)
    //    {
    //        Debug.LogError($"AtlasName에 {_number} 인덱스가 존재하지 않습니다.");
    //        return null;
    //    }

    //    if (_imageObjects.Count != _spriteName.Count)
    //    {
    //        Debug.LogError($"이미지 오브젝트 수와 {_spriteName} 수가 일치하지 않습니다.");
    //        return null;
    //    }


    //    string atlasName = AtlasName[_number];
    //    for (int iNum = 0; iNum < _spriteName.Count; iNum++)
    //    {
    //        Image Image = _imageObjects[iNum].GetComponent<Image>();
    //        if (Image != null)
    //        {
    //            Sprite sprite = GetSpritAtlas(atlasName, _spriteName[iNum]);
    //            //Debug.Log($"Image Object Name = {Image},{sprite}");

    //            Image.overrideSprite = sprite;
    //            Image.gameObject.SetActive(false);

    //        }
    //        else
    //        {
    //            Debug.LogWarning($"{_imageObjects[iNum].name} 오브젝트에 Image 컴포넌트가 없습니다.");
    //        }
    //    }
    //    ;
    //    return _imageObjects;
    //}
}
