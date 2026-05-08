using UnityEngine;

public enum TipeWilayah { DataranRendah, DataranTinggi, Pesisir }

public class TombolBibit : MonoBehaviour
{
	[Header("Identitas & Ekonomi")]
	public string namaPohon;
	public int harga;

	[Header("Syarat Tumbuh")]
	public TipeWilayah wilayahHarus;
	public float minLembap = 0;
	public float maxLembap = 100;
	public float minNutrisi = 0;
	public float maxNutrisi = 100;

	[Header("Syarat Suhu (Input Derajat)")]
	public float minSuhuDerajat = 20; // Contoh: 20 derajat (Dingin)
	public float maxSuhuDerajat = 30; // Contoh: 30 derajat (Sedang)

	[Header("Visual Tahapan")]
	public GameObject prefabBibit;
	public GameObject prefabSedang;
	public GameObject prefabDewasa;
	public GameObject prefabMati;

	// Fungsi konversi angka ke string kategori agar sama dengan SoilProperty
	public string AmbilKategoriDariAngka(float derajat)
	{
		if (derajat <= 22) return "DINGIN";
		if (derajat >= 23 && derajat <= 30) return "SEDANG";
		return "PANAS";
	}

	public void KlikBeli()
	{
		UIManagerToko uiManager = Object.FindFirstObjectByType<UIManagerToko>();
		if (uiManager != null) uiManager.ProsesTanam(this);
	}
}