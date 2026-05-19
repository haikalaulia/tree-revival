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
    public float smoothSpeed = 2f;

    // --- TAMBAHAN BARU: TARGET MAX YANG BISA DIATUR ---
    [Header("Target Sasaran (Max)")]
    public float targetCO2Max = 1000f;      // Contoh: 1000 Ton untuk 100%
    public float targetAirMax = 100000f;    // Contoh: 100.000 Liter untuk 100%
    public int targetPekerjaanMax = 50;     // Contoh: 50 Orang untuk 100%
    // -------------------------------------------------

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

		// --- LOGIKA TARGET (MENGGUNAKAN VARIABEL MAX) ---
        
		float targetTutupan = (ui.totalLuasTajuk / ui.luasLahanTotal);
        
        // Sekarang kita bagi dengan variabel Max, bukan angka mentah lagi
		float targetCO2 = ui.totalCO2 / targetCO2Max;
		float targetAir = ui.totalAir / targetAirMax;
		float targetKerja = (float)ui.totalLapanganKerja / (float)targetPekerjaanMax;

		// Gerakkan slider secara halus
		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupan, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, targetCO2, Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, targetAir, Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, targetKerja, Time.deltaTime * smoothSpeed);

		// --- UPDATE TEKS (Tetap sama) ---
		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (barTutupan.value * 100f).ToString("F0") + "%";

		txtCO2.text = ui.totalCO2.ToString("F0") + " Ton";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		// Update Persentase VS
		float pPenjaga = ui.AmbilPersenPenjaga();
		float pBuah = (ui.jumlahPohon == 0) ? 0 : 100f - pPenjaga;

		txtPersenPenjaga.text = "Penjaga: " + pPenjaga.ToString("F0") + "%";
		txtPersenBuah.text = "Buah: " + pBuah.ToString("F0") + "%";

		// Update Status
		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();

		if (txtStatus.text.Contains("KRITIS")) txtStatus.color = Color.red;
		else if (txtStatus.text.Contains("WASPADA")) txtStatus.color = Color.yellow;
		else txtStatus.color = Color.green;
	}
}