using Cysharp.Threading.Tasks;
using UnityEngine;
using static Enums;


public class FollowCamera3D : MonoBehaviour
{
   // [SerializeField] private Transform target;
    [SerializeField, ReadOnly] private float smoothSpeed = 8f;
    [SerializeField, ReadOnly] private float yOffset = 2f;

    [SerializeField, ReadOnly] private Transform viewObj; // 플레이어 트랜스폼
     public float rotSensitive = 3.0f;
     private float yMinLimit = -25f;
     private float yMaxLimit = 50f;
     private float distance = 10f;
     private float distanceMin = 8f;
     private float distanceMax = 15f;

    private float xValue = 30f;
    private float yValue = 4f;
    [SerializeField] private float pivotHeight = 1.6f;
    [SerializeField] private float cameraRadius = 0.25f;// 카메라 충돌 반지름
    [SerializeField] private float clipBuffer = 0.05f;  // 벽과 카메라 사이 여유
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float minDistance = 0.5f;
    private LayerMask obstructionMask;
    private float currentDistance;

    bool isAttack = false;
    private Animation AttackAnimation;
    private Animator CameraAnimator;

    ShakeCamera shakeCamera;
    private void OnDestroy()
    {
        if (GameShard.Instance.InputManager != null) 
        {
            GameShard.Instance.InputManager.MouseMoveEventData -= UpdateRotationValues;
            GameShard.Instance.InputManager.MouseScrollEventData -= UpdateDistanse;
        }
    }

    public void init(Player player) 
    {
        Vector3 vector3 = new Vector3(xValue, yValue, 0);
        transform.position = new Vector3(3.0f, 3.0f, distance);
        transform.rotation = Quaternion.LookRotation(vector3);

        obstructionMask = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString(), LAYER_TYPE.Wall.ToString());

        Transform bodyTrans = player.transform.GetChild(0);
        viewObj = bodyTrans;
        GameShard.Instance.InputManager.MouseMoveEventData += UpdateRotationValues;
        GameShard.Instance.InputManager.MouseScrollEventData += UpdateDistanse;

        shakeCamera = gameObject.GetComponentInChildren<ShakeCamera>();
        Camera cam = gameObject.GetComponentInChildren<Camera>();
        cam = Camera.main;
        //AttackAnimation = Shared.Instance.ResourcesManager.CameraAnimation;

        //CameraAnimator = gameObject.AddComponent<Animator>();
        //CameraAnimator = gameObject.GetComponent<Animator>();

        //AnimatorController animator = Shared.Instance.ResourcesManager.CameraAnimator;
        //CameraAnimator.runtimeAnimatorController = animator;
    }
    public async UniTask CameraAttackMoveOn(bool _check ,int _count) 
    {
        if (_check) 
        {
            isAttack = true;
            shakeCamera.Shake(_count, () => 
            {
                isAttack = false;
            });
            return;
        }
        await UniTask.Yield();
    }

    private void LateUpdate()
    {
        if (viewObj == null) return;
        UpdateCameraTransform();
    }
    private void UpdateDistanse(float _value)
    {
        distance -= _value; 
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
    }

    private void UpdateRotationValues(Vector2 input)
    {
        if(isAttack) return;

        xValue += input.x * rotSensitive;
        yValue -= input.y * rotSensitive;
        yValue = Mathf.Clamp(yValue, yMinLimit, yMaxLimit);
    }

    private void UpdateCameraTransform()
    {
        //Quaternion rotation = Quaternion.Euler(yValue, xValue, 0);
        //Vector3 desiredOffset = rotation * new Vector3(0, 0, -distance); // 목표 오프셋
        //Vector3 desiredPos = viewObj.position + desiredOffset; // 카메라 목표 위치

        //// 벽 충돌 체크
        //if ((Physics.SphereCast(viewObj.position, 0.2f, desiredOffset.normalized,
        //               out RaycastHit hit, distance)))
        //{
        //    // 벽에 닿았으면 카메라를 충돌 지점 앞으로 이동
        //    transform.position = viewObj.position + desiredOffset.normalized * (hit.distance - 0.1f);
        //}
        //else
        //{
        //    // 충돌 없으면 원래 목표 위치
        //    transform.position = desiredPos;
        //}
        //transform.LookAt(viewObj);

        Quaternion rotation = Quaternion.Euler(yValue, xValue, 0f);
        Vector3 dir = rotation * Vector3.back;

        Vector3 pivot = viewObj.position + Vector3.up * pivotHeight;

        float targetDistance = distance;

        if (Physics.SphereCast(pivot, cameraRadius, dir, out RaycastHit hit,
                               distance, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Clamp(hit.distance - cameraRadius - clipBuffer, minDistance, distance);
        }

        currentDistance = targetDistance;

        Vector3 camPos = pivot + dir * currentDistance;

        if (Physics.CheckSphere(camPos, cameraRadius, obstructionMask, QueryTriggerInteraction.Ignore))
        {
            camPos = pivot + dir * minDistance;
        }

        transform.position = camPos;
        transform.LookAt(pivot);

    }
    //private void RotateCamera(Vector2 _input)
    //{

    //    xValue += _input.x * rotSensitive;
    //    yValue -= _input.y * rotSensitive;

    //    yValue = Mathf.Clamp(yValue, yMinLimit, yMaxLimit);

    //    Quaternion rotation = Quaternion.Euler(yValue, xValue, 0f);

    //    Vector3 negDistance = new Vector3(0f, 0f, -distance);
    //    Vector3 position = rotation * negDistance + viewObj.position;


    //    transform.position = position;
    //    transform.LookAt(viewObj);
    //}
}
