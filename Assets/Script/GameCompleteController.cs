using UnityEngine;

public class GameCompleteController : MonoBehaviour
{
    [Header("UI")]
    public GameObject completedUI;

    [Header("Referencias")]
    public StartMenuController startMenu;
    public WeedEffectController weedEffect;
    
    [Header("Spawn")]
    public Transform playerSpawn;

    private bool isCompleted = false;

    void Start()
    {
        completedUI.SetActive(false);
    }

    public void TriggerComplete()
    {
        if (isCompleted) return;

        isCompleted = true;

        // detener jugador
        if (startMenu != null)
        {
            startMenu.playerController.enabled = false;
            startMenu.crosshair.SetActive(false);
            startMenu.heartUI.SetActive(false);

            // animación de reloj
            startMenu.watchAnim.ToPause();
            startMenu.camLook.LookAtWatch();
        }

        // detener efectos
        if (weedEffect != null)
            weedEffect.ForceStopEffect();

        // mostrar UI
        completedUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMenu()
    {
        if (startMenu != null)
        {
            //REINICIAR POSICIÓN DEL PLAYER
            if (playerSpawn != null)
            {
                var player = startMenu.playerController.transform;

                startMenu.playerController.enabled = false;

                player.position = playerSpawn.position;
                player.rotation = playerSpawn.rotation;

                startMenu.playerController.enabled = true;
            }

            // reset visual/UI
            completedUI.SetActive(false);
            startMenu.ActivateStartMenu();
        }

        isCompleted = false;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}
