using UnityEngine;
using TMPro;
using System.Globalization; // Penting untuk format mata uang

public class EkonomiUI : MonoBehaviour
{
	public TextMeshProUGUI txtNilaiUang;
	private UIManagerToko uiManager;

	void Start()
	{
		uiManager = Object.FindFirstObjectByType<UIManagerToko>();
	}

	void Update()
	{
		if (uiManager != null && txtNilaiUang != null)
		{
			// Menggunakan format kebudayaan Indonesia (id-ID) untuk titik ribuan
			string uangFormatted = uiManager.uangPemain.ToString("N0", new CultureInfo("id-ID"));
			txtNilaiUang.text = "Rp " + uangFormatted;
		}
	}
}