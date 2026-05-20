using UnityEngine;
using UnityEngine.EventSystems;

public class TreeRemover : MonoBehaviour
{
    public bool modeHapusAktif = false;
    public UIManagerToko manager;

    // Fungsi untuk menyalakan/mematikan mode hapus (dipanggil tombol UI)
    public void ToggleModeHapus()
    {
        modeHapusAktif = !modeHapusAktif;
        Debug.Log("Mode Hapus: " + (modeHapusAktif ? "ON" : "OFF"));
        
        // Ubah warna kursor atau beri notifikasi visual agar pemain tahu sedang di mode hapus
    }

    void Update()
    {
        // Jika mode hapus aktif dan pemain klik mouse kiri
        if (modeHapusAktif && Input.GetMouseButtonDown(0))
        {
            // Jangan hapus jika klik di atas tombol UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            HapusPohonDiPosisiMouse();
        }
    }

    void HapusPohonDiPosisiMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Gunakan Raycast untuk mencari objek di bawah mouse
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            // Cek apakah yang diklik adalah pohon
            // Pastikan Prefab pohon kamu punya Tag "Pohon" atau masuk Layer khusus
            if (hit.collider.CompareTag("Pohon"))
            {
                Destroy(hit.collider.gameObject);
                
                // Kurangi jumlah pohon di data manager agar Dashboard terupdate
                if (manager != null) manager.jumlahPohon--; 
                
                Debug.Log("Pohon berhasil dihapus!");
            }
        }
    }
}