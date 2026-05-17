using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
	[Header("UI Reference")]
	public GameObject panelIntro;
	public Button btnMulai;

	void Start()
	{
		// 1. Cek apakah di PlayerPrefs sudah ada tanda 'PernahBaca'?
		if (PlayerPrefs.GetInt("PernahBaca", 0) == 0)
		{
			// Jika BELUM PERNAH (0), munculkan panel intro di awal
			if (panelIntro != null)
			{
				panelIntro.SetActive(true);
			}
		}
		else
		{
			// Jika SUDAH PERNAH (1), panel intro langsung dimatikan
			if (panelIntro != null)
			{
				panelIntro.SetActive(false);
			}

			// PERBAIKAN: Baris pemicu TutorialManager.Instance.MulaiStep1() di sini SUDAH DIHAPUS.
			// Dengan begitu, saat game dimainkan ulang, panah tidak akan muncul lagi secara otomatis.
		}

		// 2. Hubungkan tombol ke fungsi ClosePanel
		if (btnMulai != null)
		{
			btnMulai.onClick.AddListener(ClosePanel);
		}
	}

	void ClosePanel()
	{
		if (panelIntro != null)
		{
			panelIntro.SetActive(false);
		}

		// HUBUNGKAN KE TUTORIAL: Pemicu Langkah Pertama (Hanya berjalan saat tombol diklik pertama kali)
		if (TutorialManager.Instance != null)
		{
			TutorialManager.Instance.MulaiStep1();
		}

		// Set 'PernahBaca' jadi 1 agar panel intro & tutorial tidak memicu lagi di kemudian hari
		PlayerPrefs.SetInt("PernahBaca", 1);
		PlayerPrefs.Save();

		Debug.Log("Game Dimulai: Misi M1 Aktif & Tutorial Dimulai!");
	}
}