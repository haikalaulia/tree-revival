using UnityEngine;
using UnityEngine.Tilemaps;

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;
	public bool sudahDitanami = false;

	[HideInInspector] public Vector3 posisiGridTanam;

	private void OnMouseDown()
	{
		// ========================================================
		// PERBAIKAN: CEK APAKAH PANEL INTRO SEDANG TERBUKA
		// ========================================================
		// Menggunakan nama objek "Penugasan" berdasarkan judul panel visualmu
		GameObject panelIntroObj = GameObject.Find("Penugasan");

		// Backup cek jika nama di hierarchy kamu masih pakai nama default "PanelIntro"
		if (panelIntroObj == null)
		{
			panelIntroObj = GameObject.Find("PanelIntro");
		}

		// Jika panelnya ketemu dan lagi aktif di layar, batalkan klik tanah agar tidak tembus!
		if (panelIntroObj != null && panelIntroObj.activeInHierarchy)
		{
			return;
		}

		// 1. CEK PRESISI TOMBOL DASHBOARD
		GameObject btnDashboard = GameObject.Find("Btn_ToggleStatus");

		if (btnDashboard != null)
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, btnDashboard.transform.position);
			float jarak = Vector2.Distance(Input.mousePosition, screenPoint);

			if (jarak < 50f)
			{
				return;
			}
		}

		if (sudahDitanami) return;

		UIManagerToko uiToko = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			if (uiToko.panelToko.activeInHierarchy ||
			   (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
			{
				return;
			}

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
			uiToko.SetTanahAktif(this);
		}

		PanelManager pm = Object.FindFirstObjectByType<PanelManager>();
		if (pm != null)
		{
			pm.TampilkanPanel(dataTanah);
		}

		// ==========================================
		// HUBUNGKAN KE SISTEM TUTORIAL
		// ==========================================
		if (TutorialManager.Instance != null)
		{
			TutorialManager.Instance.SelesaiStep1();
		}
	}
}