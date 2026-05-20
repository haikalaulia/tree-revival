using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DashboardUI : MonoBehaviour
{
	private UIManagerToko ui;
    private GameDisasterManager gdm; // Tambahkan referensi ke script GameManager temanmu

	[Header("Slider Bars")]
	public Slider barTutupan;
	public Slider barCO2;
	public Slider barAir;
	public Slider barPekerjaan;

	[Header("Smooth Settings")]
	public float smoothSpeed = 2f;

	[Header("Target Sasaran (Max)")]
	public float targetCO2Max = 1000f;
	public float targetAirMax = 100000f;
	public int targetPekerjaanMax = 50;

	[Header("UI Pindah Wilayah")]
	public GameObject panelPindahWilayah;
	public bool panelSudahMuncul = false;
	public string namaSceneTujuan = "NamaSceneKamu"; 

	[Header("Angka & Status")]
	public TextMeshProUGUI txtPersenTutupan;
	public TextMeshProUGUI txtCO2;
	public TextMeshProUGUI txtAir;
	public TextMeshProUGUI txtPekerjaan;
	public TextMeshProUGUI txtStatus;

	[Header("Persentase VS")]
	public TextMeshProUGUI txtPersenPenjaga;
	public TextMeshProUGUI txtPersenBuah;

	[Header("Jumlah Real")]
	public TextMeshProUGUI txtJmlPenjaga; 
	public TextMeshProUGUI txtJmlBuah;

	void Start()
	{
		ui = Object.FindFirstObjectByType<UIManagerToko>();
        gdm = Object.FindFirstObjectByType<GameDisasterManager>(); // Cari script GameManager otomatis

		if (panelPindahWilayah != null)
			panelPindahWilayah.SetActive(false);
	}

	void Update()
	{
		if (ui == null || gdm == null) return;

		// --- LOGIKA PROGRESS BAR (Tetap Sama) ---
		float targetTutupan = (ui.totalLuasTajuk / ui.luasLahanTotal);
		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupan, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, (ui.totalCO2 / targetCO2Max), Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, (ui.totalAir / targetAirMax), Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, ((float)ui.totalLapanganKerja / (float)targetPekerjaanMax), Time.deltaTime * smoothSpeed);

		// --- UPDATE TEKS PERSENTASE ---
		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (targetTutupan * 100f).ToString("F0") + "%";

		float persenCO2 = (ui.totalCO2 / targetCO2Max) * 100f;
		txtCO2.text = persenCO2.ToString("F0") + "%";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		// --- UPDATE JUMLAH REAL (PENJAGA & BUAH) DENGAN TARGET ---
        // Format: "Penjaga: 3 / 6"
		if (txtJmlPenjaga != null)
			txtJmlPenjaga.text = "Penjaga: " + ui.jmlPenjaga + " / " + gdm.targetPenjaga;

		if (txtJmlBuah != null)
			txtJmlBuah.text = "Berbuah: " + ui.jmlBerbuah + " / " + gdm.targetBuah;

		// --- UPDATE STATUS WILAYAH ---
		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();
	}

	public void TombolLanjutKlik()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(namaSceneTujuan);
	}
}