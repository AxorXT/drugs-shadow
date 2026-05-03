using DG.Tweening;
using UnityEngine;

public class ShadowEntity : MonoBehaviour
{
    public float lifeTime = 3f;
    public float fadeDuration = 0.5f;
    public float moveDistance = 1.5f;

    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        SetAlpha(0f); // empieza invisible

        PlaySequence();
    }

    void PlaySequence()
    {
        Sequence seq = DOTween.Sequence();

        //FADE IN
        seq.Append(Fade(1f));

        //MOVIMIENTO LATERAL
        Vector3 left = transform.position + transform.right * -moveDistance;
        Vector3 right = transform.position + transform.right * moveDistance;

        seq.Append(transform.DOMove(right, lifeTime / 2f).SetEase(Ease.InOutSine));
        seq.Append(transform.DOMove(left, lifeTime / 2f).SetEase(Ease.InOutSine));

        //FADE OUT
        seq.Append(Fade(0f));

        //destruir
        seq.OnComplete(() => Destroy(gameObject));
    }

    Tween Fade(float targetAlpha)
    {
        Tween t = null;

        foreach (var rend in renderers)
        {
            t = rend.material.DOColor(
                new Color(0, 0, 0, targetAlpha),
                fadeDuration
            );
        }

        return t;
    }

    void SetAlpha(float a)
    {
        foreach (var rend in renderers)
        {
            Color c = rend.material.color;
            c.a = a;
            rend.material.color = c;
        }
    }
}
