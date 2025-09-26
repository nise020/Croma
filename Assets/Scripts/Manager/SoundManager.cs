using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using static OptionHandler;
using static UnityEngine.Rendering.DebugUI;
public enum SOUND_TABLE_TYPE : int
{
    None = 0,
    Bgm = 1,
    Effect = 2,
    Ui = 3,
    Butten = 4,
}
public enum BGM_ID_TYPE : int
{
    //BGM
    None = 0,
    Title = 20001,
    Stage1 = 20002,
    Stage2 = 20003,
    Stage3 = 20004,
    Stage4 = 20005,
    RanKing = 20006,
    Ui = 3,
    Boss = 20051
}

public class SoundManager : MonoBehaviour
{
    public AudioSource BgmPlayer;
    public AudioSource SFXPlayer;
    public AudioMixer audioMixer;
    class SoundInfo
    {
        public AudioClip Sound;
        public SoundTable.Info TableData;
    }

    //Dictionary<int, SoundInfo> BgmData = new Dictionary<int, SoundInfo>();

    public async UniTask Init()
    {
        Shared.Instance.SoundManager = this;
        BgmPlayer       = gameObject.AddComponent<AudioSource>();
        SFXPlayer       = gameObject.AddComponent<AudioSource>();
        LoadMixser().Forget(); ;
        //await UniTask.WhenAll(
        //     AudioFind()
        //     //BgmSetting()
        //    );

        await UniTask.Yield();
    }
    public async UniTask LoadMixser() 
    {
        audioMixer = Resources.Load<AudioMixer>("Sound/MasterAudioMixer");
        BgmPlayer.outputAudioMixerGroup = GetMixser(OptionHandler.SOUND.Bgm);
        await UniTask.CompletedTask;
    }
    public AudioMixerGroup GetMixser(SOUND _type)
    {
        string groupName = _type.ToString();

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(groupName);

        if (groups.Length > 0)
        {
            return groups[0];
        }
        else
        {
            Debug.LogError($"'{groupName}' Not Found.");
        }

        return null;
    }
    public void BgmPlaying(bool _check) 
    {
        if (!_check) 
        {
            BgmPlayer.Stop();
            return;
        }
        BgmPlayer.Play();
    }

    public async UniTask BgmPlayerSetting(int _id) 
    {
        //BgmPlayer.Stop();
        AudioClip audioClip = ClipGet(_id);

        BgmPlayer.clip = audioClip;
        BgmPlayer.loop = true;
        BgmPlayer.spatialBlend = 0.00f;
        BgmPlayer.volume = 0.5f;
        BgmPlayer.pitch = 1.0f;

        BgmPlayer.Play();

        await UniTask.Yield();
    }

    //public async UniTask AudioFind()
    //{
    //    var table = Shared.Instance.DataManager.Sound_Table.SoundTableData;
    //    int BgmId = (int)BGM_ID_TYPE.Title;

    //    foreach (var sound in table)
    //    {
    //        SoundInfo data = new();
    //        int id = sound.Key;
    //        SoundTable.Info info = sound.Value;

    //        AudioClip audoi = Resources.Load<AudioClip>(info.Path);

    //        if (info.Type == (int)SOUND_TABLE_TYPE.Bgm)
    //        {
    //            data.TableData = info;
    //            data.Sound = audoi;

    //            BgmData.Add(BgmId, data);
    //            BgmId++;
    //        }
    //    }
    //    await UniTask.Yield();
    //}
    
    //2D
    public AudioClip ClipGet(int _id) 
    {
        var table = Shared.Instance.DataManager.Sound_Table.Get(_id);

        if (table != null) 
        {
            AudioClip audoi = Resources.Load<AudioClip>(table.Path);
            return audoi;
        }
        return null;
    }

    //3D
    public AudioSource SoundSetting(int _id, AudioSource _audio) 
    {
        var table = Shared.Instance.DataManager.Sound_Table.Get(_id);

        if (table != null) 
        {
            AudioClip audoi = Resources.Load<AudioClip>(table.Path);

            _audio.spatialBlend = 1.0f; 
            _audio.clip = audoi;
            _audio.volume = table.Volume * 0.01f;
            _audio.pitch = table.Pitch * 0.01f;
            _audio.priority = table.Priority;
            _audio.minDistance = table.MinDistance;
            _audio.maxDistance = table.MaxDistance;

            return _audio;
        }
        return null;
    }

    public void PlaySFXOneShot(int id)
    {
        var clip = ClipGet(id);
        if (clip) 
            SFXPlayer.PlayOneShot(clip);
    }

    //public async UniTask BgmSetting() 
    //{
    //    await UniTask.Yield();
    //}

    //public void PlatBgm(string _Bgm)
    //{
    //    //2019 ���� ������ ������ �־ �Ʒ� ó�� ����
    //    Object obj = Resources.Load(_Bgm);

    //    if (obj == null)
    //        return;

    //    AudioClip clip = obj as AudioClip;

    //    if (null == clip)
    //        return;

    //    BgmPlayer.clip = clip;
    //    BgmPlayer.Play();
    //    //Bgm.Stop(); �ʱ�ȭ
    //    //Bgm.mute = false; �Ͻ�����
    //}
    //public void PlayEffect(string _Effect)
    //{
    //    Object obj = Resources.Load(_Effect);

    //    if (null == obj)
    //        return;

    //    AudioClip clip = obj as AudioClip;

    //    if (null == clip)
    //        return;

    //    //Effect.PlayOneShot(clip);//�ѹ��� ����
    //}
}
