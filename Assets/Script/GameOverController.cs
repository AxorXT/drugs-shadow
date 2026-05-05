using System.Collections;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject gameOverUI;
    public StartMenuController startMenu;
    public WeedEffectController weedEffect;
    public HeartRateSystem heartSystem;
    
    [Header("Spawn")]
    public Transform playerSpawn;

    private bool isGameOver = false;
    Coroutine freezeRoutine;

    void Start()
    {
        gameOverUI.SetActive(false);

        // Suscribirse al evento de muerte
        if (heartSystem != null)
            heartSystem.onDeath += TriggerGameOver;
    }

    void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // detener efecto weed
        if (weedEffect != null)
            weedEffect.ForceStopEffect();

        // detener jugador (SIN usar PauseGame)
        if (startMenu != null)
        {
            startMenu.playerController.enabled = false;
            startMenu.crosshair.SetActive(false);
            startMenu.heartUI.SetActive(false);

            //AQUÍ metes la animación de cámara/reloj
            startMenu.watchAnim.ToPause();
            startMenu.camLook.LookAtWatch();
        }

        // mostrar UI de game over
        gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //REINICIAR (vuelve al estado inicial del juego)
    public void RestartGame()
    {
        Time.timeScale = 1f;

        ResetSystems();

        //REINICIAR POSICIÓN DEL PLAYER
        if (startMenu != null && playerSpawn != null)
        {
            var player = startMenu.playerController.transform;

            startMenu.playerController.enabled = false;

            player.position = playerSpawn.position;
            player.rotation = playerSpawn.rotation;

            startMenu.playerController.enabled = true;
        }

        gameOverUI.SetActive(false);
        isGameOver = false;

        startMenu.ActivateStartMenu();
    }

    // MENÚ (es lo mismo porque tu menú está en la misma escena)
    public void GoToMenu()
    {
        RestartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }

    void ResetSystems()
    {
        if (heartSystem != null)
        {
            heartSystem.ResetState();
            heartSystem.recoverySpeed = 5f;
        }

        if (weedEffect != null)
            weedEffect.FullReset();
    }

    void OnDestroy()
    {
        if (heartSystem != null)
            heartSystem.onDeath -= TriggerGameOver;
    }
}
