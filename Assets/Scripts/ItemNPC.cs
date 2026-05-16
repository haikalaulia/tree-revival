using UnityEngine;
using UnityEngine.UI;

public class ItemNPC : MonoBehaviour
{
    public string namaNPC;
    public int hargaRekrut;
    public GameObject prefabNPC; // Masukkan Prefab NPC dari folder
    public Transform lokasiMuncul; // Titik di mana NPC akan muncul (misal: gerbang masuk)

    private UIManagerToko manager;

    void Start() {
        manager = FindFirstObjectByType<UIManagerToko>();
    }

    public void KlikRekrut() {
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
}