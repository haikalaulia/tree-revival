using UnityEngine;
using UnityEngine.UI;

public class ItemNPC : MonoBehaviour
{
    public string namaNPC;
    public int hargaRekrut;
    public GameObject prefabNPC; 
    
    [Header("Sistem Banyak Titik Muncul")]
    public Transform[] daftarLokasiMuncul; 
    private int indeksLokasiSekarang = 0;

    // Variabel targetHutanLevel di sini DIHAPUS karena sudah pindah ke UIManagerToko

    public Button tombolRekrut; 
    private UIManagerToko manager; 

    void Start() {
        manager = FindFirstObjectByType<UIManagerToko>();
    }

   public void KlikRekrut() {
        if (manager == null) return;

        bool kuotaTersedia = false;
        if (namaNPC == "Peneliti" && manager.jmlPeneliti < manager.maxPeneliti) kuotaTersedia = true;
        else if (namaNPC == "Petani" && manager.jmlPetani < manager.maxPetani) kuotaTersedia = true;
        else if (namaNPC == "Penjaga" && manager.jmlPenjaga < manager.maxPenjaga) kuotaTersedia = true;
        else if (namaNPC == "Pemandu" && manager.jmlPemandu < manager.maxPemandu) kuotaTersedia = true;

        if (manager.uangPemain >= hargaRekrut && kuotaTersedia) {
            manager.uangPemain -= hargaRekrut;

            Vector3 posisiTujuan = transform.position; 
            if (daftarLokasiMuncul.Length > 0)
            {
                posisiTujuan = daftarLokasiMuncul[indeksLokasiSekarang].position;
                indeksLokasiSekarang = (indeksLokasiSekarang + 1) % daftarLokasiMuncul.Length;
            }

            Instantiate(prefabNPC, posisiTujuan, Quaternion.identity);

            if(namaNPC == "Peneliti") { manager.jmlPeneliti++; manager.adaPeneliti = true; }
            else if(namaNPC == "Petani") manager.jmlPetani++;
            else if(namaNPC == "Penjaga") manager.jmlPenjaga++;
            else if(namaNPC == "Pemandu") manager.jmlPemandu++;

            manager.TutupToko(); 
        }
    }

    void Update()
    {
        if (manager == null) return;

        if (namaNPC == "Penjaga")
        {
            // --- CARI DASHBOARD UNTUK MENGINTIP TARGETNYA ---
            DashboardUI dash = FindFirstObjectByType<DashboardUI>();
            
            if (dash != null)
            {
                // Mengambil targetPersenHutan dari DashboardUI
                float targetHutan = dash.targetPersenHutan;
                
                // Hitung persentase bar dashboard
                float persenDashboard = (manager.totalLuasTajuk / manager.luasLahanTotal) / targetHutan * 100f;
                
                if(tombolRekrut != null)
                {
                    tombolRekrut.interactable = (persenDashboard >= 40f && manager.uangPemain >= hargaRekrut);
                }
            }
        }
        else if (namaNPC == "Pemandu")
        {
            string status = manager.AmbilStatusWilayah();
            if(tombolRekrut != null)
                tombolRekrut.interactable = (status == "AMAN" || status == "OPTIMAL") && manager.uangPemain >= hargaRekrut;
        }
    }
}