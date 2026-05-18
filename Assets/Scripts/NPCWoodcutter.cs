using UnityEngine;
using System.Collections.Generic; // Penting untuk menggunakan List

public class NPCWoodcutter : MonoBehaviour
{
    public float speed = 2f;
    public float waktuTebang = 3f;
    private UIManagerToko manager;
    private Transform targetPohon;
    private bool sedangMenebang = false;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        CariPohonPintar();
    }

    void Update()
    {
        if (manager.jmlPenjaga > 0) { Kabur(); return; }

        if (targetPohon == null)
        {
            CariPohonPintar();
            if (targetPohon == null) Kabur();
            return;
        }

        float jarak = Vector2.Distance(transform.position, targetPohon.position);
        if (jarak > 0.2f && !sedangMenebang)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPohon.position, speed * Time.deltaTime);
        }
        else if (!sedangMenebang)
        {
            StartCoroutine(ProsesMenebang());
        }
    }

    void CariPohonPintar()
    {
        GameObject folderPohon = GameObject.Find("pohon");
        if (folderPohon == null || folderPohon.transform.childCount == 0) return;

        List<Transform> daftarDewasa = new List<Transform>();
        List<Transform> daftarMuda = new List<Transform>();

        // 1. Kelompokkan pohon yang ada
        foreach (Transform p in folderPohon.transform)
        {
            BibitPertumbuhan bp = p.GetComponent<BibitPertumbuhan>();
            if (bp != null && bp.sudahDewasa) 
                daftarDewasa.Add(p);
            else 
                daftarMuda.Add(p);
        }

        // 2. Logika Probabilitas (80% Dewasa, 20% Muda)
        int dadu = Random.Range(0, 100);

        if (dadu < 80 && daftarDewasa.Count > 0)
        {
            // Incar yang dewasa
            targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
            Debug.Log("Pembalak serakah! Mengincar pohon dewasa: " + targetPohon.name);
        }
        else if (daftarMuda.Count > 0)
        {
            // Incar yang muda (atau fallback jika tidak ada yang dewasa)
            targetPohon = daftarMuda[Random.Range(0, daftarMuda.Count)];
            Debug.Log("Pembalak membabat pohon muda: " + targetPohon.name);
        }
        else if (daftarDewasa.Count > 0)
        {
            // Jika tidak ada yang muda, terpaksa ambil yang dewasa
            targetPohon = daftarDewasa[Random.Range(0, daftarDewasa.Count)];
        }
    }

    System.Collections.IEnumerator ProsesMenebang()
    {
        sedangMenebang = true;
        yield return new WaitForSeconds(waktuTebang);

        if (targetPohon != null)
        {
            // Jika pohon berbuah ditebang, kurangi angka jmlBerbuah di manager
            BibitPertumbuhan bp = targetPohon.GetComponent<BibitPertumbuhan>();
            // (Tambahkan logika deteksi jenis pohon di sini jika perlu)

            Destroy(targetPohon.gameObject);
            manager.jumlahPohon--; 
        }

        sedangMenebang = false;
        targetPohon = null;
    }

    void Kabur()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
        Destroy(gameObject, 2f);
    }
}