using UnityEngine;

public class NPCResearcher : MonoBehaviour
{
    public GameObject dialogCanvas;
    public UIManagerToko globalManager; // Referensi ke GlobalManager temanmu
    public float multiplierCO2 = 1.5f; // Bonus serapan karbon 1.5x lipat

    void Start()
{
    if(dialogCanvas != null) dialogCanvas.SetActive(false);
    
    // Cari GlobalManager secara otomatis
    if(globalManager == null) globalManager = FindFirstObjectByType<UIManagerToko>();

    // Begitu peneliti muncul, dia langsung mengaktifkan bonus CO2 di Global Manager
    if(globalManager != null) {
        globalManager.adaPeneliti = true; 
        Debug.Log("Bonus Peneliti Aktif!");
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogCanvas.SetActive(true);
            // Saat didekati, dia memberi semangat
            Debug.Log("Peneliti: Saya sedang meneliti cara pohon menyerap lebih banyak CO2!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogCanvas.SetActive(false);
        }
    }

    // Fungsi ini bisa dipanggil saat peneliti "direkrut"
    public void AktifkanBuffPeneliti()
    {
        // Logika untuk mengubah efisiensi di GlobalManager
        Debug.Log("Efisiensi Karbon Meningkat!");
    }
}