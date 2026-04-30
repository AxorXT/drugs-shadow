using UnityEngine;
using UnityEngine.Windows;

public class PauseDetector : MonoBehaviour
{
    public StartMenuController menuController;
    public Transform cameraTransform;

    [Header("Look Down Settings")]
    public float lookThreshold = -0.8f; // qué tan abajo
    public float requiredTime = 2f;     // tiempo necesario

    private float lookTimer = 0f;

    public InputManager input;

    void Update()
    {
        if (!menuController.IsGameStarted)
            return;

        if (menuController.pauseMenuUI.activeSelf)
            return;

        bool isPressed = input.pausePressed;
        //no hacer nada si ya está pausado o no está jugando
        if (!menuController || !menuController.enabled) return;

        //OPCIÓN 1: ESC
        if (input.pausePressed)
        {
            menuController.PauseGame();
            return;
        }

        //OPCIÓN 2: mirar hacia abajo
        float dot = Vector3.Dot(cameraTransform.forward, Vector3.down);

        if (dot > -lookThreshold) // mirando hacia abajo
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= requiredTime)
            {
                menuController.PauseGame();
                lookTimer = 0f;
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }
}