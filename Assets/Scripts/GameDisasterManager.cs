using UnityEngine;
using TMPro;
using System.Collections;

public class GameDisasterManager : MonoBehaviour
{
	[Header("Target Mitigasi")]
	public int deadlineHari = 10;
	public int targetJumlahPohon = 15;
	public float targetAir = 13500f;
	public float targetCO2 = 100f;
	public int targetPenjaga = 9;
	public int targetBuah = 6;

	[Header("UI Referensi")]
	public GameObject panelHitam;
	public TextMeshProUGUI textPesanBencana;
	public GameObject panelNotif;
	public TextMeshProUGUI textNotifikasi;

	[Header("Referensi Lain")]
	public Transform folderPohon;
	public Transform player;
	private UIManagerToko uiManager;

	private bool isGameOver = false;
	private bool sudahNotifHari4 = false;

	void Start()
	{
		uiManager = Object.FindFirstObjectByType<UIManagerToko>();
		if (panelHitam) panelHitam.SetActive(false);
		if (panelNotif) panelNotif.SetActive(false);
	}

	void Update()
	{
		if (isGameOver || uiManager == null) return;

		if (uiManager.hariKe == 4 && !sudahNotifHari4)
		{
			StartCoroutine(TampilkanNotifHari4());
		}

		if (uiManager.hariKe > deadlineHari)
		{
			CekKondisiAkhir();
		}
	}

	IEnumerator TampilkanNotifHari4()
	{
		sudahNotifHari4 = true;
		if (panelNotif && textNotifikasi)
		{
			textNotifikasi.text = "PERINGATAN: Lahan kritis! Pastikan target mitigasi (Air, CO2, & Rasio) tercapai sebelum hari ke-10!";
			panelNotif.SetActive(true);
			yield return new WaitForSeconds(5f);
			panelNotif.SetActive(false);
		}
	}

	void CekKondisiAkhir()
	{
		isGameOver = true;
		int jumlahPohon = (folderPohon) ? folderPohon.childCount : 0;

		float pPenjaga = uiManager.AmbilPersenPenjaga();
		int hitungPenjaga = Mathf.RoundToInt((pPenjaga / 100f) * jumlahPohon);
		int hitungBuah = jumlahPohon - hitungPenjaga;

		if (jumlahPohon < targetJumlahPohon || uiManager.totalAir < targetAir ||
			uiManager.totalCO2 < targetCO2 || hitungPenjaga < targetPenjaga || hitungBuah < targetBuah)
		{
			StartCoroutine(ProsesBencana());
		}
		else
		{
			isGameOver = false;
		}
	}

	IEnumerator ProsesBencana()
	{
		panelHitam.SetActive(true);
		textPesanBencana.text = "BENCANA DATANG! Mitigasi gagal.";
		yield return new WaitForSeconds(5f);

		if (folderPohon) foreach (Transform p in folderPohon) Destroy(p.gameObject);
		if (uiManager) uiManager.hariKe = 1;
		if (player) player.position = Vector3.zero;

		panelHitam.SetActive(false);
		isGameOver = false;
		sudahNotifHari4 = false;
	}
}