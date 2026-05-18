using UnityEngine;
using System.Collections.Generic;

public class NPCWoodcutter : MonoBehaviour
{
    public float speed = 2f;
    public float waktuTebang = 3f; 
    public float waktuSabar = 10f; 
    private float timerSabar;

    private UIManagerToko manager;
    private Transform targetPohon;
    private bool sedangMenebang = false;
    
    // --- LOGIKA PULANG ---
    private Vector3 posisiAwal; 
    private bool sedangKabur = false;
    // ------------------------------------------

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // --- SIMPAN POSISI SAAT DIA BARU MUNCUL ---
        posisiAwal = transform.position;
        // ------------------------------------------

        CariPohonPintar();
    }

    void Update()
    {
        if (manager == null) return;

        // 1. LOGIKA KABUR/PULANG
        if (sedangKabur || manager.jmlPenjaga > 0)
        {
            sedangKabur = true; // Kunci status sedang kabur
            JalanPulang();
            return;
        }

        // 2. LOGIKA MENCARI POHON
        if (targetPohon == null)
        {
            CariPohonPintar();
            if (targetPohon == null) 
            { 
                BerhentiDiTempat(); 
                timerSabar += Time.deltaTime;
                if(timerSabar >= waktuSabar)
                {
                    Debug.Log("Pembalak: Gak ada pohon, saya pulang!");
                    sedangKabur = true; // Mulai proses pulang
                }
                return; 
            }
        }

        timerSabar = 0;
        float jarak = Vector2.Distance(transform.position, targetPohon.position);

        if (jarak > 0.4f && !sedangMenebang)
        {
            Vector2 arah = (targetPohon.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, targetPohon.position, speed * Time.deltaTime);
            
            if(anim != null) {
                anim.SetFloat("moveX", arah.x);
                anim.SetFloat("moveY", arah.y);
                anim.SetBool("isWalking", true);
            }
        }
        else if (!sedangMenebang)
        {
            StartCoroutine(ProsesMenebang());
        }
    }

    void JalanPulang()
    {
        float jarakKeGerbang = Vector2.Distance(transform.position, posisiAwal);

        if (jarakKeGerbang > 0.1f)
        {
            // Hitung arah ke titik awal
            Vector2 arahPulang = (posisiAwal - transform.position).normalized;
            
            // Gerak menuju titik awal
            transform.position = Vector2.MoveTowards(transform.position, posisiAwal, speed * 1.5f * Time.deltaTime);

            // Update Animasi sesuai arah pulang
            if(anim != null) {
                anim.SetFloat("moveX", arahPulang.x);
                anim.SetFloat("moveY", arahPulang.y);
                anim.SetBool("isWalking", true);
            }
        }
        else
        {
            // Jika sudah sampai di titik awal, hilangkan objek
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator ProsesMenebang()
    {
        sedangMenebang = true;
        BerhentiDiTempat(); 
        yield return new WaitForSeconds(waktuTebang);
        if (targetPohon != null)
        {
            Destroy(targetPohon.gameObject);
            manager.jumlahPohon--; 
        }
        sedangMenebang = false;
        targetPohon = null; 
    }

    void CariPohonPintar()
    {
        GameObject folderPohon = GameObject.Find("pohon");
        if (folderPohon == null || folderPohon.transform.childCount == 0) return;
        List<Transform> daftarDewasa = new List<Transform>();
        List<Transform> daftarMuda = new List<Transform>();
        foreach (Transform p in folderPohon.transform)
        {
            BibitPertumbuhan bp = p.GetComponent<BibitPertumbuhan>();
            if (bp != null && bp.sudahDewasa) daftarDewasa.Add(p);
            else daftarMuda.Add(p);
        }
        int dadu = Random.Range(0, 100);
        if (dadu < 80 && daftarDewasa.Count > 0) targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
        else if (daftarMuda.Count > 0) targetPohon = daftarMuda[Random.Range(0, daftarMuda.Count)];
        else if (daftarDewasa.Count > 0) targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
    }

    void BerhentiDiTempat()
    {
        if(anim != null) {
            anim.SetBool("isWalking", false);
            anim.SetFloat("moveX", 0);
            anim.SetFloat("moveY", 0);
        }
    }
}