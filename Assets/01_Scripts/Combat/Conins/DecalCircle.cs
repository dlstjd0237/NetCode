using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DecalCircle : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _spriteRenderer;


    public bool showDecal = false;

    public void OpenCircle(Vector3 point, float radius)
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0);
        transform.position = point;
        transform.localPosition = Vector3.one;

        showDecal = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(_spriteRenderer.DOFade(1, 0.3f));
        seq.Append(transform.DOScale(Vector3.one * (radius * 2), 0.8f));
    }

    public void CloseCircle()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, 0.8f));
        seq.Join(_spriteRenderer.DOFade(0, 1.0f));
        showDecal = false;
    }
}
