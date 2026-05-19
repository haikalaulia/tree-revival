using UnityEngine;

public class LandCoverSystem : MonoBehaviour
{
	public UIManagerToko uiManager; // Tarik script UIManager kamu ke sini
	public Transform folderPohon;   // Tarik folder parent pohon kamu ke sini
	public GameObject panelPindah;  // Panel yang akan muncul

	// Target yang harus dicapai
	public int targetPohon = 15;
	public float targetAir = 13500;
	public float targetCO2 = 100;
	public int targetPenjaga = 9;
	public int targetBuah = 6;

	private bool sudahMuncul = false;

	// Fungsi ini dipanggil setiap kali ada perubahan di lahan
	public void CekKondisiLahan()
	{
		if (sudahMuncul) return; // Agar panel tidak muncul berulang kali

		int jumlahPohon = folderPohon.childCount;
		float pPenjaga = uiManager.AmbilPersenPenjaga();
		int hitungPenjaga = Mathf.RoundToInt((pPenjaga / 100f) * jumlahPohon);
		int hitungBuah = jumlahPohon - hitungPenjaga;

		// Cek 100%
		if (jumlahPohon >= targetPohon &&
			uiManager.totalAir >= targetAir &&
			uiManager.totalCO2 >= targetCO2 &&
			hitungPenjaga >= targetPenjaga &&
			hitungBuah >= targetBuah)
		{
			panelPindah.SetActive(true);
			sudahMuncul = true;
		}
	}
}