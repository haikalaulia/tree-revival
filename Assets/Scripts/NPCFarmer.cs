using UnityEngine;

public class NPCFarmer : MonoBehaviour
{
    public UIManagerToko manager;
    public int uangPerPohonBerbuah = 100; // Satu pohon berbuah menghasilkan Rp 100
    public GameObject dialogCanvas;

    private int hariTerakhirDibayar = 0;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        if(dialogCanvas != null) dialogCanvas.SetActive(false);
        
        // Daftarkan hari saat dia mulai bekerja
        hariTerakhirDibayar = manager.hariKe;
    }

    void Update()
    {
        // LOGIKA: Setiap kali hari berganti, berikan uang hasil panen
        if (manager.hariKe > hariTerakhirDibayar)
        {
            KasihHasilPanen();
            hariTerakhirDibayar = manager.hariKe;
        }
    }

    void KasihHasilPanen()
    {
        // Hitung: Jumlah pohon berbuah x Rp 100
        int totalHasil = manager.jmlBerbuah * uangPerPohonBerbuah;
        
        if (totalHasil > 0)
        {
            manager.uangPemain += totalHasil;
            Debug.Log("Petani: Berhasil panen! Kamu dapat Rp " + totalHasil);
            // Nanti bisa tambah teks melayang "+ Rp XXX" di atas kepala petani
        }
    }

    // Interaksi dialog standar
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) dialogCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) dialogCanvas.SetActive(false);
    }
}