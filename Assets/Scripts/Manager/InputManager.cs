using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static Enums.MOUSE_INPUT_TYPE;

public class InputManager : MonoBehaviour
{
    [Header("Player Queue")]
    //Player
    public Queue<KeyCode> KeyinPutQueData = new Queue<KeyCode>();
    public Queue<MOUSE_INPUT_TYPE> MouseInputQueData = new Queue<MOUSE_INPUT_TYPE>();
    public Queue<Vector3> PlayerMoveQueData = new Queue<Vector3>();
    bool isMoving = false;

    [Header("Camera Queue")]
    //Camera
    public Queue<Vector2> MouseMoveQueData = new Queue<Vector2>();
    public Queue<float> MouseScrollQueData = new Queue<float>();

    float Speed = 10.0f;

    [Header("UI Queue")]
    //Ui
    public Queue<KeyCode> UiKeyInputQueData = new Queue<KeyCode>();
    public bool isUIOpen = false;
    public bool isMouse = false;
    public bool isFade;



    [Header("Player Action")]
    public Action<KeyCode> KeyinPutEventData;
    public Action<MOUSE_INPUT_TYPE> MouseInputEventData;
    public Action<Vector3> PlayerMoveEventData;
    public Action<KeyCode> KeyinPutUiEventData;

    [Header("Camera Action")]
    //Camera
    public Action<Vector2> MouseMoveEventData;
    public Action<float> MouseScrollEventData;

    //public Action<KeyCode> Ui
    //public UiState uiState = UiState.Ui_Off;

    public async UniTask InitAsync()
    {
        GameShard.Instance.InputManager = this;
        isMouse = true;
        //Cursor.lockState = CursorLockMode.Locked;
        await UniTask.Yield(); // 필요시 프레임 분산
    }
    private void Update()
    {
        inputEvent();
    }

    public void inputEvent()
    {
        if (isFade) { return; }
        //Queue.Add(Input)
        UiButtonInput();
        PlayerInput();

        //Action.Add(Queue)
        ControllEventOn();
    }


    public void ControllEventOn() 
    {
        while (UiKeyInputQueData.Count > 0)
        {
            KeyCode type = UiKeyInputQueData.Dequeue();
            KeyinPutUiEventData?.Invoke(type);
        }
        while (KeyinPutQueData.Count > 0)
        {
            KeyCode type = KeyinPutQueData.Dequeue();
            KeyinPutEventData?.Invoke(type);
        }
        while (MouseInputQueData.Count > 0)
        {
            MOUSE_INPUT_TYPE type = MouseInputQueData.Dequeue();
            MouseInputEventData?.Invoke(type);
        }
        while (MouseMoveQueData.Count > 0)
        {
            Vector2 type = MouseMoveQueData.Dequeue();
            MouseMoveEventData?.Invoke(type);
        }
        //while (PlayerMoveQueData.Count > 0)
        //{
        //    Vector3 type = PlayerMoveQueData.Dequeue();
        //    PlayerMoveEventData?.Invoke(type);
        //}
        while (MouseScrollQueData.Count > 0)
        {
            float type = MouseScrollQueData.Dequeue();
            MouseScrollEventData?.Invoke(type);
        }
        
    }

    private void UiButtonInput()
    {
        if (Input.GetKey(KeyCode.LeftAlt)) 
        {
            isMouse = true; // Inventory
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt)) 
        {
            isMouse = false; // Inventory
        }
        if (Input.GetKeyDown(KeyCode.I))
            UiKeyInputQueData.Enqueue(KeyCode.I); // Inventory

        if (Input.GetKeyDown(KeyCode.K))
            UiKeyInputQueData.Enqueue(KeyCode.K); // Inventory

        if (Input.GetKeyDown(KeyCode.P))
            UiKeyInputQueData.Enqueue(KeyCode.P); // Inventory

        if (Input.GetKeyDown(KeyCode.L))
            UiKeyInputQueData.Enqueue(KeyCode.L); // Inventory

        if (Input.GetKeyDown(KeyCode.Escape))
            UiKeyInputQueData.Enqueue(KeyCode.Escape); // Menu

    }

    public void PlayerInput()
    {
        if (isUIOpen) 
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (isMouse)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Direction
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (move.magnitude > 0.1f)
        {
            PlayerMoveEventData?.Invoke(move);
            isMoving = true;

            //PlayerMoveQueData.Clear(); // 이전 값 버리기
            //PlayerMoveQueData.Enqueue(move);
        }
        else 
        {
            if (isMoving) 
            {
                isMoving = false;
                //PlayerMoveQueData.Enqueue(Vector3.zero);
            }
        }

        //Mouse
        //if (Input.GetMouseButton(0))
        //    MouseInputQueData.Enqueue(Click);//mouseClick

        //if (Input.GetMouseButtonUp(0))
        //    MouseInputQueData.Enqueue(Release);//mouseClickUp

        if (Input.GetMouseButtonDown(0))
            MouseInputQueData.Enqueue(Hold);//mouseClickDown

        //if (Input.GetKeyDown(KeyCode.Mouse1))
        //    KeyinPutQueData.Enqueue(KeyCode.Mouse1);//RunCheck

        //Key
        if (Input.GetKeyDown(KeyCode.Alpha1))
            KeyinPutQueData.Enqueue(KeyCode.Alpha1); // QuickSlot1

        if (Input.GetKeyDown(KeyCode.Alpha2))
            KeyinPutQueData.Enqueue(KeyCode.Alpha2); // QuickSlot2

        if (Input.GetKeyDown(KeyCode.Alpha3))
            KeyinPutQueData.Enqueue(KeyCode.Alpha3); // QuickSlot3

        if (Input.GetKeyDown(KeyCode.R))
            KeyinPutQueData.Enqueue(KeyCode.R);//Burst

        if (Input.GetKeyDown(KeyCode.Q))
            KeyinPutQueData.Enqueue(KeyCode.Q);//Skill1

        if (Input.GetKeyDown(KeyCode.E))
            KeyinPutQueData.Enqueue(KeyCode.E);//Skill2

        if (Input.GetKeyDown(KeyCode.Space))
            KeyinPutQueData.Enqueue(KeyCode.Space);//Space

        //if (Input.GetKeyDown(KeyCode.Z))
        //    KeyinPutQueData.Enqueue(KeyCode.Z);//shitdown

        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //    KeyinPutQueData.Enqueue(KeyCode.LeftControl);//Space

        //MouseScroll
        float scroll = Input.GetAxis("Mouse ScrollWheel") * Speed;
        if (scroll != 0.0f) MouseScrollQueData.Enqueue(scroll);

        //MouseMove = Camara Transform
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
        {
            MouseMoveQueData.Enqueue(new Vector2(mouseX, mouseY));
        }
    }

}
