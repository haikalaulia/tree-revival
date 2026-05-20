using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefabPembalak;
    
    [Header("Banyak Titik Muncul")]
    public Transform[] daftarTitikMuncul; // Ganti dari satu titik menjadi banyak titik

    public UIManagerToko manager;

    [Header("Pengaturan Jeda Muncul (Detik)")]
    public float jedaKritis = 15f;  
    public float jedaWaspada = 40f; 
    public float jedaAman = 120f;   

    private float timer;

    void Update()
    {
        if (manager == null) return;

        string status = manager.AmbilStatusWilayah();
        float jedaAktif = 0;

        if (status == "KRITIS") jedaAktif = jedaKritis;
        else if (status == "WASPADA") jedaAktif = jedaWaspada;
        else if (status == "AMAN") jedaAktif = jedaAman;
        else if (status == "OPTIMAL") return; 

        timer += Time.deltaTime;
        if (timer >= jedaAktif)
        {
            SpawnLawanDiTitikAcak();
            timer = 0;
        }
    }

    void SpawnLawanDiTitikAcak()
    {
        if (daftarTitikMuncul.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: Daftar titik muncul kosong!");
            return;
        }

        // PILIH SATU TITIK SECARA ACAK DARI DAFTAR
        int indeksAcak = Random.Range(0, daftarTitikMuncul.Length);
        Transform titikPilihan = daftarTitikMuncul[indeksAcak];

        // MUNCULKAN PEMBALAK
        Instantiate(prefabPembalak, titikPilihan.position, Quaternion.identity);
        
        Debug.Log("Pembalak Liar Muncul di Titik: " + titikPilihan.name);
    }
}