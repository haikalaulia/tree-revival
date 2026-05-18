using UnityEngine;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{
	[Header("UI Components")]
	public Button tombolMulaiTugas; // Tarik tombol dari PanelIntro ke sini
	public GameObject panelIntro;   // Tarik objek PanelIntro ke sini

	void Start()
	{
		if (tombolMulaiTugas != null)
		{
			tombolMulaiTugas.onClick.AddListener(MulaiTutorialDiTempat);
		}
	}

	void MulaiTutorialDiTempat()
	{
		// 1. Tutup panel cerita intro biar hilang dari layar
		if (panelIntro != null) panelIntro.SetActive(false);

		// 2. Langsung panggil panah tutorial langkah 1 (menunjuk tanah)
		if (TutorialManager.Instance != null)
		{
			TutorialManager.Instance.MulaiStep1();
		}

		Debug.Log("Panel Intro ditutup, Tutorial dimulai!");
	}
}