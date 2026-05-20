using UnityEngine;

public class NPCFollower : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public Transform targetYangDiikuti; 
    public float speed = 1.4f;          
    public float jarakBerhenti = 0.8f;  

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // --- TAMBAHAN DI SINI: LOGIKA AUTO-FIND PEMANDU ---
        // Jika kotak target di Inspector kosong (None), cari saudara kandungnya yang punya script Pemandu
        if (targetYangDiikuti == null)
        {
            // Mencari script NPCTourGuide yang ada di dalam satu keluarga (parent yang sama)
            NPCTourGuide pemandu = transform.parent.GetComponentInChildren<NPCTourGuide>();
            
            if (pemandu != null)
            {
                targetYangDiikuti = pemandu.transform;
                Debug.Log(gameObject.name + " berhasil menemukan pemimpin jalannya!");
            }
            else
            {
                Debug.LogWarning(gameObject.name + " tidak bisa menemukan Pemandu di dalam satu grup!");
            }
        }
        // ---------------------------------------------------
    }

    void Update()
    {
        if (targetYangDiikuti == null) return;

        float jarak = Vector2.Distance(transform.position, targetYangDiikuti.position);

        if (jarak > jarakBerhenti)
        {
            // JALAN MENGIKUTI
            transform.position = Vector2.MoveTowards(transform.position, targetYangDiikuti.position, speed * Time.deltaTime);
            
            // Hitung arah untuk animasi
            Vector2 arah = (targetYangDiikuti.position - transform.position).normalized;
            UpdateAnimation(arah, true);
        }
        else
        {
            // BERHENTI
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
}