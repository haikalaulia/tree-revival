using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DashboardUI : MonoBehaviour
{
	private UIManagerToko ui;
	private GameDisasterManager gdm;

	[Header("Slider Bars")]
	public Slider barTutupan;
	public Slider barCO2;
	public Slider barAir;
	public Slider barPekerjaan;

	[Header("Smooth Settings")]
	public float smoothSpeed = 2f;

	[Header("Target Level Dinamis")]
	[Range(0.01f, 1f)]
	public float targetPersenHutan = 0.05f; 
	public float targetCO2Max = 1000f;
	public float targetAirMax = 100000f;
	public int targetPekerjaanMax = 50;

	[Header("UI Pindah Wilayah")]
	public GameObject panelPindahWilayah;
	public bool panelSudahMuncul = false;
	public string namaSceneTujuan = "Scene Map 2 Test"; 

	[Header("Angka & Status")]
	public TextMeshProUGUI txtPersenTutupan;
	public TextMeshProUGUI txtCO2;
	public TextMeshProUGUI txtAir;
	public TextMeshProUGUI txtPekerjaan;
	public TextMeshProUGUI txtStatus;

	[Header("Jumlah Real & Persen")]
	public TextMeshProUGUI txtJmlPenjaga; 
	public TextMeshProUGUI txtJmlBuah;

	void Start()
	{
		ui = Object.FindFirstObjectByType<UIManagerToko>();
		gdm = Object.FindFirstObjectByType<GameDisasterManager>();

		if (panelPindahWilayah != null)
			panelPindahWilayah.SetActive(false);
	}

	void Update()
	{
		if (ui == null || gdm == null) return;

		// --- LOGIKA PROGRESS BAR ---
		float persenReal = (ui.totalLuasTajuk / ui.luasLahanTotal);
		float targetTutupanBar = persenReal / targetPersenHutan;

		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupanBar, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, (ui.totalCO2 / targetCO2Max), Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, (ui.totalAir / targetAirMax), Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, ((float)ui.totalLapanganKerja / (float)targetPekerjaanMax), Time.deltaTime * smoothSpeed);

		// --- LOGIKA MENANG ---
		if (targetTutupanBar >= 0.99f && !panelSudahMuncul)
		{
			if (panelPindahWilayah != null)
			{
				panelPindahWilayah.SetActive(true);
				panelSudahMuncul = true;
				Time.timeScale = 0f;
			}
		}

		// --- UPDATE TEKS PERSENTASE ---
		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (barTutupan.value * 100f).ToString("F0") + "%";

		txtCO2.text = ui.totalCO2.ToString("F0") + " Kg";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		// --- UPDATE PERSEN PENJAGA & BUAH (PERBAIKAN ERROR) ---
		int totalPohon = ui.jumlahPohon;
		float persenPenjaga = (totalPohon > 0) ? ((float)ui.jmlPohonPenjaga / totalPohon) * 100f : 0f;
		float persenBuah = (totalPohon > 0) ? ((float)ui.jmlBerbuah / totalPohon) * 100f : 0f;

		if (txtJmlPenjaga != null)
		{
			// Format: Penjaga: [Jumlah] ([Persen Sekarang]% / [Syarat]%)
			txtJmlPenjaga.text = "Penjaga: " + ui.jmlPohonPenjaga + " (" + persenPenjaga.ToString("F0") + "% / " + gdm.syaratRasioPenjaga + "%)";
			
			// Jika di bawah 60% warna merah
			txtJmlPenjaga.color = (persenPenjaga < gdm.syaratRasioPenjaga) ? Color.red : Color.white;
		}

		if (txtJmlBuah != null)
		{
			txtJmlBuah.text = "Berbuah: " + ui.jmlBerbuah + " (" + persenBuah.ToString("F0") + "% / " + gdm.syaratRasioBerbuah + "%)";
			
			// Jika di bawah 40% warna merah
			txtJmlBuah.color = (persenBuah < gdm.syaratRasioBerbuah) ? Color.red : Color.white;
		}

		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();
	}

	public void TombolLanjutKlik()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(namaSceneTujuan);
	}
}