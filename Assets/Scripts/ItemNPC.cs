using UnityEngine;
using UnityEngine.UI;

public class ItemNPC : MonoBehaviour
{
    public string namaNPC;
    public int hargaRekrut;
    public GameObject prefabNPC; // Masukkan Prefab NPC dari folder
    public Transform lokasiMuncul; // Titik di mana NPC akan muncul (misal: gerbang masuk)

    public Button tombolRekrut; // Tombol untuk merekrut NPC ini
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

            // Tambah angka ke manager sesuai jenis
            if(namaNPC == "Peneliti") { manager.jmlPeneliti++; manager.adaPeneliti = true; }
            else if(namaNPC == "Petani") manager.jmlPetani++;
            else if(namaNPC == "Penjaga") manager.jmlPenjaga++;
            else if(namaNPC == "Pemandu") manager.jmlPemandu++;

            // Munculkan NPC
            Vector3 posisi = (lokasiMuncul != null) ? lokasiMuncul.position : Vector3.zero;
            Instantiate(prefabNPC, posisi, Quaternion.identity);

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
        // Tombol hanya menyala jika status AMAN atau OPTIMAL
        if(tombolRekrut != null)
            tombolRekrut.interactable = (status == "AMAN" || status == "OPTIMAL") && manager.uangPemain >= hargaRekrut;
        }
    }
}