using UnityEngine;

public class DashboardController : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject panelDashboard;
    public GameObject panelTenagaKerja; // Tambahkan baris ini

    public void ToggleDashboard()
    {
        if (panelDashboard != null)
        {
            bool isActive = panelDashboard.activeSelf;
            panelDashboard.SetActive(!isActive);
        }
    }

    // FUNGSI BARU UNTUK TENAGA KERJA
    public void ToggleTenagaKerja()
    {
        if (panelTenagaKerja != null)
        {
            bool isActive = panelTenagaKerja.activeSelf;
            panelTenagaKerja.SetActive(!isActive);
        }
    }
}