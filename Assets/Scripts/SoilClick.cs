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
		// 1. CEK PRESISI TOMBOL DASHBOARD
		// Kita cari objek tombol berdasarkan namanya di hierarki
		GameObject btnDashboard = GameObject.Find("Btn_ToggleStatus");

		if (btnDashboard != null)
		{
			// Ambil posisi tombol di layar
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, btnDashboard.transform.position);

			// Hitung jarak antara mouse dan tengah tombol (dalam pixel)
			float jarak = Vector2.Distance(Input.mousePosition, screenPoint);

			// Jika klik sangat dekat dengan tombol (radius 40-50 pixel), batalkan klik tanah
			// Kamu bisa ubah angka 50f ini sesuai besarnya tombolmu
			if (jarak < 50f)
			{
				return;
			}
		}

		if (sudahDitanami) return;

		UIManagerToko uiToko = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			// Cek jika panel toko atau analisis sedang terbuka
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
	}
}