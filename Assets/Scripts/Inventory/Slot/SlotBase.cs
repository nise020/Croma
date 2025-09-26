using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System;
using System.Collections;

public enum SLOTTYPE
{
    Item,
    Skill,
    StatPlus,
    Quest,
    Menu,
    SkillAction,
    OptionSlider,   
}

public class SlotBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    public SLOTTYPE slotType;

    [SerializeField] public List<Image> selectImages;

    public ButtonUIManager buttonManager;

    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public Coroutine fadeCoroutine;

    private AudioSource buttonPlayer;

    private const int slotSoundID = 20015;


    [Header("ClickEffect")]
    [SerializeField] private bool usePressScale = false;
    [SerializeField] private Transform scaleTarget;
    [SerializeField] private float pressedScale = 0.8f;
    [SerializeField] private bool tweenOnRelease = true;
    [SerializeField] private float releaseDuration = 0.2f;

    private Vector3 _originScale;
    private Coroutine _releaseCo;

    protected virtual void Awake()
    {
        buttonPlayer = gameObject.AddComponent<AudioSource>();

        if (scaleTarget == null) scaleTarget = transform;
        _originScale = scaleTarget.localScale;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!usePressScale) return;
        RestoreScale();
    }

    public virtual void OnDrag(PointerEventData eventData) { }

    public virtual void OnEndDrag(PointerEventData eventData) { }
    public virtual void OnDrop(PointerEventData eventData) { }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ButtonPlay();
    }

    public virtual void OnPointerEnter(PointerEventData eventData) { }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!usePressScale) return;
        RestoreScale();
    }

    protected void OnDisable()
    {
        // 선택/하이라이트 초기화
        isSelected = false;
        if (buttonManager != null)
        {
            for (int i = 0; i < selectImages.Count; i++)
            {
                if (selectImages[i] == null) continue;
                var c = selectImages[i].color;
                c.a = 0f;
                selectImages[i].color = c;
            }
        }

        // 스케일/코루틴 안전 복구
        if (scaleTarget != null) scaleTarget.localScale = _originScale;
        if (_releaseCo != null) { StopCoroutine(_releaseCo); _releaseCo = null; }
    }

    protected void ButtonPlay()
    {
        if (buttonPlayer == null)
        {
            buttonPlayer = gameObject.AddComponent<AudioSource>();
        }
        var clip = Shared.Instance.SoundManager.ClipGet(slotSoundID);
        buttonPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        buttonPlayer.PlayOneShot(clip);
    }

    // ====== Press Scale Handlers ======
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!usePressScale) return;

        if (_releaseCo != null)
        {
            StopCoroutine(_releaseCo);
            _releaseCo = null;
        }

        // 누르는 순간 즉시 축소 (반응성 최우선)
        scaleTarget.localScale = _originScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!usePressScale) return;
        RestoreScale();
    }

    private void RestoreScale()
    {
        if (!tweenOnRelease || releaseDuration <= 0f)
        {
            if (_releaseCo != null) { StopCoroutine(_releaseCo); _releaseCo = null; }
            scaleTarget.localScale = _originScale;
            return;
        }

        if (_releaseCo != null) StopCoroutine(_releaseCo);
        _releaseCo = StartCoroutine(CoRelease(scaleTarget.localScale, _originScale, releaseDuration));
    }

    private IEnumerator CoRelease(Vector3 from, Vector3 to, float dur)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, dur);
            scaleTarget.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }
        scaleTarget.localScale = to;
        _releaseCo = null;
    }
}