using UnityEngine;

public enum TipeWilayah { DataranRendah, DataranTinggi, Pesisir }
public enum JenisPohon { PenjagaHutan, Berbuah }

public class TombolBibit : MonoBehaviour
{
	[Header("Identitas & Ekonomi")]
	public string namaPohon;
	public int harga;
	public JenisPohon jenisPohon;

	[Header("Dampak Lingkungan (Fase 5)")]
	public float luasTajuk = 25.0f;  // TAMBAHAN: Luas tajuk dalam m2
	public float co2PerPohon = 25.5f;
	public float airPerPohon = 10.0f;
	public int lapanganKerja = 1;

	[Header("Syarat Tumbuh")]
	public TipeWilayah wilayahHarus;
	public float minLembap = 0;
	public float maxLembap = 100;
	public float minNutrisi = 0;
	public float maxNutrisi = 100;

	[Header("Syarat Suhu (Input Derajat)")]
	public float minSuhuDerajat = 20;
	public float maxSuhuDerajat = 30;

	[Header("Visual Tahapan")]
	public GameObject prefabBibit;
	public GameObject prefabSedang;
	public GameObject prefabDewasa;
	public GameObject prefabMati;

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