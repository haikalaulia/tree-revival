using UnityEngine;
using UnityEngine.Tilemaps;

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;
	public bool sudahDitanami = false;

	// Menyimpan posisi grid yang diklik
	[HideInInspector] public Vector3 posisiGridTanam;

	private void OnMouseDown()
	{
		if (sudahDitanami) return;

		UIManagerToko uiToko = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			// Mencegah klik jika menu masih terbuka
			if (uiToko.panelToko.activeInHierarchy || (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
			{
				return;
			}

			// AMBIL POSISI GRID:
			// 1. Ambil posisi mouse di dunia
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// 2. Cari komponen Tilemap di objek ini
			Tilemap tilemap = GetComponent<Tilemap>();
			if (tilemap != null)
			{
				// Konversi posisi mouse ke koordinat cell grid
				Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
				// Kembalikan ke posisi dunia yang pas di tengah cell tersebut
				posisiGridTanam = tilemap.GetCellCenterWorld(cellPosition);
			}
			else
			{
				// Jika bukan Tilemap, gunakan posisi mouse biasa
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