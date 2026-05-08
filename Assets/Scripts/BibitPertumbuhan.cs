using UnityEngine;

public class BibitPertumbuhan : MonoBehaviour
{
    private GameObject prefabSedang;
    private GameObject prefabDewasa;
    public float waktuTumbuh = 5f;

    public void MulaiTumbuh(GameObject sedang, GameObject dewasa)
    {
        prefabSedang = sedang;
        prefabDewasa = dewasa;
        Invoke("JadiSedang", waktuTumbuh);
    }

    void JadiSedang()
    {
        if (prefabSedang != null)
        {
            GameObject sedang = Instantiate(prefabSedang, transform.position, Quaternion.identity, transform.parent);
            sedang.AddComponent<BibitPertumbuhan>().LanjutKeDewasa(prefabDewasa);
        }
        Destroy(gameObject);
    }

    public void LanjutKeDewasa(GameObject dewasa)
    {
        prefabDewasa = dewasa;
        Invoke("JadiDewasa", waktuTumbuh);
    }

    void JadiDewasa()
    {
        if (prefabDewasa != null)
        {
            Instantiate(prefabDewasa, transform.position, Quaternion.identity, transform.parent);
        }
        Destroy(gameObject);
    }
}