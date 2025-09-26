using System;
using UnityEngine;
using static Enums;
public class ColorSteal
{
    public bool IsStealing { get; private set; } = false;

    private float startTime;
    private float stealTimer = 0f;
    private float stealCooldown = 0f;
    private float stealDurationMax = 5f;

    public Action OnStealStart;
    public Action OnStealEnd;
    public Action<float> OnStealCooldownSet;

    private const float minCooldown = 1f;
    private const float maxCooldown = 3f;

    public bool CanStartSteal(float gauge)
    {
        return !IsStealing && stealCooldown <= 0f && gauge >= 1f;
    }

    public void StartSteal()
    {
        IsStealing = true;
        stealTimer = stealDurationMax;
        startTime = Time.unscaledTime;

        Time.timeScale = 0.3f; // Slow Motion
        Time.fixedDeltaTime = 0.03f * Time.timeScale;
        OnStealStart?.Invoke();
    }

    public void TickSteal(float deltaTime)
    {
        if (!IsStealing) return;

        stealTimer -= deltaTime;
        if (stealTimer <= 0f)
        {
            EndSteal();
        }
    }

    public void TryClickMonster(Camera cam, Func<COLOR_TYPE, bool> gainColorFunc)
    {
        if (!IsStealing)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            /*var monster = hit.collider.GetComponent<ColorTarget>();
            if (monster != null)
            {
                COLOR_TYPE color = monster.GetColor();
                gainColorFunc?.Invoke(color);
                EndSteal();
                Debug.Log($"Steal Success: {color}");
                return;
            }*/
        }

        EndSteal();
        Debug.Log("Steal Failed");
    }

    public void EndSteal()
    {
        if (!IsStealing) return;

        IsStealing = false;
        Time.timeScale = 1f;
        stealTimer = 0f;

        float elapsedTime = Time.unscaledTime - startTime;
        startTime = -1f;

        if (elapsedTime > 0f)
        {
            float cooldown = Mathf.Clamp(elapsedTime, 2f, 5f);
            stealCooldown = cooldown;
            OnStealCooldownSet?.Invoke(cooldown);
            Debug.Log($"[ColorSteal] Cooldown Set: {cooldown:F2}s");
        }

        OnStealEnd?.Invoke();   
    }

    // After add TakeDamage 
    public void CancleByAttack()
    {
        if (!IsStealing)
            return;

        EndSteal();
        stealCooldown = 5f;
        OnStealCooldownSet.Invoke(stealCooldown);
        Debug.Log("Steal Cancle by Attack");
    }

    private float GetCooldown(float elapsedTime)
    {
        float cd = Mathf.Clamp(elapsedTime, 2f, 5f);
        Debug.Log($"[Cooldown] Calculated from elapsed {elapsedTime:F2} �� {cd:F2}");
        return cd;
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, stealCooldown);
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (stealCooldown > 0f)
        {
            stealCooldown -= deltaTime;
            if (stealCooldown < 0f) stealCooldown = 0f;
        }
    }
}
