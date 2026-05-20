using UnityEngine;
using System.Collections.Generic;

public class NPCFarmer : MonoBehaviour
{
    [Header("Pengaturan Ekonomi")]
    public int pendapatanPerPohon = 100;
    private int hariTerakhirGajian = 0;

    [Header("Pengaturan Gerak (Patroli)")]
    public float speed = 1.2f;
    public float radiusPatroli = 4f; 
    public float waktuDiam = 3f;
    private Vector3 posisiAwal;
    private Vector3 targetTujuan;
    private float timerDiam;
    private bool sedangJalan = false;

    [Header("Referensi")]
    private UIManagerToko manager;
    private Animator anim;
    public GameObject dialogCanvas;
    public TMPro.TMP_Text txtDialog;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        posisiAwal = transform.position;
        
        if(dialogCanvas != null) dialogCanvas.SetActive(false);
        if (manager != null) hariTerakhirGajian = manager.hariKe;

        CariTujuanBaru();
    }

    void Update()
    {
        if (manager == null) return;

        // 1. LOGIKA GAJIAN (Harian)
        if (manager.hariKe > hariTerakhirGajian)
        {
            BerikanHasilPanen();
            hariTerakhirGajian = manager.hariKe;
        }

        // 2. LOGIKA GERAK PATROLI
        float jarakKeTujuan = Vector2.Distance(transform.position, targetTujuan);

        if (jarakKeTujuan > 0.1f)
        {
            sedangJalan = true;
            transform.position = Vector2.MoveTowards(transform.position, targetTujuan, speed * Time.deltaTime);

            // Update Animasi
            Vector2 arah = (targetTujuan - transform.position).normalized;
            UpdateAnimation(arah, true);
        }
        else
        {
            // Sudah sampai: Berhenti sejenak
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
        // Petani akan berkeliling secara acak di sekitar titik spawn-nya
        float acakX = Random.Range(-radiusPatroli, radiusPatroli);
        float acakY = Random.Range(-radiusPatroli, radiusPatroli);
        targetTujuan = posisiAwal + new Vector3(acakX, acakY, 0);
    }

    void BerikanHasilPanen()
    {
        int totalDuit = manager.jmlBerbuah * pendapatanPerPohon;
        if (totalDuit > 0)
        {
            manager.uangPemain += totalDuit;
            if(txtDialog != null) txtDialog.text = "Hasil panen hari ini: Rp " + totalDuit + "!";
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
        if (other.CompareTag("Player") && dialogCanvas != null) dialogCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null) dialogCanvas.SetActive(false);
    }
}