using UnityEngine;

[CreateAssetMenu(menuName = "EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType;         // �� ����Ʈ�� � ��������
    public ParticleSystem prefab;         // ���� ����� ����Ʈ ������
}
public enum EffectType
{
    Hit,        // �ǰ� ����Ʈ
    Explosion,  // ���� ����Ʈ
    Heal        // �� ����Ʈ
}