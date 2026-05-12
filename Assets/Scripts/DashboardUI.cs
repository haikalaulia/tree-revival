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

	[Header("Angka & Status")]
	public TextMeshProUGUI txtPersenTutupan; // Taruh di samping Bar Tutupan (biar tidak stuck 60%)
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

		// 1. UPDATE BAR TUTUPAN (MENGGUNAKAN LUAS M2)
		// Kita hitung persentase real-nya di sini
		float persenReal = (ui.totalLuasTajuk / ui.luasLahanTotal);
		barTutupan.value = persenReal; // Slider (0 sampai 1)

		// Update teks persentase yang ada di UI kamu agar sinkron dengan bar
		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (persenReal * 100f).ToString("F0") + "%";

		barCO2.value = ui.totalCO2 / 1000f;
		barAir.value = ui.totalAir / 100000f;
		barPekerjaan.value = (float)ui.totalLapanganKerja / 50f;

		// 2. Update Teks Angka
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

		// Warna Status
		if (txtStatus.text.Contains("KRITIS")) txtStatus.color = Color.red;
		else if (txtStatus.text.Contains("WASPADA")) txtStatus.color = Color.yellow;
		else txtStatus.color = Color.green;
	}
}