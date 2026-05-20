using UnityEngine;
using System.Collections.Generic;

public class NPCWoodcutter : MonoBehaviour
{
    [Header("Misi Pembalak")]
    public int maxPohonBolehDitebang = 1; // Atur berapa pohon yang dicuri sebelum dia pulang
    private int jumlahSudahDitebang = 0;

    [Header("Pengaturan Gerak")]
    public float speed = 2f;
    public float waktuTebang = 3f; 
    public float waktuSabar = 10f; 
    public float jarakPandangLawan = 5f; 

    private float timerSabar;
    private UIManagerToko manager;
    private Transform targetPohon;
    private bool sedangMenebang = false;
    private Vector3 posisiAwal; 
    private bool sedangKabur = false;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posisiAwal = transform.position;
        CariPohonPintar();
    }

    void Update()
    {
        if (manager == null) return;

        // 1. LOGIKA DETEKSI PENJAGA
        NPCGuard[] semuaPenjaga = Object.FindObjectsByType<NPCGuard>(FindObjectsSortMode.None);
        foreach (NPCGuard p in semuaPenjaga)
        {
            if (Vector2.Distance(transform.position, p.transform.position) < jarakPandangLawan)
            {
                sedangKabur = true;
            }
        }

        // --- TAMBAHAN: CEK KUOTA TEBANG ---
        // Jika sudah mencapai batas tebang, paksa status jadi sedangKabur agar dia pulang
        if (jumlahSudahDitebang >= maxPohonBolehDitebang)
        {
            sedangKabur = true;
        }
        // ---------------------------------

        if (sedangKabur)
        {
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
                if(timerSabar >= waktuSabar) sedangKabur = true;
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

    System.Collections.IEnumerator ProsesMenebang()
    {
        sedangMenebang = true;
        BerhentiDiTempat(); 
        
        Debug.Log("Pembalak mulai menebang...");
        yield return new WaitForSeconds(waktuTebang);

        if (targetPohon != null)
        {
            Destroy(targetPohon.gameObject);
            manager.jumlahPohon--; 

            // --- TAMBAHAN: HITUNG JUMLAH YANG BERHASIL DITEBANG ---
            jumlahSudahDitebang++;
            Debug.Log("Pembalak: Berhasil menebang " + jumlahSudahDitebang + " pohon.");
            // -----------------------------------------------------
        }

        sedangMenebang = false;
        targetPohon = null; 
    }

    void JalanPulang()
    {
        float lariCepat = speed * 1.5f;
        float jarakKeGerbang = Vector2.Distance(transform.position, posisiAwal);

        if (jarakKeGerbang > 0.1f)
        {
            Vector2 arahPulang = (posisiAwal - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, posisiAwal, lariCepat * Time.deltaTime);

            if(anim != null) {
                anim.SetFloat("moveX", arahPulang.x);
                anim.SetFloat("moveY", arahPulang.y);
                anim.SetBool("isWalking", true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakPandangLawan);
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