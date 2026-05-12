using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject panelDashboard; // Tempat menaruh PanelDashBoard nanti

    public void TogglePanel()
    {
        // Jika sedang aktif maka jadi tidak aktif, dan sebaliknya
        bool isActive = panelDashboard.activeSelf;
        panelDashboard.SetActive(!isActive);
    }
}