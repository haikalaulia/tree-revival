using UnityEngine;
using UnityEngine.Tilemaps;
using System; // Tetap dipertahankan untuk sistem callback Action pergerakan otomatis

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;
	public bool sudahDitanami = false;

	[HideInInspector] public Vector3 posisiGridTanam;

	private void OnMouseDown()
	{
		// ========================================================
		// PERBAIKAN: CEK APAKAH PANEL INTRO SEDANG TERBUKA
		// ========================================================
		// Menggunakan nama objek "Penugasan" berdasarkan judul panel visualmu
		GameObject panelIntroObj = GameObject.Find("Penugasan");

		// Backup cek jika nama di hierarchy kamu masih pakai nama default "PanelIntro"
		if (panelIntroObj == null)
		{
			panelIntroObj = GameObject.Find("PanelIntro");
		}

		// Jika panelnya ketemu dan lagi aktif di layar, batalkan klik tanah agar tidak tembus!
		if (panelIntroObj != null && panelIntroObj.activeInHierarchy)
		{
			return;
		}

		// 1. CEK PRESISI TOMBOL DASHBOARD
		GameObject btnDashboard = GameObject.Find("Btn_ToggleStatus");

		if (btnDashboard != null)
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, btnDashboard.transform.position);
			float jarak = Vector2.Distance(Input.mousePosition, screenPoint);

			if (jarak < 50f)
			{
				return;
			}
		}

		if (sudahDitanami) return;

		// SOLUSI ERROR CS0104: Mengganti Object.FindFirstObjectByType menjadi FindFirstObjectByType bawaan UnityEngine langsung
		UIManagerToko uiToko = FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			if (uiToko.panelToko.activeInHierarchy ||
			   (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
			{
				return;
			}

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Tilemap tilemap = GetComponent<Tilemap>();
			if (tilemap != null)
			{
				Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
				posisiGridTanam = tilemap.GetCellCenterWorld(cellPosition);
			}
			else
			{
				mouseWorldPos.z = 0;
				posisiGridTanam = mouseWorldPos;
			}
			uiToko.SetTanahAktif(this);
		}

		// SOLUSI ERROR CS0104: Mengganti Object.FindFirstObjectByType menjadi FindFirstObjectByType bawaan UnityEngine langsung
		PanelManager pm = FindFirstObjectByType<PanelManager>();
		if (pm != null)
		{
			pm.TampilkanPanel(dataTanah);
		}

		// ==========================================
		// HUBUNGKAN KE SISTEM TUTORIAL
		// ==========================================
		if (TutorialManager.Instance != null)
		{
			TutorialManager.Instance.SelesaiStep1();
		}
	}

	// ==================================================================================
	// FUNGSI UTAMA: Dipanggil oleh UIManagerToko kelompokmu saat tombol "Tanam" ditekan
	// ==================================================================================
	public void EksekusiJalanLaluTanam(Action fungsiAsliTanamPohon)
	{
		// Cari komponen pergerakan player di scene menggunakan Tag "Player" yang sudah dipasang
		PlayerMovement player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

		if (player != null)
		{
			// Perintahkan player jalan otomatis ke koordinat grid tanah yang baru saja diklik
			player.PerintahJalanKeTanah(posisiGridTanam, () => {

				// Setelah player menginjak lokasi, tandai tanah sudah terisi
				sudahDitanami = true;

				// Eksekusi fungsi instan memunculkan pohon milik kawanmu dari UIManagerToko
				if (fungsiAsliTanamPohon != null)
				{
					fungsiAsliTanamPohon.Invoke();
				}

				Debug.Log("Player sampai di tilemap! Pohon berhasil ditanam otomatis.");
			});
		}
		else
		{
			// Jaga-jaga kalau player belum diberi tag di Inspector, pohon tetap ditanam instan agar game tidak macet
			sudahDitanami = true;
			if (fungsiAsliTanamPohon != null) fungsiAsliTanamPohon.Invoke();
			Debug.LogWarning("Player dengan Tag 'Player' tidak ditemukan! Pohon ditanam instan.");
		}
	}
}