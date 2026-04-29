using DG.Tweening;
using EasyPeasyFirstPersonController;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public FirstPersonController playerController;
    public Transform playerCamera;

    [Header("UI")]
    public GameObject startMenuUI;
    public GameObject heartUI;
    public GameObject crosshair;

    [Header("Rotación")]
    public float lookAtWatchX = 60f;
    public float transitionTime = 0.5f;

    private bool gameStarted = false;

    void Start()
    {
        ActivateStartMenu();
        playerController.enabled = false;
        playerCamera.localRotation = Quaternion.Euler(lookAtWatchX, 0f, 0f);
    }

    void Awake()
    {
        //Coloca la cámara DIRECTAMENTE viendo el reloj (sin animación)
        playerCamera.localRotation = Quaternion.Euler(lookAtWatchX, 0f, 0f);
    }

    void ActivateStartMenu()
    {
        gameStarted = false;

        //Bloquear movimiento
        playerController.enabled = false;

        //Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Mostrar UI
        startMenuUI.SetActive(true);
        heartUI.SetActive(false);
        crosshair.SetActive(false);
    }

    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;

        //Ocultar UI
        startMenuUI.SetActive(false);
        heartUI.SetActive(true);
        crosshair.SetActive(true);

        //Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.enabled = false;
        float currentX = lookAtWatchX;

        DOTween.To(() => currentX, x =>
        {
            currentX = x;

            // aplicamos rotación manual
            playerCamera.localRotation = Quaternion.Euler(currentX, 0f, 0f);

        }, 0f, transitionTime)
    .SetEase(Ease.InOutSine)
    .OnComplete(() =>
    {
        //ahora sí activamos el controller
        playerController.enabled = true;
    });
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }

    void SetCameraRotation(float xRot)
    {
        // accedemos a la rotación vertical del controller
        var field = typeof(FirstPersonController)
            .GetField("xRotation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(playerController, xRot);
        }
    }
}
