using UnityEngine;

public class UIManagerToko : MonoBehaviour
{
	public GameObject panelToko;
	public GameObject panelAnalisis;
	public int uangPemain = 10000;
	private SoilClick tanahTerakhir;

	public void SetTanahAktif(SoilClick tanah)
	{
		tanahTerakhir = tanah;
		Debug.Log("Grid aktif diupdate.");
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
		if (tanahTerakhir == null)
		{
			TutupSemuaMenu();
			return;
		}

		if (uangPemain < infoTombol.harga)
		{
			Debug.Log("Uang tidak cukup!");
			TutupSemuaMenu();
			return;
		}

		SoilProperty dataTanah = tanahTerakhir.dataTanah;

		// Logika Syarat (Sesuai kode asli kamu)
		string kategoriTanah = dataTanah.statusSuhu;
		string katMinBibit = infoTombol.AmbilKategoriDariAngka(infoTombol.minSuhuDerajat);
		string katMaxBibit = infoTombol.AmbilKategoriDariAngka(infoTombol.maxSuhuDerajat);
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
			{
				// Tanam di koordinat grid yang pas
				Instantiate(infoTombol.prefabMati, tanahTerakhir.posisiGridTanam, Quaternion.identity);
			}
		}

		TutupSemuaMenu();
	}

	public void TutupSemuaMenu()
	{
		if (panelToko != null) panelToko.SetActive(false);
		if (panelAnalisis != null) panelAnalisis.SetActive(false);
		tanahTerakhir = null;
		Debug.Log("Sistem Reset. Siap klik grid lain.");
	}

	void TanamSukses(TombolBibit info)
	{
		// Menggunakan posisiGridTanam agar pohon pas di tengah kotak grid
		GameObject bibit = Instantiate(info.prefabBibit, tanahTerakhir.posisiGridTanam, Quaternion.identity);

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		if (bibit.GetComponent<BibitPertumbuhan>() == null)
			bibit.AddComponent<BibitPertumbuhan>().MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
		else
			bibit.GetComponent<BibitPertumbuhan>().MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
	}
}