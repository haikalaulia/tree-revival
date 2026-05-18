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
    }

    void Update()
    {
        if (targetYangDiikuti == null) return;

        float jarak = Vector2.Distance(transform.position, targetYangDiikuti.position);

        if (jarak > jarakBerhenti)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetYangDiikuti.position, speed * Time.deltaTime);
            
            Vector2 arah = (targetYangDiikuti.position - transform.position).normalized;
            UpdateAnimation(arah, true);
        }
        else
        {
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