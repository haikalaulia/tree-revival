using UnityEngine;

public class NPCTourGuide : MonoBehaviour
{
    public UIManagerToko manager;
    public GameObject dialogCanvas;
    public int pendapatanWisata = 500; // Pendapatan tetap per hari dari turis

    private int hariTerakhirKerja = 0;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        if(dialogCanvas != null) dialogCanvas.SetActive(false);
        hariTerakhirKerja = manager.hariKe;
    }

    void Update()
    {
        // Berikan uang setiap hari berganti
        if (manager.hariKe > hariTerakhirKerja)
        {
            BerikanUangWisata();
            hariTerakhirKerja = manager.hariKe;
        }
    }

    void BerikanUangWisata()
    {
        // Wisatawan hanya datang kalau status minimal AMAN
        string status = manager.AmbilStatusWilayah();
        if (status == "AMAN" || status == "OPTIMAL")
        {
            manager.uangPemain += pendapatanWisata;
            Debug.Log("Pemandu: Banyak turis datang hari ini! Pendapatan: Rp " + pendapatanWisata);
        }
        else
        {
            Debug.Log("Pemandu: Hutan sedang rusak/tidak aman, tidak ada turis yang datang.");
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