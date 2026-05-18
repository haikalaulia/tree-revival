using UnityEngine;
using System.Collections.Generic; // Penting untuk mencari daftar pohon

public class NPCTourGuide : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float speed = 1.5f;
    public float waktuMenjelaskan = 5f; // Berapa lama berhenti di satu pohon
    private Vector3 targetTujuan;
    private float timerDiam;
    private bool sedangJalan = false;

    [Header("Ekonomi & Edukasi")]
    public int pendapatanWisata = 500; // Gaji harian tetap
    public int bonusTipping = 50;      // Bonus setiap kunjungan pohon
    public string[] tipsEkowisata;    // Daftar kalimat fakta lingkungan
    public TMPro.TMP_Text txtDialog;  // Seret objek Text (TMP) gelembung bicara ke sini

    [Header("Referensi")]
    public UIManagerToko manager;
    public GameObject dialogCanvas;
    private Animator anim;
    private int hariTerakhirKerja = 0;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        
        if(dialogCanvas != null) dialogCanvas.SetActive(false);
        hariTerakhirKerja = manager.hariKe;

        // Mulai mencari pohon pertama untuk dikunjungi
        CariPohonDewasa();
    }

    void Update()
    {
        if (manager == null) return;

        // --- A. LOGIKA GAJI HARIAN (LAMA) ---
        if (manager.hariKe > hariTerakhirKerja)
        {
            BerikanUangWisata();
            hariTerakhirKerja = manager.hariKe;
        }

        // --- B. LOGIKA PATROLI & TIPPING (BARU) ---
        float jarak = Vector2.Distance(transform.position, targetTujuan);

        if (jarak > 0.5f)
        {
            // SEDANG MENUJU POHON
            sedangJalan = true;
            transform.position = Vector2.MoveTowards(transform.position, targetTujuan, speed * Time.deltaTime);
            
            // Update Animasi 4 Arah
            Vector2 arah = (targetTujuan - transform.position).normalized;
            UpdateAnimation(arah, true);
        }
        else
        {
            // SUDAH SAMPAI: BERHENTI MENJELASKAN (IDLE)
            sedangJalan = false;
            UpdateAnimation(Vector2.zero, false);

            timerDiam += Time.deltaTime;
            if (timerDiam >= waktuMenjelaskan)
            {
                // BERI BONUS TIPPING
                manager.uangPemain += bonusTipping;
                Debug.Log("Pemandu: Turis memberikan tip Rp " + bonusTipping);
                
                // Cari pohon tujuan selanjutnya secara acak
                CariPohonDewasa();
                timerDiam = 0;
            }
        }
    }

    void CariPohonDewasa()
    {
        GameObject folderPohon = GameObject.Find("pohon");
        if (folderPohon == null || folderPohon.transform.childCount == 0) 
        {
            // Jika tidak ada pohon, dia akan diam di posisinya sekarang
            targetTujuan = transform.position;
            return;
        }

        List<Transform> daftarDewasa = new List<Transform>();
        
        // Cari pohon yang sudah punya status "sudahDewasa"
        foreach (Transform p in folderPohon.transform)
        {
            BibitPertumbuhan bp = p.GetComponent<BibitPertumbuhan>();
            if (bp != null && bp.sudahDewasa)
            {
                daftarDewasa.Add(p);
            }
        }

        // Jika ketemu pohon dewasa, pilih satu secara acak
        if (daftarDewasa.Count > 0)
        {
            int indeksAcak = Random.Range(0, daftarDewasa.Count);
            targetTujuan = daftarDewasa[indeksAcak].position;
        }
        else
        {
            // Jika belum ada pohon yang dewasa, tunggu di tempat
            targetTujuan = transform.position;
        }
    }

    void BerikanUangWisata()
    {
        string status = manager.AmbilStatusWilayah();
        if (status == "AMAN" || status == "OPTIMAL")
        {
            manager.uangPemain += pendapatanWisata;
            Debug.Log("Pemandu: Pendapatan harian turis masuk: Rp " + pendapatanWisata);
        }
    }

    void UpdateAnimation(Vector2 arah, bool isWalking)
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isWalking);
            if (isWalking)
            {
                anim.SetFloat("moveX", arah.x);
                anim.SetFloat("moveY", arah.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null)
        {
            dialogCanvas.SetActive(true);
            
            // GANTI TEKS DIALOG DENGAN TIPS EDUKASI ACAK
            if (tipsEkowisata.Length > 0 && txtDialog != null)
            {
                int indeksAcak = Random.Range(0, tipsEkowisata.Length);
                txtDialog.text = tipsEkowisata[indeksAcak];
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null)
        {
            dialogCanvas.SetActive(false);
        }
    }
}