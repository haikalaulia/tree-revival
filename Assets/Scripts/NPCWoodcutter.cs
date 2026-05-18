using UnityEngine;
using System.Collections.Generic;

public class NPCWoodcutter : MonoBehaviour
{
    public float speed = 2f;
    public float waktuTebang = 3f; 
    private UIManagerToko manager;
    private Transform targetPohon;
    private bool sedangMenebang = false;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CariPohonPintar();
    }

    void Update()
    {
        if (manager == null) return;

        // 1. Jika ada Penjaga Hutan, langsung kabur
        if (manager.jmlPenjaga > 0) { Kabur(); return; }

        if (targetPohon == null)
        {
            CariPohonPintar();
            if (targetPohon == null) { BerhentiDiTempat(); return; }
        }

        float jarak = Vector2.Distance(transform.position, targetPohon.position);

        // 2. Logika Pergerakan
        if (jarak > 0.4f && !sedangMenebang)
        {
            // Hitung arah menuju pohon
            Vector2 arah = (targetPohon.position - transform.position).normalized;

            // Gerakkan pembalak
            transform.position = Vector2.MoveTowards(transform.position, targetPohon.position, speed * Time.deltaTime);
            
            // Kirim angka arah ke Animator untuk Blend Tree
            if(anim != null) {
                anim.SetFloat("moveX", arah.x);
                anim.SetFloat("moveY", arah.y);
                anim.SetBool("isWalking", true);
            }
            
            if(spriteRenderer != null) spriteRenderer.flipX = false; 
        }
        else if (!sedangMenebang)
        {
            // Sudah sampai: Berhenti dan diam sejenak
            StartCoroutine(ProsesMenebang());
        }
    }

    System.Collections.IEnumerator ProsesMenebang()
    {
        sedangMenebang = true;
        
        // Panggil fungsi berhenti agar animasi jalan mati saat menebang
        BerhentiDiTempat(); 
        
        Debug.Log("Pembalak berhenti di depan pohon...");
        yield return new WaitForSeconds(waktuTebang);

        if (targetPohon != null)
        {
            Destroy(targetPohon.gameObject);
            manager.jumlahPohon--; 
            Debug.Log("Pembalak: Pohon ditebang!");
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
        if (dadu < 80 && daftarDewasa.Count > 0)
            targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
        else if (daftarMuda.Count > 0)
            targetPohon = daftarMuda[Random.Range(0, daftarMuda.Count)];
        else if (daftarDewasa.Count > 0)
            targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
    }

    void Kabur()
    {
        if(anim != null) {
            anim.SetBool("isWalking", true);
            anim.SetFloat("moveX", 0);
            anim.SetFloat("moveY", 1);
        }
        transform.position += Vector3.up * speed * 1.5f * Time.deltaTime;
        Destroy(gameObject, 2f);
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