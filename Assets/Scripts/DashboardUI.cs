using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public float targetPersenHutan = 0.05f; // Isi 0.05 untuk 5%, 0.5 untuk 50%
	public float targetCO2Max = 1000f;
	public float targetAirMax = 100000f;
	public int targetPekerjaanMax = 50;

	[Header("UI Pindah Wilayah")]
	public GameObject panelPindahWilayah;
	public bool panelSudahMuncul = false;
	public string namaSceneTujuan = "Level2"; 

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
        gdm = Object.FindFirstObjectByType<GameDisasterManager>();

		if (panelPindahWilayah != null)
			panelPindahWilayah.SetActive(false);
	}

	void Update()
	{
		if (ui == null || gdm == null) return;

		// --- LOGIKA PROGRESS BAR DINAMIS ---
        
        // 1. Tutupan (Disesuaikan dengan targetPersenHutan)
		float persenReal = (ui.totalLuasTajuk / ui.luasLahanTotal);
        float targetTutupanBar = persenReal / targetPersenHutan;

        // 2. CO2, Air, Kerja (Disesuaikan dengan Target Max)
		float targetCO2 = ui.totalCO2 / targetCO2Max;
		float targetAir = ui.totalAir / targetAirMax;
		float targetKerja = (float)ui.totalLapanganKerja / (float)targetPekerjaanMax;

		// Gerakkan slider secara halus
		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupanBar, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, targetCO2, Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, targetAir, Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, targetKerja, Time.deltaTime * smoothSpeed);

		// --- LOGIKA MENANG / PINDAH LEVEL ---
		if (barTutupan.value >= 0.99f && !panelSudahMuncul)
		{
			if (panelPindahWilayah != null)
			{
				panelPindahWilayah.SetActive(true);
				panelSudahMuncul = true;
				Time.timeScale = 0f;
			}
		}

		// --- UPDATE SEMUA TEKS ---
		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (barTutupan.value * 100f).ToString("F0") + "%";

		txtCO2.text = ui.totalCO2.ToString("F0") + " Kg";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		if (txtJmlPenjaga != null)
			txtJmlPenjaga.text = "Penjaga: " + ui.jmlPenjaga + " / " + gdm.targetPenjaga;

		if (txtJmlBuah != null)
			txtJmlBuah.text = "Berbuah: " + ui.jmlBerbuah + " / " + gdm.targetBuah;

		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();
	}

	public void TombolLanjutKlik()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(namaSceneTujuan);
	}
}