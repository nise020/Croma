using UnityEngine;

[CreateAssetMenu(menuName = "EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType;         // 이 이팩트가 어떤 종류인지
    public ParticleSystem prefab;         // 실제 사용할 이팩트 프리팹
}
public enum EffectType
{
    Hit,        // 피격 이팩트
    Explosion,  // 폭발 이팩트
    Heal        // 힐 이팩트
}