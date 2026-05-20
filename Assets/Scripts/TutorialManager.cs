using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance;

	[Header("UI Guide Components")]
	public GameObject tutorialGuideObject;
	public TextMeshProUGUI textInstruksi;

	[Header("Target Positions (Atur Offset di bawah)")]
	public Transform targetTanah;
	public RectTransform targetBtnToko;
	public RectTransform targetBtnBeliBibit;
	public RectTransform targetBtnBukaDashboard;
	public RectTransform targetIsiDashboard;

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
		if (tutorialGuideObject == null || !tutorialGuideObject.activeSelf) return;

		// Step 1: Posisi di Tanah
		if (currentStep == 1 && targetTanah != null)
		{
			Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTanah.position);
			tutorialGuideObject.transform.position = screenPos + new Vector3(0, 120, 0);
		}

		// Step 2: Posisi di Tombol Tanam
		if (currentStep == 2 && targetBtnToko != null)
		{
			tutorialGuideObject.transform.position = targetBtnToko.position + new Vector3(-80f, 0f, 0);
		}

		// Step 3: Posisi di Tombol Beli Bibit
		if (currentStep == 3 && targetBtnBeliBibit != null)
		{
			// UBAH ANGKA INI UNTUK MENGGESER PANAH BIBIT
			tutorialGuideObject.transform.position = targetBtnBeliBibit.position + new Vector3(-50f, 0f, 0);
		}

		// Step 4: Posisi di Tombol Dashboard
		if (currentStep == 4 && targetBtnBukaDashboard != null)
		{
			tutorialGuideObject.transform.position = targetBtnBukaDashboard.position + new Vector3(-80f, 0f, 0);
		}

		// Step 5: Posisi di Isi Dashboard
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

	public void SelesaiStep3()
	{
		if (currentStep == 3)
		{
			currentStep = 4;
			if (textInstruksi != null) textInstruksi.text = "Pohon berhasil ditanam! Sekarang, klik tombol <b>Dashboard</b> untuk melihat status kondisi lingkungan lahanmu!";
		}
	}

	public void KlikTombolDashboard()
	{
		if (currentStep == 4)
		{
			currentStep = 5;
			if (textInstruksi != null)
				textInstruksi.text = "Periksa Dashboard! Capai target Tutupan Lahan minimal 60% dan pastikan ketersediaan Air cukup untuk menjaga lingkungan dari risiko bencana! \n\n(Klik tombol Dashboard sekali lagi untuk menutup dan menyelesaikan tugas";
			return;
		}

		if (currentStep == 5)
		{
			if (tutorialGuideObject != null) tutorialGuideObject.SetActive(false);

			// Ubah nama scene di dalam tanda petik di bawah ini:
			SceneManager.LoadScene("Scene Map 1 Test");
		}
	}
}