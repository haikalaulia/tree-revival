using UnityEngine;
using System.Collections.Generic;

public class NPCResearcher : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float speed = 1.5f;
    public float radiusPatroli = 5f; // Jarak keliling dari titik awal
    public float waktuDiam = 4f;    // Berapa lama dia berhenti saat "meneliti"

    private Vector3 posisiAwal;
    private Vector3 targetTujuan;
    private float timerDiam;
    private bool sedangJalan = false;

    [Header("Referensi")]
    public UIManagerToko globalManager;
    public GameObject dialogCanvas;
    private Animator anim;

    void Start()
    {
        globalManager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        posisiAwal = transform.position;
        
        if(dialogCanvas != null) dialogCanvas.SetActive(false);

        // Langsung aktifkan bonus CO2 saat muncul
        if(globalManager != null) globalManager.adaPeneliti = true;

        CariTujuanBaru();
    }

    void Update()
    {
        float jarakKeTujuan = Vector2.Distance(transform.position, targetTujuan);

        if (jarakKeTujuan > 0.1f)
        {
            // SEDANG BERJALAN KE TUJUAN
            sedangJalan = true;
            transform.position = Vector2.MoveTowards(transform.position, targetTujuan, speed * Time.deltaTime);

            // Update Animasi (4 Arah)
            Vector2 arah = (targetTujuan - transform.position).normalized;
            UpdateAnimation(arah, true);
        }
        else
        {
            // SUDAH SAMPAI: BERHENTI UNTUK MENELITI
            sedangJalan = false;
            UpdateAnimation(Vector2.zero, false);

            timerDiam += Time.deltaTime;
            if (timerDiam >= waktuDiam)
            {
                CariTujuanBaru();
                timerDiam = 0;
            }
        }
    }

    void CariTujuanBaru()
    {
        // 50% Peluang ke pohon, 50% Peluang ke lahan kosong acak
        int pilihan = Random.Range(0, 2);
        GameObject folderPohon = GameObject.Find("pohon");

        if (pilihan == 0 && folderPohon != null && folderPohon.transform.childCount > 0)
        {
            // TUJUAN: Pilih salah satu pohon secara acak untuk diperiksa
            int indeksPohon = Random.Range(0, folderPohon.transform.childCount);
            targetTujuan = folderPohon.transform.GetChild(indeksPohon).position;
            Debug.Log("Peneliti: Memeriksa kesehatan pohon...");
        }
        else
        {
            // TUJUAN: Pilih koordinat acak di sekitar lokasi awal (lahan kosong)
            float acakX = Random.Range(-radiusPatroli, radiusPatroli);
            float acakY = Random.Range(-radiusPatroli, radiusPatroli);
            targetTujuan = posisiAwal + new Vector3(acakX, acakY, 0);
            Debug.Log("Peneliti: Mengambil sampel tanah...");
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

    // Fungsi Dialog
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null) dialogCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null) dialogCanvas.SetActive(false);
    }
}