using UnityEngine;

public abstract partial class UiBase : MonoBehaviour
{
    protected int UiId;
    protected int SoundId;
    protected abstract void UiStateOn();
    protected abstract void UiStateOff();
    protected abstract void EffectSoundOneShot();
}
