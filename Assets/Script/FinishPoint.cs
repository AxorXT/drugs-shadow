using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public GameCompleteController completeController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            completeController.TriggerComplete();
        }
    }
}
