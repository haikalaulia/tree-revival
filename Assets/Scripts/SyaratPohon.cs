using UnityEngine;

[System.Serializable]
public class SyaratPohon
{
	public string namaPohon;
	public float minKelembapan;
	public float minNutrisi;
	public string wilayahCocok; // Akan diadu dengan SoilProperty.namaWilayah
	public string suhuCocok;    // Akan diadu dengan SoilProperty.statusSuhu

	[Header("Prefabs")]
	public GameObject prefabBibit; // Objek kecil
	public GameObject prefabPohonDewasa; // Objek besar
}