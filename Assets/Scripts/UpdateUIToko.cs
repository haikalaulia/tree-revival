using UnityEngine;
using TMPro;

public class UpdateUIToko : MonoBehaviour
{
	private TombolBibit data;

	[Header("Tarik Objek Teks Dari Hierarchy Ke Sini")]
	public TMP_Text textNama;
	public TMP_Text textHarga;
	public TMP_Text textCO2;
	public TMP_Text textAir;

	void Start()
	{
		// Mencari script TombolBibit di Button_Beli (anak dari Bibit_Mahoni)
		data = GetComponentInChildren<TombolBibit>();

		if (data != null)
		{
			TampilkanData();
		}
		else
		{
			Debug.LogError("Script TombolBibit tidak ditemukan di anak objek!");
		}
	}

	void TampilkanData()
	{
		if (textNama) textNama.text = data.namaPohon;
		if (textHarga) textHarga.text = "Rp " + data.harga.ToString();

		// Ini baris CO2 yang sudah benar
		if (textCO2) textCO2.text = "CO2 : " + data.co2PerPohon.ToString("F1") + " kg";

		// TAMBAHKAN BARIS INI supaya Air muncul (mengambil data 1700 dari TombolBibit)
		if (textAir) textAir.text = "Air : " + data.airPerPohon.ToString() + " L";
	}
}