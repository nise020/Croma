using System;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public bool CameraShake = false; // ī�޶� ��鸮�� �������� ���θ� ��Ÿ���� ����

    Transform ShakeTr; // ī�޶� ��鸲�� ������ Transform
    private Coroutine runningShake;
    public class cShakeInfo  // ī�޶� ��鸲 ������ �����ϴ� Ŭ����
    {
        public float StartDelay; // ��鸲 ���� �� ���� �ð�
        public bool UseTotalTime; // ��ü ��鸲 �ð��� ������� ����
        public float TotalTime; // ��ü ��鸲 ���� �ð�
        public Vector3 Dest; // ��鸲�� ��ǥ ��ġ
        public Vector3 Shake; // ��鸲 ũ�� ����
        public Vector3 Dir; // ��鸲 ����
        //public Vector3 ShakeOffset;

        public float RemainDist; // ���� �̵� �Ÿ�
        public float RemainCountDis; // ���� Ƚ�� �� �̵� �Ÿ�

        public bool UseCount; // ��鸲�� Ư�� Ƚ���� �������� ����
        public int Count; // ��鸲 Ƚ��

        public float Veclocity; // ��鸲 �ӵ�

        public bool UseDamping; // ������ ������� ����
        public float Damping; // ���� ���
        public float DampingTime; // ���� �ð�
    }

    public cShakeInfo ShakeInfo = new cShakeInfo(); // ī�޶� ��鸲 ���� �ν��Ͻ�

    Vector3 Orgpos; // ī�޶��� ���� ��ġ

    float FovX = 0.2f; // �¿� �̵� ���� ���� ��
    float FovY = 0.2f; // ���� �̵� ���� ���� ��

    float Left = -1.0f; // ���� ��谪
    float Right = 1.0f; // ������ ��谪

    float Bottom = -1.0f; // �Ʒ� ���
    float Top = 1.0f;     // �� ���

    //ShakeMode shakeMode = ShakeMode.StaticCamera;
    //public void ShakeMOdeChange(ShakeMode _mode)
    //{
    //    shakeMode = _mode;
    //}
    private void Awake()
    {
        //Shared.ShakeCamera = this; // ShakeCamera �ν��Ͻ��� ���� ������ ����
        Orgpos = transform.localPosition; // �ʱ� ī�޶� ��ġ ����
        InitShake(); // �ʱ�ȭ �Լ� ȣ��
    }

    private void InitShake()
    {
        //ShakeTr = transform.parent; // ��鸲�� ������ Transform ����
        ShakeTr = transform; // ��鸲�� ������ Transform ����
        CameraShake = false; // �ʱ� ���¸� ��鸲 �������� ����
    }


    private void ResetShakeTr()
    {
        //transform.localPosition = Orgpos;

        //ShakeTr.localRotation = Quaternion.identity;
        ShakeTr.localPosition = Vector3.zero; // ī�޶� ��ġ�� �ʱ�ȭ<- ���� ����
        CameraShake = false; // ��鸲 ���� ����
        CameraLimit(); // ī�޶� ��ġ ���� ����<- ���� ����
    }

    void CameraLimit(bool _OrgPosY = false)
    {

        Vector3 camera = Orgpos; // �ʱ� ī�޶� ��ġ ���

        if (camera.x - FovX < Left) // ���� ���� �˻�
            camera.x = Left + FovX;
        else if (camera.x + FovX > Right) // ������ ���� �˻�
            camera.x = Right - FovX;

        if (camera.y - FovY < Bottom)
            camera.y = Bottom + FovY;
        else if (camera.y + FovY > Top)
            camera.y = Top - FovY;

        if (_OrgPosY) // Y�� �ʱ� ��ġ ���� ����
            camera.y = Orgpos.y;
    }

    public void Shake(int _count,Action onDone = null)
    {
        ShakeInfo.StartDelay = 0f; // ��鸲 ���� �ð� �ʱ�ȭ
        ShakeInfo.TotalTime = 1f; // ��鸲 ���� �ð� ����
        ShakeInfo.UseTotalTime = true; // ��ü �ð� ��� ����

        ShakeInfo.Shake = new Vector3(0.2f, 0.2f, 0f); // ��鸲 ũ�� ����
        ShakeInfo.Dir = ShakeInfo.Shake.normalized; // �ʱ� ���� ����
        ShakeInfo.Dest = ShakeInfo.Shake; // �ʱ� ��ǥ ��ġ ����
        ShakeInfo.Dir.Normalize(); // ���� ���� ����ȭ

        ShakeInfo.RemainDist = ShakeInfo.Shake.magnitude; // ���� �̵� �Ÿ� ���
        ShakeInfo.RemainCountDis = float.MaxValue; // ���� �Ÿ� �ʱ�ȭ

        ShakeInfo.Veclocity = 8; // ��鸲 �ӵ� ����

        ShakeInfo.Damping = 0.5f; // ���� ��� ����
        ShakeInfo.UseDamping = true; // ���� ��� ����
        ShakeInfo.DampingTime = ShakeInfo.RemainDist / ShakeInfo.Veclocity; // ���� �ð� ���

        ShakeInfo.Count = _count; // ��鸲 Ƚ�� ����
        ShakeInfo.UseCount = true; // Ƚ�� ��� ����

        //StopCoroutine("ShakeCoroutin");
        //ResetShakeTr(); 
        //yield return StartCoroutine("ShakeCoroutin");
        //onDone?.Invoke();

        if (runningShake != null)
            StopCoroutine(runningShake);
        ResetShakeTr(); 
        runningShake = StartCoroutine(ShakeCoroutin(onDone));

    }

    public IEnumerator ShakeCoroutin(Action onDone)
    {
        CameraShake = true; // ��鸲 ���� Ȱ��ȭ

        float dt, dist;

        if (ShakeInfo.StartDelay > 0)
            yield return new WaitForSeconds(ShakeInfo.StartDelay); // ���� �ð� ���

        while (true)
        {
            dt = Time.fixedDeltaTime; // ���� ��Ÿ Ÿ�� ���
            dist = dt * ShakeInfo.Veclocity; // ���� ������ �̵� �Ÿ� ���

            if ((ShakeInfo.RemainDist -= dist) > 0) // ���� �Ÿ� ����
            {
                //ShakeTr.localPosition += ShakeInfo.Dir * dist; // �̵� ����

                ShakeTr.localPosition = Vector3.MoveTowards(
                ShakeTr.localPosition,
                ShakeInfo.Dest,
                dist);

                float rc = transform.localPosition.x - FovX - Left; // ��� �˻�

                if (rc < 0f)
                { ShakeTr.localPosition += new Vector3(-rc, 0, 0); } // ���� ���� ����

                rc = Right - (transform.localPosition.x + FovX);

                if (rc < 0)
                    ShakeTr.localPosition += new Vector3(rc, 0, 0); // ������ ���� ����

                CameraLimit(true); // ī�޶� ��ġ ���� ����

                if (ShakeInfo.UseCount)
                {
                    if ((ShakeInfo.RemainCountDis -= dist) < 0)
                    {
                        ShakeInfo.RemainCountDis = float.MaxValue; // Ƚ�� �Ÿ� �ʱ�ȭ

                        if (--ShakeInfo.Count < 0) // ��鸲 Ƚ�� ����
                        {
                            break; // �ݺ� ����
                        }
                    }
                }
            }
            else
            {
                if (ShakeInfo.UseDamping) // ���� ��� ����
                {
                    float disdamping = Mathf.Max(
                        ShakeInfo.Damping * ShakeInfo.DampingTime,
                        ShakeInfo.Damping * dt);

                    if (ShakeInfo.Shake.magnitude > disdamping)
                    { ShakeInfo.Shake -= ShakeInfo.Dir * disdamping; }
                    else
                    {
                        ShakeInfo.UseCount = true;
                        ShakeInfo.Count = 1; // ���� Ƚ�� ����
                    }
                }
                ShakeTr.localPosition = ShakeInfo.Dest - ShakeInfo.Dir * (-ShakeInfo.RemainDist); ; // ���� ��ġ ����

                float rc = transform.localPosition.x - FovX - Left;

                if (rc < 0)
                    ShakeTr.localPosition += new Vector3(-rc, 0, 0); // ���� ���� ����

                rc = Right - (transform.localPosition.x + FovX);

                if (rc < 0)
                    ShakeTr.localPosition += new Vector3(rc, 0, 0); // ������ ���� ����

                CameraLimit(true); // ī�޶� ��ġ ����

                ShakeTr.localPosition = ShakeInfo.Dest - ShakeInfo.Dir * (-ShakeInfo.RemainDist); ; // ���� ��ġ ����; // �̵� ����

                ShakeInfo.Shake = -ShakeInfo.Shake; // ���� ����
                ShakeInfo.Dest = ShakeInfo.Shake; // ���ο� ��ǥ ��ġ
                ShakeInfo.Dir = -ShakeInfo.Dir; // ���� ���� ����

                float len = ShakeInfo.Shake.magnitude;

                ShakeInfo.RemainCountDis = len + ShakeInfo.RemainDist; // ���� �Ÿ� ����
                ShakeInfo.RemainDist += len * 2f;

                ShakeInfo.DampingTime = ShakeInfo.RemainDist / ShakeInfo.Veclocity; // ���� �ð� �缳��

                if (ShakeInfo.RemainDist < dist) break; // �ݺ� ���� ����
            }

            if (ShakeInfo.UseTotalTime && (ShakeInfo.TotalTime -= dt) < 0)
                break; // ��ü �ð� �ʰ� �� ����
            yield return new WaitForFixedUpdate(); // ���� �����ӱ��� ���
        }
        ResetShakeTr(); // ��鸲 ���� �� �ʱ�ȭ
        onDone?.Invoke();
        runningShake = null;
        yield break;
    }
}
