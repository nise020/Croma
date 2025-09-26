using Cysharp.Threading.Tasks;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class Player : Character_Base
{
    public void VoiceSoundPlay() //Event
    {
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip voiceClip))
        {
            CharacterSoundPlayer.volume = 0.7f;
            CharacterSoundPlayer.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            CharacterSoundPlayer.PlayOneShot(voiceClip);
        }
    }
    public void AttackSoundPlay() //Event
    {
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.AttackSoundId], out AudioClip attackClip))
        {
            WeaponSoundPlayer.volume = 0.6f;
            WeaponSoundPlayer.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            WeaponSoundPlayer.PlayOneShot(attackClip);
        }

    }
    public void WlakSoundPlay()//Event
    {
        //if (CharacterSoundPlayer.isPlaying) return;
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip clip))
        {
            CharacterSoundPlayer.volume = 0.4f;
            CharacterSoundPlayer.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            CharacterSoundPlayer.PlayOneShot(clip);
        }

    }
    public void WlakSoundOn() 
    {
        //if (CharacterSoundPlayer.isPlaying) return;
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip clip))
        {
            CharacterSoundPlayer.volume = 0.8f;
            CharacterSoundPlayer.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            CharacterSoundPlayer.PlayOneShot(clip);
        }

    }
    public override void DeathEvent() //Event
    {
        GameShard.Instance?.StageManager?.PlayerDied();


       /* GameShard.Instance.GameManager.FadeEvent(SCENE_SCENES.Title, async () =>
        {
            GameShard.Instance.GameUiManager.CanvasDelet();
            await UniTask.CompletedTask;
        }).Forget();*/
    }

    public override void SkillEventOn(int value) //Event, 1 == skill1
    {
        if (SkillKeyWardData.TryGetValue(value, out SKILL_ID_TYPE skill))
        {
            basicSkillSystem.SkillActive(skill);
            GameShard.Instance.GameUiManager.PlayerStateBar.SkillActive(value, skill);
            viewcam.CameraAttackMoveOn(true,2).Forget();
        }
        else
        {
            Debug.LogError($"{skill} Skill = null");
        }
    }

    
    public override void SkillEventEnd(int _skillnumber) //Event <- Attack End Event
    {

        if (SkillKeyWardData.TryGetValue(_skillnumber, out SKILL_ID_TYPE skill))
        {
            StateCheckData[Attack.ToString()] = false;
            //basicSkillSystem.SkillOff(skill);
        }
        else
        {
            Debug.LogError($"{skill} Skill = null");
        }

        while (reservedSkills.Count > 0)
        {
            var (value, type) = reservedSkills.Dequeue();
            SkillChange(value, type);
        }
        //CharacterAnimator.SetInteger(_skillnumber.ToString(), 0);

    }
    public void AnimationOut(string _paramaterType)//Event 
    {
        if (StateCheckData[Attack.ToString()] == true) 
        {
            if (_paramaterType == ANIMATION_PATAMETERS_TYPE.Skill_1.ToString()
                && SkillKeyWardData.TryGetValue(0, out SKILL_ID_TYPE skill1))
            {
                StateCheckData[Attack.ToString()] = false;
                //basicSkillSystem.SkillOff(skill1);
            }
            else if (_paramaterType == ANIMATION_PATAMETERS_TYPE.Dash.ToString()
                     && SkillKeyWardData.TryGetValue(1, out SKILL_ID_TYPE skill2))
            {
                StateCheckData[Attack.ToString()] = false;
                //basicSkillSystem.SkillOff(skill2);
            }
            else if (_paramaterType == ANIMATION_PATAMETERS_TYPE.Skill_2.ToString()
                 && SkillKeyWardData.TryGetValue(2, out SKILL_ID_TYPE skil3))
            {
                StateCheckData[Attack.ToString()] = false;
                //basicSkillSystem.SkillOff(skil3);
            }
            else if (_paramaterType == ANIMATION_PATAMETERS_TYPE.Skill_3.ToString()
                 && SkillKeyWardData.TryGetValue(3, out SKILL_ID_TYPE skil4))
            {
                StateCheckData[Attack.ToString()] = false;
                //basicSkillSystem.SkillOff(skil3);
            }
            else if (_paramaterType == ANIMATION_PATAMETERS_TYPE.Burst_1.ToString()
                && SkillKeyWardData.TryGetValue(4, out SKILL_ID_TYPE skil5))
            {
                StateCheckData[Attack.ToString()] = false;
                //basicSkillSystem.SkillOff(skil3);
            }
            else
            {
                Debug.LogError($"{_paramaterType} = {StateCheckData[Attack.ToString()]}");
            }
           // viewcam.CameraAttackMoveOn(false,0).Forget();
            CharacterAnimator.SetInteger(_paramaterType.ToString(), 0);
        }  
    }

    //public void AttackSkill(string _paramaterType) 
    //{
    //    if (_paramaterType == Attack.ToString())
    //    {
    //        basicSkillSystem.SkillActive(SKILL_ID_TYPE.Shot);
    //    }
    //    else if (_paramaterType == Skill_1.ToString())
    //    {
    //        basicSkillSystem.SkillActive(SKILL_ID_TYPE.Dash);
    //    }
    //    else if (_paramaterType == Skill_2.ToString())
    //    {
    //        basicSkillSystem.SkillActive(SKILL_ID_TYPE.Shot);
    //    }
    //}
    //public void EtcAnimation()
    //{

    //}
    //public void AnimationEventOn(string _paramaterType)
    //{
    //    if (_paramaterType == (Attack.ToString())||
    //        _paramaterType == (Skill_1.ToString())||
    //        _paramaterType == (Skill_2.ToString()))
    //    {
    //        AttackSkill(_paramaterType);
    //    }
    //    //else if (_paramaterType == Skill_1.ToString()) 
    //    //{
    //    //    EtcAnimation();
    //    //}
        
    //}
    

    //public void AnimationEnd(string _text) 
    //{
    //    if (_text == ANIMATION_PATAMETERS_TYPE.Attack.ToString()) 
    //    {
    //        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Attack, false);
    //    }
    //}

    public override GameObject GetWeaponObj()
    {
        SkinnedMeshRenderer skin = WeaponObj.GetComponent<SkinnedMeshRenderer>();
        Transform trs = skin.rootBone;
        return  trs.GetChild(0).gameObject;
        //return WeaponObj;
    }

    public override void KnockBackOn(float _value)
    {
        float knockbackForce = Mathf.Clamp(Mathf.Log10(_value + 1) * 2f, 1f, 10f);

        Vector3 knockBackDir = -transform.forward;
        knockBackDir.y = 0f;
        knockBackDir.Normalize();
        if (StateCheckData[Attack.ToString()] == false && !IsPaused)
        {
            IsPaused = true;
            AnimationParameterUpdate(Walk, false);
            AnimationParameterUpdate(KnockBack, true);
            viewcam.CameraAttackMoveOn(true, 4).Forget();
        }
        rg.AddForce(knockBackDir * knockbackForce, ForceMode.Impulse);
    }
    public override void KnockBackEvent()//AnimationEvent
    {
        base.KnockBackEvent();
        //viewcam.CameraAttackMoveOn(false, 0).Forget();
    }
}
