using UnityEngine;

public class NPCGuard : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float speed = 3f; // Kecepatan lari penjaga
    private Vector3 posisiAwal;
    private GameObject targetLawan;

    [Header("Referensi")]
    public UIManagerToko manager;
    public GameObject dialogCanvas;
    private Animator anim;

    void Start()
    {
        manager = FindFirstObjectByType<UIManagerToko>();
        anim = GetComponent<Animator>();
        
        // Catat posisi awal saat dia muncul (titik pos jaga)
        posisiAwal = transform.position;

        if(dialogCanvas != null) dialogCanvas.SetActive(false);

        if(manager != null)
        {
            Debug.Log("Penjaga Hutan: Saya siap berpatroli!");
        }
    }

    void Update()
    {
        // 1. CARI APAKAH ADA PEMBALAK DI PETA
        CariPembalak();

        if (targetLawan != null)
        {
            // 2. JIKA ADA LAWAN: KEJAR!
            MengejarLawan();
        }
        else
        {
            // 3. JIKA TIDAK ADA LAWAN: KEMBALI KE POS SEMULA
            KembaliKePos();
        }
    }

    void CariPembalak()
    {
        // Mencari objek yang memiliki script NPCWoodcutter
        NPCWoodcutter lawan = FindFirstObjectByType<NPCWoodcutter>();
        
        if (lawan != null)
        {
            targetLawan = lawan.gameObject;
        }
        else
        {
            targetLawan = null;
        }
    }

    void MengejarLawan()
    {
        // Hitung arah ke lawan
        Vector2 arah = (targetLawan.transform.position - transform.position).normalized;
        
        // Gerak menuju lawan
        transform.position = Vector2.MoveTowards(transform.position, targetLawan.transform.position, speed * Time.deltaTime);

        // Update Animasi (Jalan 4 Arah)
        UpdateAnimation(arah, true);
    }

    void KembaliKePos()
    {
        float jarakKePos = Vector2.Distance(transform.position, posisiAwal);

        if (jarakKePos > 0.1f)
        {
            // Hitung arah pulang
            Vector2 arahPulang = (posisiAwal - transform.position).normalized;
            
            // Gerak pulang (lebih lambat sedikit saat patroli santai)
            transform.position = Vector2.MoveTowards(transform.position, posisiAwal, (speed * 0.7f) * Time.deltaTime);
            
            UpdateAnimation(arahPulang, true);
        }
        else
        {
            // Sudah sampai di pos: Diam
            UpdateAnimation(Vector2.zero, false);
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

    // --- LOGIKA DIALOG (Tetap Ada) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null) 
            dialogCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogCanvas != null) 
            dialogCanvas.SetActive(false);
    }
}