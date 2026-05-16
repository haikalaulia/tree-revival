using UnityEngine;

public class NPCGuard : MonoBehaviour
{
    public UIManagerToko manager;
    public GameObject dialogCanvas;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        if(dialogCanvas != null) dialogCanvas.SetActive(false);

        // SAAT MUNCUL: Langsung menambah jumlah penjaga di sistem
        if(manager != null)
        {
            manager.jmlPenjagaHutan++;
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