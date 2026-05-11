using UnityEngine;
using UnityEngine.UI; // Tambahkan ini agar bisa mengontrol tombol

public class UIManagerToko : MonoBehaviour
{
	[Header("UI Panels")]
	public GameObject panelToko;
	public GameObject panelAnalisis;

	[Header("Ekonomi & Stats")]
	public int uangPemain = 10000;
	public int jumlahPohon = 0;
	public int hariKe = 1;

	[Header("Sistem Waktu")]
	public float durasiSatuHari = 60f; // Uji coba 1 menit (Ganti 600f untuk 10 menit nanti)
	private float timerDetik;

	[Header("Daftar Tombol Toko")]
	public Button[] tombolBibit; // Masukkan semua tombol beli di sini lewat Inspector

	private SoilClick tanahTerakhir;

	void Update()
	{
		JalankanWaktu();
		CekKemampuanBeli();
	}

	void JalankanWaktu()
	{
		timerDetik += Time.deltaTime;
		if (timerDetik >= durasiSatuHari)
		{
			hariKe++;
			timerDetik = 0;
			Debug.Log("Hari berganti ke: " + hariKe);
		}
	}

	void CekKemampuanBeli()
	{
		// Meloop semua tombol yang didaftarkan, jika uang kurang, tombol jadi mati (abu-abu)
		foreach (Button btn in tombolBibit)
		{
			TombolBibit info = btn.GetComponent<TombolBibit>();
			if (info != null)
			{
				btn.interactable = (uangPemain >= info.harga);
			}
		}
	}

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
		GameObject bibit = Instantiate(info.prefabBibit, tanahTerakhir.posisiGridTanam, Quaternion.identity);

		jumlahPohon++; // Menambah jumlah pohon saat berhasil tanam

		GameObject folder = GameObject.Find("pohon");
		if (folder != null) bibit.transform.SetParent(folder.transform);

		BibitPertumbuhan bp = bibit.GetComponent<BibitPertumbuhan>();
		if (bp == null) bp = bibit.AddComponent<BibitPertumbuhan>();

		bp.MulaiTumbuh(info.prefabSedang, info.prefabDewasa);
	}
}