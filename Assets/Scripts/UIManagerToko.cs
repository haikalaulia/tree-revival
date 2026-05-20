using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class UIManagerToko : MonoBehaviour
{
	[Header("UI Panels")]
	public GameObject panelToko;
	public GameObject panelAnalisis;
	public GameObject panelDashboard;
	public TMP_Text[] daftarTeksAnggaran; // Untuk sinkronisasi semua teks uang

	[Header("Ekonomi & Stats")]
	public int uangPemain = 10000;
	public int jumlahPohon = 0;
	public int hariKe = 1;

	[Header("Tenaga Kerja (HUD)")]
	public int jmlPeneliti = 0;
	public int jmlPetani = 0;
	public int jmlPenjaga = 0;
	public int jmlPemandu = 0;

	public int maxPeneliti = 2;
	public int maxPetani = 3;
	public int maxPenjaga = 5;
	public int maxPemandu = 2;

	public TMP_Text txtPenelitiHUD;
	public TMP_Text txtPetaniHUD;
	public TMP_Text txtPenjagaHUD;
	public TMP_Text txtPemanduHUD;

	[Header("Kalkulasi Lingkungan")]
	public float totalCO2 = 0;
	public float totalAir = 0;
	public int totalLapanganKerja = 0;
	public int jmlBerbuah = 0;
	public int jmlPohonPenjaga = 0;
	public bool adaPeneliti = false;

	[Header("Sistem Luas Lahan")]
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

	void Update()
	{
		JalankanWaktu();
		CekKemampuanBeli();
		UpdatePopulasiHUD(); // Pastikan ini dipanggil agar HUD terupdate
	}

	void UpdatePopulasiHUD()
	{
		// Menampilkan angka "Sekarang / Maksimal" untuk masing-masing profesi
		if (txtPenelitiHUD != null) txtPenelitiHUD.text = "Peneliti: " + jmlPeneliti + " / " + maxPeneliti;
		if (txtPetaniHUD != null) txtPetaniHUD.text = "Petani: " + jmlPetani + " / " + maxPetani;
		if (txtPenjagaHUD != null) txtPenjagaHUD.text = "Penjaga: " + jmlPenjaga + " / " + maxPenjaga;
		if (txtPemanduHUD != null) txtPemanduHUD.text = "Pemandu: " + jmlPemandu + " / " + maxPemandu;
	}

	void CekKemampuanBeli()
	{
		// 1. Update semua teks anggaran agar nilainya sama di seluruh layar
		foreach (TMP_Text t in daftarTeksAnggaran)
		{
			if (t != null) t.text = "ANGGARAN: Rp " + uangPemain.ToString("N0");
		}

		// 2. Cek apakah tombol beli pohon bisa diklik atau tidak
		foreach (Button btn in tombolBibit)
		{
			if (btn == null) continue;
			TombolBibit info = btn.GetComponent<TombolBibit>();
			if (info != null) btn.interactable = (uangPemain >= info.harga);
		}
	}

	public float AmbilPersenPenjaga()
	{
		if (jumlahPohon == 0) return 0;
		// Menggunakan jmlPenjaga yang direkrut untuk Dashboard
		return ((float)jmlPenjaga / jumlahPohon) * 100f;
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

	// --- FUNGSI UI & SISTEM ---
	public void BukaTutupDashboard(bool status) { if (panelDashboard != null) panelDashboard.SetActive(status); }
	public void SetTanahAktif(SoilClick tanah) { tanahTerakhir = tanah; }
	public void BukaTokoTengah() { if (panelToko != null) panelToko.SetActive(true); }
	public void TutupToko() { if (panelToko != null) panelToko.SetActive(false); }

	void JalankanWaktu()
	{
		timerDetik += Time.deltaTime;
		if (timerDetik >= durasiSatuHari) { hariKe++; timerDetik = 0; }
	}

	void HitungLuasLahanOtomatis()
	{
		GameObject objekGround = GameObject.Find("ground");
		if (objekGround != null)
		{
			Tilemap tilemapGround = objekGround.GetComponent<Tilemap>();
			if (tilemapGround != null)
			{
				int jumlahTile = 0;
				tilemapGround.CompressBounds();
				BoundsInt bounds = tilemapGround.cellBounds;
				foreach (var pos in bounds.allPositionsWithin)
				{
					if (tilemapGround.HasTile(pos)) jumlahTile++;
				}
				float nilaiPerTile = 10000f / 3000f;
				luasLahanTotal = jumlahTile * nilaiPerTile;
				targetSerapanAir = luasLahanTotal * 2f;
			}
		}
	}

	// =========================================================================================
	// PERBAIKAN UTAMA: Proses Tanam dialihkan ke Sistem Jalan Otomatis Player Terlebih Dahulu
	// =========================================================================================
	public void ProsesTanam(TombolBibit infoTombol)
	{
		if (tanahTerakhir == null || infoTombol == null) { TutupSemuaMenu(); return; }
		if (uangPemain < infoTombol.harga) { TutupSemuaMenu(); return; }

		// Ambil referensi komponen tanah aktif saat ini
		SoilClick targetTanah = tanahTerakhir;

		// Sembunyikan panel toko dan analisis lebih awal agar transisi player berjalan terlihat bersih
		if (panelToko != null) panelToko.SetActive(false);
		if (panelAnalisis != null) panelAnalisis.SetActive(false);

		// Perintahkan pergerakan lewat jembatan SoilClick, logika penanaman dibungkus di dalam callback () => { ... }
		targetTanah.EksekusiJalanLaluTanam(() =>
		{
			SoilProperty dataTanah = targetTanah.dataTanah;
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
				// Panggil fungsi internal kawanmu untuk melakukan instansiasi objek bibit sukses
				TanamSuksesAtPos(infoTombol, targetTanah.posisiGridTanam);
			}
			else
			{
				uangPemain -= infoTombol.harga;
				if (infoTombol.prefabMati != null)
					Instantiate(infoTombol.prefabMati, targetTanah.posisiGridTanam, Quaternion.identity);
			}

			// Jalankan kelanjutan tutorial jika game kebetulan sedang berada dalam Scene Intro tutorial
			if (TutorialManager.Instance != null)
			{
				TutorialManager.Instance.SelesaiStep3();
			}
		});

		// Reset reference setelah perintah pergerakan dikirimkan
		tanahTerakhir = null;
	}

	public void TutupSemuaMenu()
	{
		if (panelToko != null) panelToko.SetActive(false);
		if (panelAnalisis != null) panelAnalisis.SetActive(false);
		tanahTerakhir = null;
	}

	// Helper function untuk menjamin posisi koordinat tanam dikunci secara presisi
	void TanamSuksesAtPos(TombolBibit info, Vector3 posisiTanam)
	{
		GameObject bibit = Instantiate(info.prefabBibit, posisiTanam, Quaternion.identity);

		jumlahPohon++;
		totalLapanganKerja += info.lapanganKerja;
		totalLuasTajuk += info.luasTajuk;

		// KALKULASI LINGKUNGAN (Hanya untuk pohon, bukan NPC)
		float multiplier = adaPeneliti ? 1.5f : 1f;
		// Sekarang CO2 dihitung dalam satuan KG murni, bukan TON
		totalCO2 += info.co2PerPohon * multiplier;
		totalAir += info.airPerPohon;

		if (info.jenisPohon == JenisPohon.PenjagaHutan)
		{
			jmlPohonPenjaga++; // Hitung pohon pelindung (Mahoni, Jati, dll)
		}
		else if (info.jenisPohon == JenisPohon.Berbuah)
		{
			jmlBerbuah++; // Hitung pohon buah (Mangga, Kelapa, dll)
		}

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		BibitPertumbuhan bp = bibit.GetComponent<BibitPertumbuhan>();
		if (bp == null) bp = bibit.AddComponent<BibitPertumbuhan>();
		bp.MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
	}

	// Fungsi cadangan lama (menghindari error referensi internal luar jika ada)
	void TanamSukses(TombolBibit info)
	{
		if (tanahTerakhir != null) TanamSuksesAtPos(info, tanahTerakhir.posisiGridTanam);
	}
}