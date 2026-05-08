using UnityEngine;

public class UIManagerToko : MonoBehaviour
{
	public GameObject panelToko;
	public int uangPemain = 1000;
	private SoilClick tanahTerakhir;

	public void SetTanahAktif(SoilClick tanah)
	{
		tanahTerakhir = tanah;
		Debug.Log("Target tanah diupdate ke: " + tanah.name);
	}

	public void BukaTokoTengah()
	{
		if (panelToko != null) panelToko.SetActive(true);
	}

	public void TutupToko()
	{
		if (panelToko != null) panelToko.SetActive(false);
	}

	public void ProsesTanam(TombolBibit infoTombol)
	{
		// Jika tidak ada tanah yang dipilih atau tanahnya sudah ada pohon, batalkan
		if (tanahTerakhir == null || tanahTerakhir.sudahDitanami)
		{
			TutupToko();
			return;
		}

		if (uangPemain < infoTombol.harga)
		{
			Debug.Log("Uang tidak cukup!");
			TutupToko();
			return;
		}

		SoilProperty dataTanah = tanahTerakhir.dataTanah;

		// Syarat Suhu
		string kategoriTanah = dataTanah.statusSuhu;
		string katMinBibit = infoTombol.AmbilKategoriDariAngka(infoTombol.minSuhuDerajat);
		string katMaxBibit = infoTombol.AmbilKategoriDariAngka(infoTombol.maxSuhuDerajat);

		// Syarat Lainnya
		string wilayahBibit = infoTombol.wilayahHarus.ToString().Replace("Rendah", " Rendah").Replace("Tinggi", " Tinggi").ToUpper();
		bool wilayahOk = (dataTanah.namaWilayah.Trim().ToUpper() == wilayahBibit);
		bool lembapOk = (dataTanah.kelembapan >= infoTombol.minLembap && dataTanah.kelembapan <= infoTombol.maxLembap);
		bool nutrisiOk = (dataTanah.nutrisi >= infoTombol.minNutrisi && dataTanah.nutrisi <= infoTombol.maxNutrisi);
		bool suhuOk = (kategoriTanah == katMinBibit || kategoriTanah == katMaxBibit);

		if (wilayahOk && lembapOk && nutrisiOk && suhuOk)
		{
			uangPemain -= infoTombol.harga;
			TanamSukses(infoTombol);
		}
		else
		{
			uangPemain -= infoTombol.harga;
			if (infoTombol.prefabMati != null)
				Instantiate(infoTombol.prefabMati, tanahTerakhir.transform.position, Quaternion.identity);

			tanahTerakhir.sudahDitanami = true; // Anggap gagal & petak terisi prefab mati
		}

		TutupToko();
		tanahTerakhir = null; // RESET: Agar klik grid selanjutnya bisa terbaca
	}

	void TanamSukses(TombolBibit info)
	{
		GameObject bibit = Instantiate(info.prefabBibit, tanahTerakhir.transform.position, Quaternion.identity);

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		bibit.AddComponent<BibitPertumbuhan>().MulaiTumbuh(info.prefabSedang, info.prefabDewasa);

		tanahTerakhir.sudahDitanami = true;
	}
}