using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagerToko : MonoBehaviour
{
	[Header("UI Panels")]
	public GameObject panelToko;
	public GameObject panelAnalisis;
	public GameObject panelDashboard; // Slot untuk Panel Baru Fase 5
	public TMP_Text txtAnggaranDiToko;

	[Header("Ekonomi & Stats")]
	public int uangPemain = 10000;
	public int jumlahPohon = 0;
	public int hariKe = 1;

	[Header("Kalkulasi Lingkungan (Fase 5)")]
	public float totalCO2 = 0;
	public float totalAir = 0;
	public int totalLapanganKerja = 0;
	public int jmlPenjagaHutan = 0;
	public int jmlBerbuah = 0;
	public int targetTutupanHutan = 50; // Target pohon untuk 100%

	[Header("Sistem Waktu")]
	public float durasiSatuHari = 60f;
	private float timerDetik;

	[Header("Daftar Tombol Toko")]
	public Button[] tombolBibit;

	private SoilClick tanahTerakhir;

	void Update()
	{
		JalankanWaktu();
		CekKemampuanBeli();
	}

	// --- FUNGSI TAMBAHAN FASE 5 ---
	public float AmbilPersenPenjaga()
	{
		if (jumlahPohon == 0) return 0;
		return ((float)jmlPenjagaHutan / jumlahPohon) * 100f;
	}

	public string AmbilStatusWilayah()
	{
		float persen = ((float)jumlahPohon / targetTutupanHutan) * 100f;
		if (persen < 25 || AmbilPersenPenjaga() < 60) return "KRITIS";
		if (persen < 50) return "WASPADA";
		if (persen < 75) return "AMAN";
		return "OPTIMAL";
	}

	public void BukaTutupDashboard(bool status)
	{
		if (panelDashboard != null) panelDashboard.SetActive(status);
	}
	// ------------------------------

	void JalankanWaktu()
	{
		timerDetik += Time.deltaTime;
		if (timerDetik >= durasiSatuHari)
		{
			hariKe++;
			timerDetik = 0;
		}
	}

	void CekKemampuanBeli()
	{
		foreach (Button btn in tombolBibit)
		{
			TombolBibit info = btn.GetComponent<TombolBibit>();
			if (info != null) btn.interactable = (uangPemain >= info.harga);
		}

		if(txtAnggaranDiToko != null) 
    	{
        txtAnggaranDiToko.text = "ANGGARAN: Rp " + uangPemain.ToString("N0");
    	}

		foreach (Button btn in tombolBibit)
    	{
        TombolBibit info = btn.GetComponent<TombolBibit>();
        if (info != null) btn.interactable = (uangPemain >= info.harga);
    	}
	}

	public void SetTanahAktif(SoilClick tanah) { tanahTerakhir = tanah; }
	public void BukaTokoTengah() { if (panelToko != null) panelToko.SetActive(true); }
	public void TutupToko() { if (panelToko != null) panelToko.SetActive(false); }

	public void ProsesTanam(TombolBibit infoTombol)
	{
		if (tanahTerakhir == null) { TutupSemuaMenu(); return; }
		if (uangPemain < infoTombol.harga) { TutupSemuaMenu(); return; }

		SoilProperty dataTanah = tanahTerakhir.dataTanah;
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
				Instantiate(infoTombol.prefabMati, tanahTerakhir.posisiGridTanam, Quaternion.identity);
		}
		TutupSemuaMenu();
	}

	public void TutupSemuaMenu()
	{
		if (panelToko != null) panelToko.SetActive(false);
		if (panelAnalisis != null) panelAnalisis.SetActive(false);
		tanahTerakhir = null;
	}

	void TanamSukses(TombolBibit info)
	{
		GameObject bibit = Instantiate(info.prefabBibit, tanahTerakhir.posisiGridTanam, Quaternion.identity);

		// --- UPDATE KALKULASI FASE 5 ---
		jumlahPohon++;
		totalLapanganKerja += info.lapanganKerja;

		if (info.jenisPohon == JenisPohon.PenjagaHutan)
		{
			jmlPenjagaHutan++;
			totalCO2 += info.co2PerPohon;
			totalAir += info.airPerPohon;
		}
		else
		{
			jmlBerbuah++;
		}
		// ------------------------------

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		BibitPertumbuhan bp = bibit.GetComponent<BibitPertumbuhan>();
		if (bp == null) bp = bibit.AddComponent<BibitPertumbuhan>();
		bp.MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
	}
}