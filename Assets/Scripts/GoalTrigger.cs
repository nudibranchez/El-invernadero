using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InterfaceHandler ui = FindFirstObjectByType<InterfaceHandler>();
            if (ui != null)
            {
                ui.ShowWinScreen();
            }
        }
    }
}