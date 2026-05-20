using UnityEngine;
using UnityEngine.UI;

public class ItemNPC : MonoBehaviour
{
    public string namaNPC;
    public int hargaRekrut;
    public GameObject prefabNPC; 
    
    [Header("Sistem Banyak Titik Muncul")]
    public Transform[] daftarLokasiMuncul; // Masukkan Kabin 1 dan Kabin 2 di sini
    private int indeksLokasiSekarang = 0;

    public Button tombolRekrut; 
    private UIManagerToko manager; 

    void Start() {
        manager = FindFirstObjectByType<UIManagerToko>();
    }

   public void KlikRekrut() {
        if (manager == null) return;

        // Tentukan apakah kuota masih ada berdasarkan nama NPC
        bool kuotaTersedia = false;
        if (namaNPC == "Peneliti" && manager.jmlPeneliti < manager.maxPeneliti) kuotaTersedia = true;
        else if (namaNPC == "Petani" && manager.jmlPetani < manager.maxPetani) kuotaTersedia = true;
        else if (namaNPC == "Penjaga" && manager.jmlPenjaga < manager.maxPenjaga) kuotaTersedia = true;
        else if (namaNPC == "Pemandu" && manager.jmlPemandu < manager.maxPemandu) kuotaTersedia = true;

        if (manager.uangPemain >= hargaRekrut && kuotaTersedia) {
            manager.uangPemain -= hargaRekrut;

            // --- LOGIKA PENENTUAN POSISI MUNCUL ---
            Vector3 posisiTujuan = transform.position; // Default posisi kartu sendiri jika lupa isi kotak

            if (daftarLokasiMuncul.Length > 0)
            {
                // Pilih lokasi berdasarkan urutan indeks
                posisiTujuan = daftarLokasiMuncul[indeksLokasiSekarang].position;
                
                // Geser angka indeks untuk pembeli berikutnya (0 -> 1 -> 0)
                indeksLokasiSekarang = (indeksLokasiSekarang + 1) % daftarLokasiMuncul.Length;
            }
            // --------------------------------------

            // Munculkan NPC di posisi kabin yang dipilih
            Instantiate(prefabNPC, posisiTujuan, Quaternion.identity);

            // Tambah angka ke manager sesuai jenis
            if(namaNPC == "Peneliti") { manager.jmlPeneliti++; manager.adaPeneliti = true; }
            else if(namaNPC == "Petani") manager.jmlPetani++;
            else if(namaNPC == "Penjaga") manager.jmlPenjaga++;
            else if(namaNPC == "Pemandu") manager.jmlPemandu++;

            manager.TutupToko(); 
        } else {
            Debug.Log("Gagal Rekrut: Uang tidak cukup atau Kuota Penuh!");
        }
    }

    void Update()
    {
        if (manager == null) return;

        if (namaNPC == "Penjaga")
        {
            float persen = (manager.totalLuasTajuk / manager.luasLahanTotal) * 100f;
            if(tombolRekrut != null)
                tombolRekrut.interactable = (persen >= 40 && manager.uangPemain >= hargaRekrut);
        }
        else if (namaNPC == "Pemandu")
        {
            string status = manager.AmbilStatusWilayah();
            if(tombolRekrut != null)
                tombolRekrut.interactable = (status == "AMAN" || status == "OPTIMAL") && manager.uangPemain >= hargaRekrut;
        }
    }
}