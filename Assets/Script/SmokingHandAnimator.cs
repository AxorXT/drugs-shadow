using DG.Tweening;
using UnityEngine;

public class SmokingHandAnimator : MonoBehaviour
{
    public Transform hand;

    [Header("Posiciones")]
    public Vector3 idlePos;
    public Vector3 smokePos;

    public Vector3 idleRot;
    public Vector3 smokeRot;

    public float duration = 0.4f;

    private Tween moveTween;
    private Tween rotTween;

    void Start()
    {
        SetInstantIdle();
    }

    public Tween ToSmoke()
    {
        KillTweens();

        moveTween = hand.DOLocalMove(smokePos, duration).SetEase(Ease.OutCubic);
        rotTween = hand.DOLocalRotate(smokeRot, duration).SetEase(Ease.OutCubic);

        return moveTween;
    }

    public void ToIdle()
    {
        KillTweens();

        moveTween = hand.DOLocalMove(idlePos, duration).SetEase(Ease.OutCubic);
        rotTween = hand.DOLocalRotate(idleRot, duration).SetEase(Ease.OutCubic);
    }

    void KillTweens()
    {
        if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
        if (rotTween != null && rotTween.IsActive()) rotTween.Kill();
    }

    public void SetInstantIdle()
    {
        hand.localPosition = idlePos;
        hand.localRotation = Quaternion.Euler(idleRot);
    }
}
