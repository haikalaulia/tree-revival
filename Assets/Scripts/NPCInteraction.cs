using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogCanvas; // Tarik Canvas pesan ke sini

    void Start()
    {
        // Pastikan di awal pesan tersembunyi
        if(dialogCanvas != null) dialogCanvas.SetActive(false);
    }

    // Fungsi saat pemain masuk area (Circle Collider)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Pastikan Player kamu punya Tag "Player"
        {
            dialogCanvas.SetActive(true); // Munculkan pesan
        }
    }

    // Fungsi saat pemain menjauh
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogCanvas.SetActive(false); // Sembunyikan pesan
        }
    }
}