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
	public RectTransform targetBtnToko;

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
			if (tutorialGuideObject != null && targetBtnToko != null)
			{
				// Pindah posisi tepat di atas Tombol Toko (UI Space Canvas)
				tutorialGuideObject.transform.position = targetBtnToko.position + new Vector3(0, 150, 0);
			}
			if (textInstruksi != null) textInstruksi.text = "Buka Toko untuk memilih bibit pohon yang cocok!";
		}
	}

	public void SelesaiStep2Dan3()
	{
		if (currentStep == 2)
		{
			currentStep = 3;
			if (tutorialGuideObject != null) tutorialGuideObject.SetActive(false);
			Debug.Log("Tutorial Selesai!");
		}
	}
}