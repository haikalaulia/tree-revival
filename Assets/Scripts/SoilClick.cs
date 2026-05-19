using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;

	[HideInInspector] public Vector3 posisiGridTanam;

	private void OnMouseDown()
	{
		// 1. CEK BLOKIR UI (Sudah diperbaiki agar mendeteksi Joystick)
		if (IsClickBlockedByUI()) return;

		// 2. Tentukan posisi klik dunia (DEKLARASI HANYA SEKALI DI SINI)
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

		// 3. CEK APAKAH SUDAH ADA POHON DI TITIK INI
		if (CekApakahSudahAdaPohon(posisiGridTanam))
		{
			Debug.Log("Gagal: Titik ini sudah ada pohonnya!");
			return;
		}

		// 4. CEK PANEL PENUGASAN / INTRO
		GameObject panelIntroObj = GameObject.Find("Penugasan") ?? GameObject.Find("PanelIntro");
		if (panelIntroObj != null && panelIntroObj.activeInHierarchy)
			return;

		// 5. CEK PRESISI TOMBOL DASHBOARD
		GameObject btnDashboard = GameObject.Find("Btn_ToggleStatus");
		if (btnDashboard != null)
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, btnDashboard.transform.position);
			float jarak = Vector2.Distance(Input.mousePosition, screenPoint);
			if (jarak < 50f) return;
		}

		// 6. BUKA UI TOKO / TANAM
		UIManagerToko uiToko = FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			if (uiToko.panelToko.activeInHierarchy ||
			   (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
				return;

			uiToko.SetTanahAktif(this);
		}

		// 7. TAMPILKAN PANEL INFO TANAH
		PanelManager pm = FindFirstObjectByType<PanelManager>();
		if (pm != null)
			pm.TampilkanPanel(dataTanah);

		// 8. HUBUNGKAN KE SISTEM TUTORIAL
		if (TutorialManager.Instance != null)
			TutorialManager.Instance.SelesaiStep1();
	}

	// ====================================================================================
	// PERBAIKAN UTAMA: Mencegah klik tembus jika menyentuh UI BUTTON maupun JOYSTICK (IMAGE UI)
	// ====================================================================================
	private bool IsClickBlockedByUI()
	{
		if (UnityEngine.EventSystems.EventSystem.current == null) return false;

		var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
		eventData.position = Input.mousePosition;
		var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
		UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

		foreach (var result in results)
		{
			// JIKA SENTUHAN MENGENAI TOMBOL (Sistem Asli Kawanmu)
			if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
				return true;

			// FIX MOBILE: JIKA SENTUHAN MENGENAI JOYSTICK (Mengecek nama objek atau komponen Joystick-nya)
			if (result.gameObject.name == "Variable Joystick" || result.gameObject.GetComponentInParent<Joystick>() != null)
				return true;
		}
		return false;
	}

	private bool CekApakahSudahAdaPohon(Vector3 posisi)
	{
		GameObject folderPohon = GameObject.Find("pohon");
		if (folderPohon == null) return false;

		foreach (Transform pohon in folderPohon.transform)
		{
			if (Vector2.Distance(pohon.position, posisi) < 0.5f)
			{
				return true; // FIX: Sekarang mengembalikan nilai true jika ada pohon, sesuai fungsi aslinya
			}
		}
		return false;
	}

	public void EksekusiJalanLaluTanam(Action fungsiAsliTanamPohon)
	{
		PlayerMovement playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

		if (playerScript != null)
		{
			playerScript.PerintahJalanKeTanah(posisiGridTanam, () => {

				// 1. Matikan input jalan (biar player gak geser)
				playerScript.isPlanting = true;

				// 2. Paksa animator diam (reset kecepatan ke 0 agar transisi lain tidak ganggu)
				playerScript.anim.SetFloat("horizontal", 0);
				playerScript.anim.SetFloat("vertical", 0);

				// 3. Nyalakan Bool animasi mencangkul
				playerScript.anim.SetBool("isPlanting", true);

				// 4. Mulai proses kemunculan pohon
				StartCoroutine(ProsesTanamBerjeda(fungsiAsliTanamPohon, playerScript));
			});
		}
	}

	private System.Collections.IEnumerator ProsesTanamBerjeda(Action fungsiTanam, PlayerMovement pScript)
	{
		// Sesuaikan angka ini dengan durasi animasi mencangkul kamu (misal 1 detik)
		yield return new WaitForSeconds(1.0f);

		if (fungsiTanam != null) fungsiTanam.Invoke();

		// 5. Matikan Bool animasi (agar balik ke Idle)
		pScript.anim.SetBool("isPlanting", false);

		// 6. Buka kembali kontrol jalan
		pScript.isPlanting = false;
	}
}