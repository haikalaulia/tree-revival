using UnityEngine;
using UnityEngine.SceneManagement; // Penting untuk pindah scene

public class MenuController : MonoBehaviour
{
    public GameObject panelMenu;

    public void BukaMenu()
    {
        panelMenu.SetActive(true);
        Time.timeScale = 0f; // Berhenti waktu game (Pause)
    }

    public void TutupMenu()
    {
        panelMenu.SetActive(false);
        Time.timeScale = 1f; // Jalankan waktu game lagi
    }

    public void KeluarGame()
    {
        Debug.Log("Keluar dari Game...");
        Application.Quit(); // Hanya berfungsi setelah game di-build (.exe)
    }

    public void SimpanGame()
    {
        // Untuk tahap awal, kita beri pesan saja dulu
        Debug.Log("Game Berhasil Disimpan!");
    }
}