using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class UIManagerToko : MonoBehaviour
{
	[Header("UI Panels")]
	public GameObject panelToko;
	public GameObject panelAnalisis;
	public GameObject panelDashboard; // Slot untuk Panel Baru Fase 5
	public TMP_Text[] daftarTeksAnggaran;

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
		// JALANKAN HITUNGAN BERDASARKAN RUMUS 3000 TILE = 1 HEKTAR
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
				float nilaiPerTile = 10000f / 3000f;

				// Total Luas akan otomatis menyesuaikan jumlah tile
				luasLahanTotal = jumlahTile * nilaiPerTile;

				// Target Air menyesuaikan luas lahan (misal 2 liter per m2)
				targetSerapanAir = luasLahanTotal * 2f;

				Debug.Log("<color=green><b>[SISTEM TILE DINAMIS]</b></color>");
				Debug.Log("Tile Terdeteksi: " + jumlahTile);
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
        // Gunakan jmlPenjaga yang dari HUD tadi
        return ((float)jmlPenjaga / jumlahPohon) * 100f; 
    }

	public string AmbilStatusWilayah()
	{
		if (luasLahanTotal <= 0) return "DATA ERROR";

		// Status Berdasarkan Persentase Tutupan Hutan (Luas Tajuk / Luas Lahan)
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

		// 1. Sinkronisasi Nama Wilayah (Biar gak sensitif spasi/huruf besar kecil)
		string wilayahBibit = infoTombol.wilayahHarus.ToString().Replace("Rendah", " Rendah").Replace("Tinggi", " Tinggi").ToUpper().Trim();
		string wilayahTanah = dataTanah.namaWilayah.ToUpper().Trim();

		// 2. Pengecekan Syarat Tumbuh (Menggunakan Range Min & Max)
		bool wilayahOk = (wilayahTanah == wilayahBibit);
		bool lembapOk = (dataTanah.kelembapan >= infoTombol.minLembap && dataTanah.kelembapan <= infoTombol.maxLembap);
		bool nutrisiOk = (dataTanah.nutrisi >= infoTombol.minNutrisi && dataTanah.nutrisi <= infoTombol.maxNutrisi);

		// Perbaikan: Sudah menggunakan nama variabel 'suhu' sesuai permintaanmu
		bool suhuOk = (dataTanah.suhu >= infoTombol.minSuhuDerajat && dataTanah.suhu <= infoTombol.maxSuhuDerajat);

		if (wilayahOk && lembapOk && nutrisiOk && suhuOk)
		{
			uangPemain -= infoTombol.harga;
			TanamSukses(infoTombol);
			Debug.Log("<color=green><b>[BERHASIL]</b></color> " + infoTombol.namaPohon + " tumbuh di kondisi yang sesuai.");
		}
		else
		{
			// Gagal tumbuh: Uang tetap berkurang, muncul prefab mati
			uangPemain -= infoTombol.harga;
			if (infoTombol.prefabMati != null)
			{
				Instantiate(infoTombol.prefabMati, tanahTerakhir.posisiGridTanam, Quaternion.identity);
			}

			// Cek log ini di Console Unity untuk tahu syarat mana yang tidak terpenuhi (False)
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
    // 1. Munculkan bibit pohon di lokasi tanah yang diklik
    GameObject bibit = Instantiate(info.prefabBibit, tanahTerakhir.posisiGridTanam, Quaternion.identity);

    // 2. Update statistik hutan
    jumlahPohon++;
    totalLapanganKerja += info.lapanganKerja;
    totalLuasTajuk += info.luasTajuk;

		if (info.jenisPohon == JenisPohon.PenjagaHutan)
		{
			jmlPenjagaHutan++;
			totalCO2 += (info.co2PerPohon / 1000f); // Konversi KG ke Ton
			// Jika ada peneliti, bonus serapan CO2 jadi 1.5x lebih besar (150%)
        	float multiplier = adaPeneliti ? 1.5f : 1f; 
        	totalCO2 += (info.co2PerPohon / 1000f) * multiplier; 
			totalAir += info.airPerPohon;
		}
		else
		{
			jmlBerbuah++;
		}

    // 5. Merapikan Hierarchy (masukkan pohon ke dalam object 'pohon')
    GameObject folder = GameObject.Find("pohon");
    if (folder != null) bibit.transform.SetParent(folder.transform);

    // 6. Jalankan sistem pertumbuhan pohon
    BibitPertumbuhan bp = bibit.GetComponent<BibitPertumbuhan>();
    if (bp == null) bp = bibit.AddComponent<BibitPertumbuhan>();
    bp.MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
    }
}