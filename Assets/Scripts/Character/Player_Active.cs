using System;
using System.Collections;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class Player : Character_Base
{
    protected void inPutKeyEvent(KeyCode type) 
    {
        switch (type)
        {
            case KeyCode.Alpha1:
                MenuSystem.Instance.UseQuickSlot(1);
                break;
            case KeyCode.Alpha2:
                MenuSystem.Instance.UseQuickSlot(2);
                break;
            case KeyCode.Alpha3:
                MenuSystem.Instance.UseQuickSlot(3);
                break;
        }

        if (IsPaused || IsSturn) { return; }

        switch (type)
        {
            case KeyCode.Mouse1:
                //walkStateChange(playerStateData.runState);
                //AvoidanceCheck();
                break;

            case KeyCode.R:
            case KeyCode.Q:
            case KeyCode.E:
            case KeyCode.Space:
                skillAttack(type);//SkillE
                break;

            case KeyCode.Z:
                StateBar.ExpUpdateEvent?.Invoke((float)50);//Test
                break;

            case KeyCode.LeftControl:
                //blockCheck();
                break;
        }
    }

    private void Dash()
    {
        StartCoroutine(DashMove());
    }

    private IEnumerator DashMove()
    {
       // float moveStep = dashSpeed * Time.deltaTime;
       // Vector3 move = deitanse * moveStep;
       // rg.MovePosition(rg.position + move);
        yield return null;
    }

    protected void inPutMoveEvent(Vector3 type) 
    {
        if (IsPaused || IsSturn) { return; }
        if (StateCheckData[Attack.ToString()] == true) return;
        moveDir = type;
        //move(moveDir);
    }
    protected void inPutMouseEvent(MOUSE_INPUT_TYPE type) 
    {
        if (IsPaused || IsSturn) { return; }
        switch (type)
        {
            case MOUSE_INPUT_TYPE.Click://mouseClick
                break;
            case MOUSE_INPUT_TYPE.Release://mouseClickUp
                                          //inPutCameraAnimation(false)
                ;
                break;
            case MOUSE_INPUT_TYPE.Hold://mouseClickDown
                attack();                     //attack();
                                              //inPutCameraAnimation(true)
                ;
                break;
        }
    }

    //protected void inPutUiEvent(KeyCode type)
    //{
    //    switch (type)
    //    {
    //        case KeyCode.I:
    //            //invenOpen();
    //            break;
    //    }
    //}
    
    private void move(Vector3 _pos)
    {
        if (this.moveDir == Vector3.zero)
        {
            //if (CharacterSoundPlayer.isPlaying) 
            //{
            //    CharacterSoundPlayer.Stop();
            //}
            return;
        }
        //else { WlakSoundOn(); }

        float rotationSpeed = 20.0f;

        //if (viewcam == null) viewcam = Camera.main;
        Transform camTrs = viewcam.transform;

        Vector3 camForward = camTrs.forward;
        Vector3 camRight = camTrs.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * _pos.z + camRight * _pos.x;
        moveDir.Normalize();

        float baseSpeed = 5f;
        float maxSpeed = 10f;
        //float speed = StatusData[CHARACTER_STATUS.Speed];

        //float t = speed / 40f;
        //t = Mathf.Clamp01(t); 
        //float finalSpeed = Mathf.Lerp(baseSpeed, maxSpeed, t);

        float t = Mathf.Clamp01(StatusData[CHARACTER_STATUS.Speed] / 40f);
        float finalSpeed = Mathf.Lerp(baseSpeed, maxSpeed, t);

        //Velocity.x = moveDir.x * finalSpeed;
        //Velocity.z = moveDir.z * finalSpeed;

        rg.MovePosition(rg.position + moveDir * finalSpeed * Time.deltaTime);

        //transform.position += moveDir * speed * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(moveDir.normalized), 
            Time.deltaTime * rotationSpeed);
    }

    private void clearWalkAnimation()
    {
        throw new NotImplementedException();
    }

    private void attack()
    {
        if (StateCheckData[Attack.ToString()] == false) 
        {
            StateCheckData[Attack.ToString()] = true;
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Skill_1, true);
        }
        //basicSkillSystem.SkillActive(PLAYER_SKILL.Shot);
    }

    private void blockCheck()
    {
       
    }

    private void shitdownCheak()
    {
        
    }
    protected ANIMATION_PATAMETERS_TYPE Inputcheck(KeyCode key ,out SKILL_ID_TYPE type) 
    {
        switch (key) 
        {               
            case KeyCode.Space:
                type = skillvalueGet(1);
                return ANIMATION_PATAMETERS_TYPE.Dash;
            case KeyCode.Q:
                type = skillvalueGet(2);
                return ANIMATION_PATAMETERS_TYPE.Skill_2;  
            case KeyCode.E:
                type = skillvalueGet(3);
                return ANIMATION_PATAMETERS_TYPE.Skill_3;
            case KeyCode.R:
                type = skillvalueGet(4);
                return ANIMATION_PATAMETERS_TYPE.Burst_1;
            default:
                type = SkillKeyWardData[0];
                return ANIMATION_PATAMETERS_TYPE.Skill_1;
        }    
    }
    private SKILL_ID_TYPE skillvalueGet(int _value) 
    {
        if (SkillKeyWardData.TryGetValue(_value, out SKILL_ID_TYPE type))
        {
            return type;
        }
        else 
        {
            return SKILL_ID_TYPE.None;
        }
    }
    private void skillAttack(KeyCode _input)
    {
        ANIMATION_PATAMETERS_TYPE type = Inputcheck(_input ,out SKILL_ID_TYPE skill);

        if (skill == SKILL_ID_TYPE.None) return;

        if (StateCheckData[Attack.ToString()] == false)
        {
            if (basicSkillSystem.CooltimeCheck(skill))
            {
                if (type == ANIMATION_PATAMETERS_TYPE.Burst_1&&
                    StateBar.BurstSkillStat() == false)
                {
                    Debug.LogError("BurstGage Not Full");
                    return;
                }

                StateCheckData[Attack.ToString()] = true;
                AnimationParameterUpdate(type, true);
            }
            
            //if (type == ANIMATION_PATAMETERS_TYPE.Skill_1)
            //{
            //    if (basicSkillSystem.CooltimeCheck(skill))
            //    {
            //        StateCheckData[Attack.ToString()] = true;
            //        AnimationParameterUpdate(type, true);
            //    }
            //}
            //else if (type == ANIMATION_PATAMETERS_TYPE.Skill_2)
            //{
            //    if (basicSkillSystem.CooltimeCheck(SkillType[3]))
            //    {
            //        StateCheckData[Attack.ToString()] = true;
            //        AnimationParameterUpdate(type, true);
            //    }
            //}
            //else 
            //{
            //    return;
            //}   
            
        }

        //StateCheckData[Attack.ToString()] = true;
        //AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Skill_1, true);
        //basicSkillSystem.SkillActive(PLAYER_SKILL.Dash);
    }

    private void skillAttack_common2()
    {
        if (StateCheckData[Attack.ToString()] == false)
        {
            StateCheckData[Attack.ToString()] = true;
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Skill_2, true);
        }

        //StateCheckData[Attack.ToString()] = true;
       // AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Skill_2, true);
    }

    private void commonRSkill()
    {
        throw new NotImplementedException();
    }

    private void AvoidanceCheck()
    {
        throw new NotImplementedException();
    }
    
    /*//**private void invenOpen()
    {
        if (inventoryObj != null)
        {
            isInvenOpen = !isInvenOpen;
            inventoryObj.SetActive(isInvenOpen);

            Debug.Log($"[Inventory] SetActive: {isInvenOpen}, obj name: {inventoryObj.name}");

            GameShard.Instance.InputManager.isUIOpen = isInvenOpen;
        }
        else
{
    Debug.Log("InvenObj is null");
}
    }*/
    protected void inputrocessing()
    {
        //while (GameShard.Instance.InputManager.KeyinPutQueData.Count > 0)//key 
        //{

        //    KeyCode type = GameShard.Instance.InputManager.KeyinPutQueData.Dequeue();

        //    switch (type)
        //    {
        //        case KeyCode.Mouse1:
        //            //walkStateChange(playerStateData.runState);
        //            //AvoidanceCheck();
        //            break;
        //        case KeyCode.R:
        //            //commonRSkill(playerStateData.PlayerType);
        //            break;
        //        case KeyCode.Q:
        //            //skillAttack_common1(playerStateData.PlayerType);//SkillQ
        //            break;
        //        case KeyCode.E:
        //            //skillAttack_common2(playerStateData.PlayerType);//SkillE
        //            break;
        //        case KeyCode.Z:
        //            StateBar.ExpUpdateEvent?.Invoke(50);
        //            break;
        //        case KeyCode.Space:
        //            //cameraModeChange();
        //            break;
        //        case KeyCode.LeftControl:
        //            //blockCheck();
        //            break;
        //    }
        //}

        //while (GameShard.Instance.InputManager.MouseInputQueData.Count > 0)//mouseClick == Attack
        //{
        //    MOUSE_INPUT_TYPE type = GameShard.Instance.InputManager.MouseInputQueData.Dequeue();
        //    switch (type)
        //    {
        //        case MOUSE_INPUT_TYPE.Click://mouseClick
        //            attack();
        //            break;
        //        case MOUSE_INPUT_TYPE.Release://mouseClickUp
        //            //inPutCameraAnimation(false)
        //            ;
        //            break;
        //        case MOUSE_INPUT_TYPE.Hold://mouseClickDown
        //            //attack();
        //            //inPutCameraAnimation(true)
        //            ;
        //            break;
        //    }

        //}

        //if (GameShard.Instance.InputManager.PlayerMoveQueData.Count != 0)
        //{
        //    //notWalkTimer = 0.0f;

        //    while (GameShard.Instance.InputManager.PlayerMoveQueData.Count > 0)//move
        //    {
        //        Vector3 type = GameShard.Instance.InputManager.PlayerMoveQueData.Dequeue();
        //        move(type);

        //        //if (playerStateData.AttackState == AttackState.Attack_On)
        //        //{
        //        //    playerStateData.AttackState = AttackState.Attack_Off;

        //        //    canReceiveInput = true;
        //        //}

        //        //if (walkStateChangeTimer >= walkStateChangeTime)
        //        //{
        //        //    playerStateData.WalkState = PlayerWalkState.Run;
        //        //}
        //        //else if (walkStateChangeTimer < walkStateChangeTime)
        //        //{
        //        //    playerStateData.WalkState = PlayerWalkState.Walk;

        //        //    walkStateChangeTimer += Time.deltaTime;
        //        //}

        //        //if (playerStateData.WalkState != PlayerWalkState.Dash)
        //        //{
        //        //    move(type);
        //        //}
        //        //else { return; }


        //    }
        //}
        //else
        //{
        //    //playerStateData.WalkState = PlayerWalkState.Stop;
        //    //clearWalkAnimation();

        //    //walkStateChangeTimer = 0.0f;
        //    //notWalkTimer += Time.deltaTime;

        //    //if (notWalkTimer > notWalkTime &&
        //    //playerStateData.WalkState != PlayerWalkState.Stop)
        //    //{
        //    //    playerStateData.WalkState = PlayerWalkState.Stop;
        //    //}
        //}

    }
}
