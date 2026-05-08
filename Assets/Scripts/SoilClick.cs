using UnityEngine;

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;
	public bool sudahDitanami = false;

	private void OnMouseDown()
	{
		// 1. Jika sudah ada pohon, jangan kasih klik sama sekali
		if (sudahDitanami) return;

		// 2. Mencegah klik tembus jika panel toko sedang aktif
		UIManagerToko uiToko = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null && uiToko.panelToko.activeSelf) return;

		// 3. LAPOR: Penting agar Manager tahu kita pindah grid
		if (uiToko != null)
		{
			uiToko.SetTanahAktif(this);
		}

		// 4. Buka Panel Analisis
		PanelManager pm = Object.FindFirstObjectByType<PanelManager>();
		if (pm != null)
		{
			pm.TampilkanPanel(dataTanah);
		}
	}
}