using DG.Tweening;
using UnityEngine;

public class WatchAnimator : MonoBehaviour
{
    public Transform watch;

    [Header("Posiciones")]
    public Vector3 startPos;
    public Vector3 playPos;
    public Vector3 pausePos;

    public Vector3 startRot;
    public Vector3 playRot;
    public Vector3 pauseRot;

    public float duration = 0.5f;

    private Tween moveTween;
    private Tween rotTween;

    public void ToStart()
    {
        AnimateTo(startPos, startRot);
    }

    public void ToPlay()
    {
        AnimateTo(playPos, playRot);
    }

    public void ToPause()
    {
        AnimateTo(pausePos, pauseRot);
    }

    void AnimateTo(Vector3 pos, Vector3 rot)
    {
        // cancelar animaciones anteriores
        if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
        if (rotTween != null && rotTween.IsActive()) rotTween.Kill();

        moveTween = watch.DOLocalMove(pos, duration).SetEase(Ease.OutCubic);
        rotTween = watch.DOLocalRotate(rot, duration).SetEase(Ease.OutCubic);
    }
}