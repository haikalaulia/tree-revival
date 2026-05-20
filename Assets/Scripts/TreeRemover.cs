using UnityEngine;
using UnityEngine.EventSystems;

public class TreeRemover : MonoBehaviour
{
    public bool modeHapusAktif = false;
    public UIManagerToko manager;
    public LayerMask layerPohon; // <--- TAMBAHKAN INI

    public void ToggleModeHapus()
    {
        modeHapusAktif = !modeHapusAktif;
        Debug.Log("Mode Hapus: " + (modeHapusAktif ? "ON" : "OFF"));
    }

    void Update()
    {
        if (modeHapusAktif && Input.GetMouseButtonDown(0))
        {
            // Opsi: Jika kamu ingin bisa hapus pohon meskipun klik di area joystick, 
            // hapus atau comment baris IsPointerOverGameObject di bawah ini.
            // if (EventSystem.current.IsPointerOverGameObject()) return;

            HapusPohonDiPosisiMouse();
        }
    }

    void HapusPohonDiPosisiMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Raycast hanya akan mendeteksi objek yang ada di 'layerPohon'
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, layerPohon);

        if (hit.collider != null)
        {
            // Karena kita sudah pakai LayerMask, pasti yang kena adalah pohon
            Destroy(hit.collider.gameObject);
            
            if (manager != null) manager.jumlahPohon--; 
            Debug.Log("Pohon BERHASIL dihapus lewat LayerMask!");
        }
        else
        {
            Debug.Log("Klik tidak mengenai Layer Pohon. Pastikan prefab pohon sudah di layer 'Pohon'.");
        }
    }
}