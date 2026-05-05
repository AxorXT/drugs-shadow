using DG.Tweening;
using EasyPeasyFirstPersonController;
using System.Collections;
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
    public GameObject pauseMenuUI;
    public GameObject creditsUI;

    [Header("Rotación")]
    public float lookAtWatchX = 60f;

    private bool gameStarted = false;

    public WatchAnimator watchAnim;
    public CameraLookController camLook;

    public bool IsGameStarted => gameStarted;

    public WeedEffectController weedEffect;

    void Awake()
    {
        //cámara inicia viendo el reloj (SIN animación)
        playerCamera.localRotation = Quaternion.Euler(lookAtWatchX, 0f, 0f);
    }

    void Start()
    {
        ActivateStartMenu();
    }

    public void ActivateStartMenu()
    {
        CancelInvoke();
        gameStarted = false;

        playerController.enabled = false;

        watchAnim.ToStart();
        camLook.LookAtWatch();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startMenuUI.SetActive(true);
        heartUI.SetActive(false);
        crosshair.SetActive(false);
        pauseMenuUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void StartGame()
    {
        CancelInvoke();
        if (gameStarted) return;

        gameStarted = true;

        startMenuUI.SetActive(false);
        heartUI.SetActive(true);
        crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.enabled = false;

        watchAnim.ToPlay();
        camLook.LookForward();

        StartCoroutine(StartGameRoutine());
    }

    void EnablePlayer()
    {
        playerController.enabled = true;
    }

    public void PauseGame()
    {
        playerController.enabled = false;

        if (weedEffect != null)
            weedEffect.ForceStopEffect();

        watchAnim.ToPause();
        camLook.LookAtWatch();

        pauseMenuUI.SetActive(true);
        heartUI.SetActive(false);
        crosshair.SetActive(false);
        startMenuUI.SetActive(false );

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        heartUI.SetActive(true);
        crosshair.SetActive(true);

        watchAnim.ToPlay();
        camLook.LookForward();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(EnablePlayerAfterRotation());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }

    IEnumerator EnablePlayerAfterRotation()
    {
        yield return new WaitForSeconds(0.5f);

        //sincronizar rotación antes de activar
        float currentX = playerCamera.localEulerAngles.x;

        if (currentX > 180f)
            currentX -= 360f;

        playerController.SetLookRotation(currentX);

        playerController.enabled = true;
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    IEnumerator StartGameRoutine()
    {
        playerController.enabled = false;

        watchAnim.ToPlay();
        camLook.LookForward();

        yield return new WaitForSeconds(0.5f);

        //sincroniza la rotación ANTES de activar
        float currentX = playerCamera.localEulerAngles.x;

        if (currentX > 180f)
            currentX -= 360f;

        playerController.SetLookRotation(currentX);

        playerController.enabled = true;
    }

    public void OpenCredits()
    {
        CancelInvoke();
        StopAllCoroutines();

        // ocultar menú principal
        startMenuUI.SetActive(false);

        // mostrar créditos
        creditsUI.SetActive(true);

        // asegurar control de cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCredits()
    {
        creditsUI.SetActive(false);

        // volver al menú inicial correctamente
        ActivateStartMenu();
    }
}
