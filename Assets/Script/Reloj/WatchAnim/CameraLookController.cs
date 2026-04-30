using DG.Tweening;
using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    public Transform cameraTransform;

    public float lookAtWatchX = 60f;
    public float normalX = 0f;
    public float duration = 0.5f;

    private float currentX;
    private Tween currentTween;

    void Start()
    {
        // Inicializa correctamente desde la rotación actual
        currentX = cameraTransform.localEulerAngles.x;

        // Evita valores tipo 300° en vez de -60°
        if (currentX > 180f) currentX -= 360f;
    }

    public void LookAtWatch()
    {
        AnimateTo(lookAtWatchX);
    }

    public void LookForward()
    {
        AnimateTo(normalX);
    }

    void AnimateTo(float target)
    {
        //Leer SIEMPRE la rotación actual real
        currentX = cameraTransform.localEulerAngles.x;

        if (currentX > 180f)
            currentX -= 360f;

        // matar tween anterior
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        currentTween = DOTween.To(() => currentX, x =>
        {
            currentX = x;
            cameraTransform.localRotation = Quaternion.Euler(currentX, 0, 0);

        }, target, duration)
        .SetEase(Ease.InOutSine);
    }
}
