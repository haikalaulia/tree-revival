using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class UIManagerToko : MonoBehaviour
{
	[Header("UI Panels")]
	public GameObject panelToko;
	public GameObject panelAnalisis;
	public GameObject panelDashboard;
	public TMP_Text[] daftarTeksAnggaran;

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
	public bool adaPeneliti = false;

	[Header("Sistem Luas Lahan (Dinamis: 3000 Tile = 1 Ha)")]
	public float luasLahanTotal = 0f;
	public float totalLuasTajuk = 0f;
	public float targetSerapanAir;

	[Header("Sistem Waktu")]
	public float durasiSatuHari = 60f;
	private float timerDetik;

	[Header("Daftar Tombol Toko")]
	public Button[] tombolBibit;

	private SoilClick tanahTerakhir;

	void Start()
	{
		HitungLuasLahanOtomatis();
	}

	void HitungLuasLahanOtomatis()
	{
		// Mencari objek bernama "ground" di Hierarchy
		GameObject objekGround = GameObject.Find("ground");
		if (objekGround != null)
		{
			Tilemap tilemapGround = objekGround.GetComponent<Tilemap>();
			if (tilemapGround != null)
			{
				int jumlahTile = 0;
				tilemapGround.CompressBounds();
				BoundsInt bounds = tilemapGround.cellBounds;

				// Menghitung jumlah tile yang terisi di Tilemap
				foreach (var pos in bounds.allPositionsWithin)
				{
					if (tilemapGround.HasTile(pos)) jumlahTile++;
				}

				// LOGIKA: 3000 Tile = 10.000 m2 (1 Hektar)
				// Maka 1 Tile = 10.000 / 3000 = 3.333333f
				float nilaiPerTile = 1000f / 3000f;

				// Total Luas akan otomatis menyesuaikan jumlah tile
				luasLahanTotal = jumlahTile * nilaiPerTile;

				// Target Air menyesuaikan luas lahan (misal 2 liter per m2)
				targetSerapanAir = luasLahanTotal * 2f;

				Debug.Log("<color=green><b>[SISTEM TILE DINAMIS]</b></color>");
				Debug.Log("Tile Terdeteksi: " + jumlahTile);

				// PERBAIKAN DI SINI: Mengubah .ToString("m2") menjadi .ToString("F1") + " m2"
				Debug.Log("Luas Lahan: " + (luasLahanTotal / 10000f).ToString("F2") + " Ha (" + luasLahanTotal.ToString("F1") + " m2)");
			}
			else
			{
				Debug.LogWarning("Objek 'ground' tidak punya komponen Tilemap!");
			}
		}
		else
		{
			Debug.LogWarning("Objek bernama 'ground' tidak ditemukan di scene!");
		}
	}

	void Update()
	{
		JalankanWaktu();
		CekKemampuanBeli();
	}

	public float AmbilPersenPenjaga()
	{
		if (jumlahPohon == 0) return 0;
		return ((float)jmlPenjagaHutan / jumlahPohon) * 100f;
	}

	public string AmbilStatusWilayah()
	{
		if (luasLahanTotal <= 0) return "DATA ERROR";

		float persen = (totalLuasTajuk / luasLahanTotal) * 100f;

		if (persen < 25 || AmbilPersenPenjaga() < 60) return "KRITIS";
		if (persen < 50) return "WASPADA";
		if (persen < 75) return "AMAN";
		return "OPTIMAL";
	}

	public void BukaTutupDashboard(bool status)
	{
		if (panelDashboard != null) panelDashboard.SetActive(status);
	}

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
		foreach (TMP_Text t in daftarTeksAnggaran)
		{
			if (t != null)
				t.text = "ANGGARAN: Rp " + uangPemain.ToString("N0");
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
		if (tanahTerakhir == null || infoTombol == null) { TutupSemuaMenu(); return; }
		if (uangPemain < infoTombol.harga) { TutupSemuaMenu(); return; }

		SoilProperty dataTanah = tanahTerakhir.dataTanah;
		if (dataTanah == null) return;

		string wilayahBibit = infoTombol.wilayahHarus.ToString().Replace("Rendah", " Rendah").Replace("Tinggi", " Tinggi").ToUpper().Trim();
		string wilayahTanah = dataTanah.namaWilayah.ToUpper().Trim();

		bool wilayahOk = (wilayahTanah == wilayahBibit);
		bool lembapOk = (dataTanah.kelembapan >= infoTombol.minLembap && dataTanah.kelembapan <= infoTombol.maxLembap);
		bool nutrisiOk = (dataTanah.nutrisi >= infoTombol.minNutrisi && dataTanah.nutrisi <= infoTombol.maxNutrisi);
		bool suhuOk = (dataTanah.suhu >= infoTombol.minSuhuDerajat && dataTanah.suhu <= infoTombol.maxSuhuDerajat);

		if (wilayahOk && lembapOk && nutrisiOk && suhuOk)
		{
			uangPemain -= infoTombol.harga;
			TanamSukses(infoTombol);
			Debug.Log("<color=green><b>[BERHASIL]</b></color> " + infoTombol.namaPohon + " tumbuh di kondisi yang sesuai.");
		}
		else
		{
			uangPemain -= infoTombol.harga;
			if (infoTombol.prefabMati != null)
			{
				Instantiate(infoTombol.prefabMati, tanahTerakhir.posisiGridTanam, Quaternion.identity);
			}

			Debug.Log("<color=red><b>[GAGAL]</b></color> " + infoTombol.namaPohon +
					  " | Wilayah: " + wilayahOk +
					  " | Lembap: " + lembapOk +
					  " | Nutrisi: " + nutrisiOk +
					  " | Suhu: " + suhuOk);
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

		jumlahPohon++;
		totalLapanganKerja += info.lapanganKerja;
		totalLuasTajuk += info.luasTajuk;

		if (info.jenisPohon == JenisPohon.PenjagaHutan)
		{
			jmlPenjagaHutan++;
			float multiplier = adaPeneliti ? 1.5f : 1f;
			totalCO2 += (info.co2PerPohon / 1000f) * multiplier;
			totalAir += info.airPerPohon;
		}
		else
		{
			jmlBerbuah++;
		}

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		BibitPertumbuhan bp = bibit.GetComponent<BibitPertumbuhan>();
		if (bp == null) bp = bibit.AddComponent<BibitPertumbuhan>();
		bp.MulaiTumbuh(info.prefabSedang, info.prefabDewasa);

		// ==========================================
		// ISI BARU: Hubungkan ke Sistem Tutorial
		// ==========================================
		if (TutorialManager.Instance != null)
		{
			TutorialManager.Instance.SelesaiStep2Dan3();
		}
	}
}