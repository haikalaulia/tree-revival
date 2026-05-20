using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefabPembalak;
    public Transform[] daftarTitikMuncul;
    public UIManagerToko manager;

    [Header("Batas Maksimal Lawan di Map")]
    public int maxLawanDiMap = 3; // Agar tidak lag, batasi maksimal hanya ada 3 pembalak sekaligus

    [Header("Pengaturan Jeda Muncul (Detik)")]
    public float jedaKritis = 15f;  
    public float jedaWaspada = 40f; 
    public float jedaAman = 120f;   

    private float timer;

    void Update()
    {
        if (manager == null) return;

        // --- TAMBAHAN PENGAMAN 1: HITUNG JUMLAH LAWAN SAAT INI ---
        int jumlahLawanSekarang = FindObjectsByType<NPCWoodcutter>(FindObjectsSortMode.None).Length;
        if (jumlahLawanSekarang >= maxLawanDiMap) return; // Jika sudah penuh, jangan spawn lagi
        // --------------------------------------------------------

        string status = manager.AmbilStatusWilayah();
        float jedaAktif = 0;

        if (status == "KRITIS") jedaAktif = jedaKritis;
        else if (status == "WASPADA") jedaAktif = jedaWaspada;
        else if (status == "AMAN") jedaAktif = jedaAman;
        else if (status == "OPTIMAL") return; 

        // --- TAMBAHAN PENGAMAN 2: LOGIKA TIMER YANG BENAR ---
        timer += Time.deltaTime;
        if (timer >= jedaAktif)
        {
            SpawnLawanDiTitikAcak();
            timer = 0; // Pastikan timer di-reset ke nol setelah spawn!
        }
    }

    void SpawnLawanDiTitikAcak()
    {
        if (daftarTitikMuncul.Length == 0 || prefabPembalak == null) return;

        int indeksAcak = Random.Range(0, daftarTitikMuncul.Length);
        Instantiate(prefabPembalak, daftarTitikMuncul[indeksAcak].position, Quaternion.identity);
    }
}