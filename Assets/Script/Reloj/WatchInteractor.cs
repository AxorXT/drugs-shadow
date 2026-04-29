using UnityEngine;

public class WatchInteractor : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;

    public LayerMask watchLayer;
    public InputManager input;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            //puedes usar tag o layer
            if (hit.collider.CompareTag("Watch"))
            {
                if (input.weedPressed) // tu input de click
                {
                    OpenPauseMenu();
                }
            }
        }
    }

    void OpenPauseMenu()
    {
        Debug.Log("Abrir menú de pausa");

        // luego aquí llamas tu GameManager
        // GameManager.Instance.SetState(GameState.Paused);
    }
}
