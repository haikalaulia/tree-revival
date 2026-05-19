using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Wajib untuk pindah scene

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

	[Header("Target Sasaran (Max)")]
	public float targetCO2Max = 1000f;
	public float targetAirMax = 100000f;
	public int targetPekerjaanMax = 50;

	[Header("UI Pindah Wilayah")]
	public GameObject panelPindahWilayah;
	public bool panelSudahMuncul = false;
	public string namaSceneTujuan = "NamaSceneKamu"; // Ubah sesuai nama scene kamu

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

		if (panelPindahWilayah != null)
			panelPindahWilayah.SetActive(false);
	}

	void Update()
	{
		if (ui == null) return;

		// --- CHEAT MODE: Tekan L ---
		if (Input.GetKeyDown(KeyCode.L))
		{
			ui.totalLuasTajuk = ui.luasLahanTotal;
			Debug.Log("Cheat Aktif: Tutupan 100%");
		}

		float targetTutupan = (ui.totalLuasTajuk / ui.luasLahanTotal);

		// --- LOGIKA PANEL ---
		if (targetTutupan >= 0.99f && !panelSudahMuncul)
		{
			if (panelPindahWilayah != null)
			{
				panelPindahWilayah.SetActive(true);
				panelSudahMuncul = true;
				Time.timeScale = 0f; // Pause game
			}
		}

		// --- UPDATE SLIDER & TEKS ---
		barTutupan.value = Mathf.Lerp(barTutupan.value, targetTutupan, Time.deltaTime * smoothSpeed);
		barCO2.value = Mathf.Lerp(barCO2.value, (ui.totalCO2 / targetCO2Max), Time.deltaTime * smoothSpeed);
		barAir.value = Mathf.Lerp(barAir.value, (ui.totalAir / targetAirMax), Time.deltaTime * smoothSpeed);
		barPekerjaan.value = Mathf.Lerp(barPekerjaan.value, ((float)ui.totalLapanganKerja / (float)targetPekerjaanMax), Time.deltaTime * smoothSpeed);

		if (txtPersenTutupan != null)
			txtPersenTutupan.text = (targetTutupan * 100f).ToString("F0") + "%";

		txtCO2.text = ui.totalCO2.ToString("F0") + " Ton";
		txtAir.text = ui.totalAir.ToString("N0") + " L";
		txtPekerjaan.text = ui.totalLapanganKerja + " Orang";

		float pPenjaga = ui.AmbilPersenPenjaga();
		float pBuah = (ui.jumlahPohon == 0) ? 0 : 100f - pPenjaga;

		txtPersenPenjaga.text = "Penjaga: " + pPenjaga.ToString("F0") + "%";
		txtPersenBuah.text = "Buah: " + pBuah.ToString("F0") + "%";

		txtStatus.text = "STATUS: " + ui.AmbilStatusWilayah();
	}

	// --- FUNGSI TOMBOL LANJUT ---
	public void TombolLanjutKlik()
	{
		Time.timeScale = 1f; // Aktifkan waktu kembali
		SceneManager.LoadScene(namaSceneTujuan); // Pindah Scene
	}
}