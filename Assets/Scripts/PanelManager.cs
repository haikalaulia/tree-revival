using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelManager : MonoBehaviour
{
	public GameObject panelObj;
	public Slider sliderLembap;
	public Slider sliderNutrisi;

	public TextMeshProUGUI txtPersenLembap;
	public TextMeshProUGUI txtPersenNutrisi;
	public TextMeshProUGUI txtSuhu;

	public void TampilkanPanel(SoilProperty data)
	{
		panelObj.SetActive(true);

		sliderLembap.value = data.kelembapan / 100f;
		sliderNutrisi.value = data.nutrisi / 100f;

		txtPersenLembap.text = data.kelembapan + "%";
		txtPersenNutrisi.text = data.nutrisi + "%";

		// Ini tidak akan error lagi karena sudah dihandle di SoilProperty
		txtSuhu.text = data.statusSuhu;
		txtSuhu.color = data.warnaSuhu;
	}

	public void TutupPanel()
	{
		panelObj.SetActive(false);

		// Memberitahu Manager untuk melepas referensi tanah
		UIManagerToko uiToko = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null) uiToko.TutupSemuaMenu();
	}
}