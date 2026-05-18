using UnityEngine;

public class BibitPertumbuhan : MonoBehaviour
{
	private GameObject prefabSedang;
	private GameObject prefabDewasa;
	public float waktuTumbuh = 5f;
	public int bonusUangDewasa = 500; 

    // --- TAMBAHKAN VARIABEL INI ---
    public bool sudahDewasa = false; 
    // ------------------------------

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
            // Pohon sedang belum dianggap dewasa (sudahDewasa tetap false secara default)
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
            // --- MODIFIKASI BAGIAN INI ---
			GameObject pohonBaru = Instantiate(prefabDewasa, transform.position, Quaternion.identity, transform.parent);
            
            // Pasang kembali script ini ke pohon dewasa agar si Pembalak bisa mengecek variabelnya
            BibitPertumbuhan status = pohonBaru.AddComponent<BibitPertumbuhan>();
            status.sudahDewasa = true; // INI TANDANYA!
            // -----------------------------

			// Kasih uang ke pemain
			UIManagerToko uiManager = Object.FindFirstObjectByType<UIManagerToko>();
			if (uiManager != null)
			{
				uiManager.uangPemain += bonusUangDewasa;
				Debug.Log("<color=yellow>Profit!</color> Pohon dewasa memberi Rp " + bonusUangDewasa);
			}
		}
		Destroy(gameObject);
	}
}