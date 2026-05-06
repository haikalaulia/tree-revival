using UnityEngine;

public class SoilClick : MonoBehaviour
{
	public SoilProperty dataTanah;

	private void OnMouseDown()
	{
		// Mencari PanelManager dan menyuruhnya tampil
		FindFirstObjectByType<PanelManager>().TampilkanPanel(dataTanah);
	}
}