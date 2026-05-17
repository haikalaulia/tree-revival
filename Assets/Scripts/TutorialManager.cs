using UnityEngine;
using UnityEngine.UI;
using TMPro; // WAJIB ADA untuk menggunakan TextMeshPro

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Instance;

	[Header("UI Guide Components")]
	public GameObject tutorialGuideObject;
	public TextMeshProUGUI textInstruksi; // Menggunakan TextMeshProUGUI

	[Header("Target Positions")]
	public Transform targetTanah;
	public RectTransform targetBtnToko; // Pasangkan objek Tombol Tanam di Inspector

	private int currentStep = 0;
	private Camera mainCamera;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		mainCamera = Camera.main;
		// MulaiStep1() DI SERINGKAS/DIHAPUS DARI SINI agar panah tidak curi start muncul di awal game
	}

	void Update()
	{
		// Step 1: Mengikuti posisi tanah di World Space (Scene)
		if (currentStep == 1 && targetTanah != null && tutorialGuideObject.activeSelf)
		{
			Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTanah.position);
			// Offset 120 pixel ke atas supaya tidak menutupi tanahnya
			tutorialGuideObject.transform.position = screenPos + new Vector3(0, 120, 0);
		}

		// ========================================================
		// PERBAIKAN UTAMA: Mengunci posisi panah di atas tombol UI saat Step 2
		// ========================================================
		if (currentStep == 2 && targetBtnToko != null && tutorialGuideObject.activeSelf)
		{
			// KODE LAMA KAMU:
			// tutorialGuideObject.transform.position = targetBtnToko.position + new Vector3(0, 100f, 0);

			// ========================================================
			// CARA PERBAIKAN: Modifikasi nilai X dan Y di bawah ini
			// ========================================================
			// Sumbu X: Minus (-) untuk geser ke kiri, Plus (+) untuk geser ke kanan
			// Sumbu Y: Minus (-) untuk geser ke bawah, Plus (+) untuk geser ke atas

			float geserHorizontalX = -80f; // Silakan ubah angka ini jika kurang kiri / terlalu kiri
			float geserVertikalY = 0f;     // Silakan ubah angka ini jika kurang bawah / terlalu bawah

			tutorialGuideObject.transform.position = targetBtnToko.position + new Vector3(geserHorizontalX, geserVertikalY, 0);
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

			// Mengubah teks instruksi agar sesuai dengan alur panel analisis kelompokmu
			if (textInstruksi != null)
			{
				textInstruksi.text = "Klik Tanam untuk merestorasi lahan ini!";
			}

			Debug.Log("Langkah 1 Selesai: Masuk ke Step 2 (Kunci posisi di Tombol Tanam)");
		}
	}

	public void SelesaiStep2Dan3()
	{
		// Mengubah currentStep ke 3 (Selesai)
		currentStep = 3;

		// LANGSUNG PAKSA MATI: Tanpa cek syarat berbelit, langsung matikan UI panduan
		if (tutorialGuideObject != null)
		{
			tutorialGuideObject.SetActive(false);
		}

		Debug.Log("<color=yellow><b>[TUTORIAL]</b></color> Tutorial selesai dan objek panduan telah dimatikan!");
	}
}