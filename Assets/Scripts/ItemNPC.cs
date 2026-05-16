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

        if (manager.uangPemain >= hargaRekrut) {
            manager.uangPemain -= hargaRekrut;

            // MUNCULKAN NPC DI DUNIA
            // Muncul di lokasi tertentu (misal: di pos jaga atau dekat player)
            Vector3 posisi = (lokasiMuncul != null) ? lokasiMuncul.position : Vector3.zero;
            Instantiate(prefabNPC, posisi, Quaternion.identity);

            // Aktifkan efek spesial (misal: Peneliti)
            if(namaNPC == "Peneliti") manager.adaPeneliti = true;

            Debug.Log(namaNPC + " Telah Didatangkan!");
            
            // Opsional: Tutup panel setelah beli
            manager.TutupToko(); 
        } else {
            Debug.Log("Uang tidak cukup untuk merekrut " + namaNPC);
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