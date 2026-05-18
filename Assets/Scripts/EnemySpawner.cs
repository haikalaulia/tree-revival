using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefabPembalak;
    public Transform titikMuncul;
    public UIManagerToko manager;

    [Header("Pengaturan Jeda Muncul (Detik)")]
    public float jedaKritis = 15f;  // Muncul sangat cepat
    public float jedaWaspada = 40f; // Muncul agak lambat
    public float jedaAman = 120f;   // Muncul sangat jarang
    // Status Optimal = Tidak muncul sama sekali

    private float timer;

    void Update()
    {
        // Ambil status wilayah dari Global Manager
        string status = manager.AmbilStatusWilayah();

        // Tentukan jeda berdasarkan status
        float jedaAktif = 0;

        if (status == "KRITIS") jedaAktif = jedaKritis;
        else if (status == "WASPADA") jedaAktif = jedaWaspada;
        else if (status == "AMAN") jedaAktif = jedaAman;
        else if (status == "OPTIMAL") return; // Jika optimal, stop kodenya di sini (tidak spawn)

        // Logika Timer Muncul
        timer += Time.deltaTime;
        if (timer >= jedaAktif)
        {
            SpawnLawan();
            timer = 0;
        }
    }

    void SpawnLawan()
    {
        // Hanya spawn jika belum ada penjaga (opsional, tergantung keinginanmu)
        // Jika ingin penjaga hanya 'mengusir' setelah muncul, biarkan kode ini:
        Instantiate(prefabPembalak, titikMuncul.position, Quaternion.identity);
        Debug.Log("Pembalak Liar Muncul! Status Wilayah: " + manager.AmbilStatusWilayah());
    }
}