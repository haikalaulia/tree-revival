using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
	private static BackgroundMusic instance;

	void Awake()
	{
		// Memastikan hanya ada 1 MusicManager di seluruh scene
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject); // Musik tidak akan hilang saat pindah scene
		}
		else
		{
			Destroy(gameObject); // Hapus jika sudah ada instance lain
		}
	}
}