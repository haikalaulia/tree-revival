using UnityEngine;
using UnityEngine.UI;
using TMPro; // Pastikan ini ada untuk mengakses TextMeshPro

public class PanelManager : MonoBehaviour
{
	public GameObject panelObj;
	public Slider sliderLembap;
	public Slider sliderNutrisi;

	// Variabel baru untuk teks persentase
	public TextMeshProUGUI txtPersenLembap;
	public TextMeshProUGUI txtPersenNutrisi;

	public TextMeshProUGUI txtSuhu;

	public void TampilkanPanel(SoilProperty data)
	{
		panelObj.SetActive(true);

		// Update Slider (pembagian 100f karena slider menggunakan rentang 0 sampai 1)
		sliderLembap.value = data.kelembapan / 100f;
		sliderNutrisi.value = data.nutrisi / 100f;

		// Update Teks Persentase sesuai desain Figma kamu
		txtPersenLembap.text = data.kelembapan + "%";
		txtPersenNutrisi.text = data.nutrisi + "%";

		// Update Status Suhu
		txtSuhu.text = data.statusSuhu;
		txtSuhu.color = data.warnaSuhu;
	}

	public void TutupPanel() { panelObj.SetActive(false); }
}