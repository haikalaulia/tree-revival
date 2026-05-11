using UnityEngine;
using TMPro;
using System.Globalization; // Penting untuk format mata uang

public class EkonomiUI : MonoBehaviour
{
	[Header("Slot Teks HUD")]
	public TextMeshProUGUI txtNilaiUang;
	public TextMeshProUGUI txtCounterPohon; // Tambahan untuk jumlah pohon
	public TextMeshProUGUI txtHari;         // Tambahan untuk hari

	private UIManagerToko uiManager;

	void Start()
	{
		uiManager = Object.FindFirstObjectByType<UIManagerToko>();
	}

	void Update()
	{
		if (uiManager == null) return;

		// 1. Update Format Rupiah
		if (txtNilaiUang != null)
		{
			string uangFormatted = uiManager.uangPemain.ToString("N0", new CultureInfo("id-ID"));
			txtNilaiUang.text = "Rp " + uangFormatted;
		}

		// 2. Update Counter Pohon (Contoh: POHON: 12)
		if (txtCounterPohon != null)
		{
			txtCounterPohon.text = "POHON: " + uiManager.jumlahPohon;
		}

		// 3. Update Teks Hari (Contoh: HARI 1)
		if (txtHari != null)
		{
			txtHari.text = "HARI " + uiManager.hariKe;
		}
	}
}