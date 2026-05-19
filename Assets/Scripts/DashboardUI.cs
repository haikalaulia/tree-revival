using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardUI : MonoBehaviour
{
	private UIManagerToko ui;

	[Header("Slider Bars")]
	public Slider barTutupan;
	public Slider barCO2;
	public Slider barAir;
	public Slider barPekerjaan;

    [Header("Smooth Settings")]
    public float smoothSpeed = 2f; // Semakin besar angkanya, semakin cepat bar mengisi

	[Header("Angka & Status")]
	public TextMeshProUGUI txtPersenTutupan; 
	public TextMeshProUGUI txtCO2;
	public TextMeshProUGUI txtAir;
	public TextMeshProUGUI txtPekerjaan;
	public TextMeshProUGUI txtStatus;

	[Header("Persentase VS")]
	public TextMeshProUGUI txtPersenPenjaga;
	public TextMeshProUGUI txtPersenBuah;

	void Start()
	{
		ui = Object.FindFirstObjectByType<UIManagerToko>();
	}

	void Update()
	{
		if (ui == null) return;

		// --- 1. LOGIKA SMOOTH FILLING (MENGGUNAKAN LERP) ---
        
		// Hitung target nilai (tujuan akhirnya)
		float targetTutupan = (ui.totalLuasTajuk / ui.luasLahanTotal);
		float targetCO2 = ui.totalCO2 / 1000f;
		float targetAir = ui.totalAir / 100000f;
		float targetKerja = (float)ui.totalLapanganKerja / 50f;

		// Gerakkan slider secara halus dari nilai sekarang ke target
		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupan, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, targetCO2, Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, targetAir, Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, targetKerja, Time.deltaTime * smoothSpeed);

		// --- 2. UPDATE TEKS AGAR MENGIKUTI PERGERAKAN BAR ---
        
		if (txtPersenTutupan != null)
            // Gunakan nilai slider (yang sedang bergerak halus) untuk teksnya
			txtPersenTutupan.text = (barTutupan.value * 100f).ToString("F0") + "%";

		txtCO2.text = ui.totalCO2.ToString("F0") + " Ton";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		// 3. Update Persentase VS
		float pPenjaga = ui.AmbilPersenPenjaga();
		float pBuah = (ui.jumlahPohon == 0) ? 0 : 100f - pPenjaga;

		txtPersenPenjaga.text = "Penjaga: " + pPenjaga.ToString("F0") + "%";
		txtPersenBuah.text = "Buah: " + pBuah.ToString("F0") + "%";

		// 4. Update Status
		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();

		// Warna Status (Tetap sama)
		if (txtStatus.text.Contains("KRITIS")) txtStatus.color = Color.red;
		else if (txtStatus.text.Contains("WASPADA")) txtStatus.color = Color.yellow;
		else txtStatus.color = Color.green;
	}
}