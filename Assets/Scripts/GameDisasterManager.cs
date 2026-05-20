using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameDisasterManager : MonoBehaviour
{
	[Header("Target Mitigasi Level 1")]
	public int deadlineHari = 10;
	public float syaratRasioPenjaga = 60f; 
	public float syaratRasioBerbuah = 40f;

    [Header("Sistem Kestabilan (6 Hari Gagal)")]
    public int batasHariGagal = 6;
    public int counterHariGagal = 0; // Berapa hari berturut-turut rasio salah
    private int hariTerakhirDicek = 1;

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
        hariTerakhirDicek = 1;
        counterHariGagal = 0;
	}

	void Update()
	{
		if (isGameOver || uiManager == null) return;

        // --- LOGIKA BARU: CEK SETIAP PERGANTIAN HARI ---
        if (uiManager.hariKe > hariTerakhirDicek)
        {
            CekKestabilanRasioHarian();
            hariTerakhirDicek = uiManager.hariKe;
        }

		// Notifikasi peringatan di hari ke-4
		if (uiManager.hariKe == 4 && !sudahNotifHari4)
		{
			StartCoroutine(TampilkanNotifPeringatan());
		}

		// CEK KEKALAHAN FINAL: Jika sudah melewati hari ke-10
		if (uiManager.hariKe > deadlineHari)
		{
			CekKondisiAkhir();
		}
	}

    void CekKestabilanRasioHarian()
    {
        int totalPohon = uiManager.jumlahPohon;
        
        // Abaikan pengecekan jika pohon masih 0 (awal game)
        if (totalPohon == 0) return;

        float rPenjaga = ((float)uiManager.jmlPohonPenjaga / totalPohon) * 100f;
        float rBuah = ((float)uiManager.jmlBerbuah / totalPohon) * 100f;

        // Cek apakah rasio TIDAK sesuai target
        if (rPenjaga < syaratRasioPenjaga || rBuah < syaratRasioBerbuah)
        {
            counterHariGagal++;
            Debug.Log("<color=orange>Peringatan:</color> Rasio tidak stabil! Hari gagal: " + counterHariGagal);
            
            // Jika sudah 6 hari berturut-turut salah, langsung Bencana!
            if (counterHariGagal >= batasHariGagal)
            {
                StartCoroutine(ProsesBencana("Ketidakseimbangan ekosistem selama " + batasHariGagal + " hari!", rPenjaga, rBuah));
            }
        }
        else
        {
            // Jika hari ini rasio benar, RESET counter ke 0
            counterHariGagal = 0;
            Debug.Log("<color=green>Rasio Stabil!</color> Counter hari gagal direset.");
        }
    }

	IEnumerator TampilkanNotifPeringatan()
	{
		sudahNotifHari4 = true;
		if (panelNotif && textNotifikasi)
		{
			textNotifikasi.text = "PERINGATAN: Jangan biarkan rasio pohon berantakan lebih dari 6 hari!";
			panelNotif.SetActive(true);
			yield return new WaitForSeconds(5f);
			panelNotif.SetActive(false);
		}
	}

	void CekKondisiAkhir()
	{
		isGameOver = true;
		int totalPohon = uiManager.jumlahPohon;

		float rPenjaga = (totalPohon > 0) ? ((float)uiManager.jmlPohonPenjaga / totalPohon) * 100f : 0f;
		float rBuah = (totalPohon > 0) ? ((float)uiManager.jmlBerbuah / totalPohon) * 100f : 0f;

        string status = uiManager.AmbilStatusWilayah();
        bool isAman = (status == "AMAN" || status == "OPTIMAL");

		if (!isAman || rPenjaga < syaratRasioPenjaga || rBuah < syaratRasioBerbuah)
		{
			StartCoroutine(ProsesBencana("Deadline Hari ke-10 tercapai.", rPenjaga, rBuah));
		}
		else
		{
            panelNotif.SetActive(true);
            textNotifikasi.text = "MISI BERHASIL! Wilayah stabil dan pulih.";
            Time.timeScale = 0f; 
		}
	}

	IEnumerator ProsesBencana(string alasan, float pPenjaga, float pBuah)
	{
		isGameOver = true;
        panelHitam.SetActive(true);
		
        textPesanBencana.text = "BENCANA DATANG!\n" + alasan + "\n\n" +
            "Rasio Penjaga: " + pPenjaga.ToString("F0") + "% (Butuh 60%)\n" +
            "Rasio Berbuah: " + pBuah.ToString("F0") + "% (Butuh 40%)";

		yield return new WaitForSeconds(5f);

        // Reset game
		if (folderPohon) foreach (Transform p in folderPohon) Destroy(p.gameObject);
		ResetStatsManager();
		if (player) player.position = Vector3.zero;

		panelHitam.SetActive(false);
		isGameOver = false;
		sudahNotifHari4 = false;
        counterHariGagal = 0; // Reset counter saat mulai ulang
	}

    void ResetStatsManager()
    {
        if (uiManager == null) return;
        uiManager.hariKe = 1;
        uiManager.totalCO2 = 0;
        uiManager.totalAir = 0;
        uiManager.totalLuasTajuk = 0;
        uiManager.jmlPohonPenjaga = 0;
        uiManager.jmlBerbuah = 0;
        uiManager.jumlahPohon = 0;
        uiManager.uangPemain = 10000; // Kembalikan modal awal
    }
}