using UnityEngine;
using UnityEngine.UI;
using TMPro; // WAJIB ADA untuk menggunakan TextMeshPro
using UnityEngine.SceneManagement; // WAJIB ADA untuk memindahkan Scene di akhir tutorial

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance;

	[Header("UI Guide Components")]
	public GameObject tutorialGuideObject;
	public TextMeshProUGUI textInstruksi;

	[Header("Target Positions")]
	public Transform targetTanah;
	public RectTransform targetBtnToko;        // Tombol Tanam (Panel Analisis)
	public RectTransform targetBtnBeliBibit;    // Tombol Beli Mangrove (Panel Toko)
	public RectTransform targetBtnBukaDashboard;// Tombol untuk membuka Dashboard (Hijau di pojok kanan atas)
	public RectTransform targetIsiDashboard;    // Panel Informasi Dashboard yang muncul setelah diklik

	private int currentStep = 0;
	private Camera mainCamera;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		mainCamera = Camera.main;
	}

	void Update()
	{
		if (!tutorialGuideObject.activeSelf) return;

		// Step 1: Mengikuti posisi tanah di World Space
		if (currentStep == 1 && targetTanah != null)
		{
			Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTanah.position);
			tutorialGuideObject.transform.position = screenPos + new Vector3(0, 120, 0);
		}

		// Step 2: Mengunci posisi di Tombol Tanam (Panel Analisis)
		if (currentStep == 2 && targetBtnToko != null)
		{
			tutorialGuideObject.transform.position = targetBtnToko.position + new Vector3(-80f, 0f, 0);
		}

		// Step 3: Mengunci posisi di Tombol Beli Mangrove (Panel Toko)
		if (currentStep == 3 && targetBtnBeliBibit != null)
		{
			tutorialGuideObject.transform.position = targetBtnBeliBibit.position + new Vector3(0f, 60f, 0);
		}

		// Step 4: Menunjuk Tombol Buka Dashboard (Pojok Kanan Atas)
		if (currentStep == 4 && targetBtnBukaDashboard != null)
		{
			tutorialGuideObject.transform.position = targetBtnBukaDashboard.position + new Vector3(-80f, 0f, 0);
		}

		// Step 5: Menunjuk ke Isi Informasi Dashboard (Menggunakan koordinat pas dari kamu)
		if (currentStep == 5 && targetIsiDashboard != null)
		{
			tutorialGuideObject.transform.position = targetIsiDashboard.position + new Vector3(-200f, -60f, 0);
		}
	}

	public void MulaiStep1()
	{
		currentStep = 1;
		if (tutorialGuideObject != null) tutorialGuideObject.SetActive(true);
		if (textInstruksi != null) textInstruksi.text = "Klik petak tanah ini untuk menganalisis kondisinya!";
	}

	public void SelesaiStep1()
	{
		if (currentStep == 1)
		{
			currentStep = 2;
			if (textInstruksi != null) textInstruksi.text = "Klik Tanam untuk merestorasi lahan ini!";
		}
	}

	public void SelesaiStep2()
	{
		if (currentStep == 2)
		{
			currentStep = 3;
			if (textInstruksi != null) textInstruksi.text = "Pilih bibit pohon Mangrove untuk memulai restorasi lingkungan!";
		}
	}

	// Dipanggil saat Tombol Beli Mangrove diklik
	public void SelesaiStep3()
	{
		if (currentStep == 3)
		{
			currentStep = 4;
			if (textInstruksi != null) textInstruksi.text = "Pohon berhasil ditanam! Sekarang, klik tombol <b>Dashboard</b> untuk melihat status kondisi lingkungan lahanmu!";
			Debug.Log("Masuk ke Step 4: Suruh klik Dashboard");
		}
	}

	// SINKRONISASI BARU: Fungsi tunggal untuk mengontrol klik tombol Dashboard
	public void KlikTombolDashboard()
	{
		// KLIK PERTAMA (Saat masih Step 4): Membuka panel & mengubah instruksi ke edukasi bencana
		if (currentStep == 4)
		{
			currentStep = 5;
			if (textInstruksi != null)
				textInstruksi.text = "Perhatikan Dashboard! Pastikan Rasio Pohon Penjaga tetap 60% dan Total Air memenuhi target lahan agar tidak terjadi Bencana Alam! \n\n<b>(Klik tombol Dashboard sekali lagi untuk menutup dan menyelesaikan tugas)</b>";

			Debug.Log("Masuk ke Step 5: Dashboard Terbuka & Edukasi Aktif");
			return; // Ditahan dulu di sini agar tidak langsung ganti scene pada klik pertama
		}

		// KLIK KEDUA (Saat sudah Step 5): Menutup panel, menamatkan tutorial, lalu pindah scene!
		if (currentStep == 5)
		{
			if (tutorialGuideObject != null) tutorialGuideObject.SetActive(false);
			Debug.Log("Tutorial tamat sepenuhnya via tombol Dashboard! Memuat SampleScene utama...");
			SceneManager.LoadScene("SampleScene");
		}
	}
}