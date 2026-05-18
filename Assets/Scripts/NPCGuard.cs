using UnityEngine;

public class NPCGuard : MonoBehaviour
{
    public UIManagerToko manager;
    public GameObject dialogCanvas;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        if(dialogCanvas != null) dialogCanvas.SetActive(false);

        if(manager != null)
        {
            Debug.Log("Penjaga Hutan: Saya siap berpatroli!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) dialogCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) dialogCanvas.SetActive(false);
    }
}