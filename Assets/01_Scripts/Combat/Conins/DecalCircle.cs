using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DecalCircle : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _iconSprite;


    public bool showDecal = false;
    private Tween _blinkTween = null;

    public void OpenCircle(Vector3 point, float radius)
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0);
        transform.position = point;
        transform.localPosition = Vector3.one;

        showDecal = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(_spriteRenderer.DOFade(1, 0.3f));
        seq.Append(transform.DOScale(Vector3.one * (radius * 2), 0.8f));
        _iconSprite.transform.position = point;
        _iconSprite.enabled = true;

        _blinkTween = _iconSprite.DOFade(0f, 0.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    public void CloseCircle()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, 0.8f));
        seq.Join(_spriteRenderer.DOFade(0, 1.0f));
        showDecal = false;
        _iconSprite.enabled = false;
    }

    public void StopBlinkIcon()
    {
        if (_blinkTween != null && _blinkTween.IsActive())
        {
            _blinkTween.Kill();
            _iconSprite.DOFade(1f, 0.1f);
        }
    }
}
