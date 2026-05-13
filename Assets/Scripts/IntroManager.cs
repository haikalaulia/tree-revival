using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
	[Header("UI Reference")]
	public GameObject panelIntro;
	public Button btnMulai;

	void Start()
	{
		// 1. Cek dulu, apakah di "buku catatan" (PlayerPrefs) sudah ada tanda 'PernahBaca'?
		// Jika nilainya 0 (berarti belum pernah), maka panel muncul.
		if (PlayerPrefs.GetInt("PernahBaca", 0) == 0)
		{
			if (panelIntro != null)
			{
				panelIntro.SetActive(true);
			}
		}
		else
		{
			// Jika sudah pernah baca (nilainya 1), panel langsung dimatikan
			panelIntro.SetActive(false);
		}

		// 2. Hubungkan tombol ke fungsi ClosePanel
		if (btnMulai != null)
		{
			btnMulai.onClick.AddListener(ClosePanel);
		}
	}

	void ClosePanel()
	{
		panelIntro.SetActive(false);

		// TULIS CATATAN: Set 'PernahBaca' jadi 1 agar tidak muncul lagi selamanya
		PlayerPrefs.SetInt("PernahBaca", 1);
		PlayerPrefs.Save();

		Debug.Log("Game Dimulai: Misi M1 Aktif!");
	}
}