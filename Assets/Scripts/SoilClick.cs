using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class SoilClick : MonoBehaviour
{
    [Header("Settings Tanah")]
    public SoilProperty dataTanah;

    [HideInInspector] public Vector3 posisiGridTanam;

    private void OnMouseDown()
    {
        // 1. CEK BLOKIR UI
        if (IsClickBlockedByUI()) return;

        // 2. Tentukan posisi klik dunia (DEKLARASI HANYA SEKALI DI SINI)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap != null)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
            posisiGridTanam = tilemap.GetCellCenterWorld(cellPosition);
        }
        else
        {
            mouseWorldPos.z = 0;
            posisiGridTanam = mouseWorldPos;
        }

        // 3. CEK APAKAH SUDAH ADA POHON DI TITIK INI
        if (CekApakahSudahAdaPohon(posisiGridTanam)) 
        {
            Debug.Log("Gagal: Titik ini sudah ada pohonnya!");
            return; 
        }

        // 4. CEK PANEL PENUGASAN / INTRO
        GameObject panelIntroObj = GameObject.Find("Penugasan") ?? GameObject.Find("PanelIntro");
        if (panelIntroObj != null && panelIntroObj.activeInHierarchy)
            return;

        // 5. CEK PRESISI TOMBOL DASHBOARD
        GameObject btnDashboard = GameObject.Find("Btn_ToggleStatus");
        if (btnDashboard != null)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, btnDashboard.transform.position);
            float jarak = Vector2.Distance(Input.mousePosition, screenPoint);
            if (jarak < 50f) return;
        }

        // 6. BUKA UI TOKO / TANAM
        UIManagerToko uiToko = FindFirstObjectByType<UIManagerToko>();
        if (uiToko != null)
        {
            if (uiToko.panelToko.activeInHierarchy ||
               (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
                return;

            uiToko.SetTanahAktif(this);
        }

        // 7. TAMPILKAN PANEL INFO TANAH
        PanelManager pm = FindFirstObjectByType<PanelManager>();
        if (pm != null)
            pm.TampilkanPanel(dataTanah);

        // 8. HUBUNGKAN KE SISTEM TUTORIAL
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.SelesaiStep1();
    }

    private bool IsClickBlockedByUI()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null) return false;
        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                return true;
        }
        return false;
    }

    private bool CekApakahSudahAdaPohon(Vector3 posisi)
    {
        GameObject folderPohon = GameObject.Find("pohon");
        if (folderPohon == null) return false;

        foreach (Transform pohon in folderPohon.transform)
        {
            if (Vector2.Distance(pohon.position, posisi) < 0.5f)
            {
                return true; 
            }
        }
        return false; 
    }

    public void EksekusiJalanLaluTanam(Action fungsiAsliTanamPohon)
    {
        PlayerMovement player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.PerintahJalanKeTanah(posisiGridTanam, () => {
                if (fungsiAsliTanamPohon != null) fungsiAsliTanamPohon.Invoke();
                Debug.Log("Pohon berhasil ditanam otomatis.");
            });
        }
        else
        {
            if (fungsiAsliTanamPohon != null) fungsiAsliTanamPohon.Invoke();
        }
    }
}