using UnityEngine;

public class NPCFollower : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public Transform targetYangDiikuti; // Siapa yang diikuti (Pemandu atau turis di depannya)
    public float speed = 1.4f;          // Sedikit lebih lambat dari pemandu agar tidak terlalu mepet
    public float jarakBerhenti = 0.8f;  // Jarak aman antar pengikut

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (targetYangDiikuti == null) return;

        // Hitung jarak ke target
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