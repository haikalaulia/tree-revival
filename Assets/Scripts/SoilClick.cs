using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SoilClick : MonoBehaviour
{
	[Header("Settings Tanah")]
	public SoilProperty dataTanah;

	[HideInInspector] public Vector3 posisiGridTanam;

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "SceneIntro")
		{
			StartCoroutine(JalankanTanamOtomatisIntro());
		}
	}

	private void OnMouseDown()
	{
		// 1. CEK BLOKIR UI (Mencegah klik tembus saat menyentuh tombol Dashboard/UI lainnya)
		if (IsClickBlockedByUI()) return;

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

		if (CekApakahSudahAdaPohon(posisiGridTanam))
		{
			Debug.Log("Gagal: Titik ini sudah ada pohonnya!");
			return;
		}

		// 2. CEK PANEL INTRO
		GameObject panelIntroObj = GameObject.Find("Penugasan") ?? GameObject.Find("PanelIntro");
		if (panelIntroObj != null && panelIntroObj.activeInHierarchy)
			return;

		// 3. BUKA UI TOKO / TANAM
		UIManagerToko uiToko = FindFirstObjectByType<UIManagerToko>();
		if (uiToko != null)
		{
			if (uiToko.panelToko.activeInHierarchy ||
			   (uiToko.panelAnalisis != null && uiToko.panelAnalisis.activeInHierarchy))
				return;

			uiToko.SetTanahAktif(this);
		}

		// 4. TAMPILKAN PANEL INFO TANAH
		PanelManager pm = FindFirstObjectByType<PanelManager>();
		if (pm != null)
			pm.TampilkanPanel(dataTanah);

		if (TutorialManager.Instance != null)
			TutorialManager.Instance.SelesaiStep1();
	}

	private bool IsClickBlockedByUI()
	{
		if (UnityEngine.EventSystems.EventSystem.current == null) return false;

		var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
		eventData.position = Input.mousePosition;
		var results = new List<UnityEngine.EventSystems.RaycastResult>();
		UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

		foreach (var result in results)
		{
			// Jika klik mengenai asset UI apa pun (Button, Image, Text Dashboard), langsung blokir klik tanah tembus
			if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null ||
				result.gameObject.GetComponentInParent<UnityEngine.UI.Button>() != null)
				return true;

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
				return true;
			}
		}
		return false;
	}

	public void EksekusiJalanLaluTanam(Action fungsiAsliTanamPohon)
	{
		PlayerMovement playerScript = FindFirstObjectByType<PlayerMovement>();

		if (playerScript != null)
		{
			playerScript.PerintahJalanKeTanah(posisiGridTanam, () => {
				playerScript.isPlanting = true;
				playerScript.anim.SetFloat("horizontal", 0);
				playerScript.anim.SetFloat("vertical", 0);
				playerScript.anim.SetBool("isPlanting", true);

				StartCoroutine(ProsesTanamBerjeda(fungsiAsliTanamPohon, playerScript));
			});
		}
	}

	private System.Collections.IEnumerator ProsesTanamBerjeda(Action fungsiTanam, PlayerMovement pScript)
	{
		yield return new WaitForSeconds(1.0f);

		if (fungsiTanam != null) fungsiTanam.Invoke();

		pScript.anim.SetBool("isPlanting", false);
		pScript.isPlanting = false;
	}

	private System.Collections.IEnumerator JalankanTanamOtomatisIntro()
	{
		yield return new WaitForSeconds(1.5f);

		PlayerMovement playerScript = FindFirstObjectByType<PlayerMovement>();
		UIManagerToko uiToko = FindFirstObjectByType<UIManagerToko>();

		if (playerScript != null && uiToko != null)
		{
			uiToko.SetTanahAktif(this);

			playerScript.isPlanting = true;
			playerScript.anim.SetFloat("horizontal", 0);
			playerScript.anim.SetFloat("vertical", 0);
			playerScript.anim.SetBool("isPlanting", true);

			yield return new WaitForSeconds(1.0f);

			// Mencari tombol beli secara dinamis di Canvas toko kelompokmu
			GameObject btnBeli = GameObject.Find("Dashboard") ?? GameObject.Find("Btn_BeliPohon") ?? GameObject.Find("Button_Tanam") ?? GameObject.Find("Btn_Tanam");
			if (btnBeli != null && btnBeli.GetComponent<UnityEngine.UI.Button>() != null)
			{
				btnBeli.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
			}

			playerScript.anim.SetBool("isPlanting", false);
			playerScript.isPlanting = false;
		}
	}
}