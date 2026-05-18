using System; // WAJIB ADA untuk menggunakan Action (Sistem Callback otomatis)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5;
	public int facingDirection = 1;
	public Rigidbody2D rb;
	public Animator anim;

	// Variabel Tambahan untuk Sistem Jalan Otomatis (Tanam Pohon di Gameplay)
	private Vector2 targetPosition;
	private bool isMovingToTarget = false;
	private Action onReachTargetAction;

	void Update()
	{
		// 1. JALAN OTOMATIS: Jika sedang otomatis berjalan ke petak lahan
		if (isMovingToTarget)
		{
			HandleAutoMovement();
			return; // Menahan input keyboard sementara agar tidak bentrok
		}

		// 2. JALAN MANUAL (KEYBOARD): Kode asli kamu tetap utuh 100%
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		if (horizontal > 0 && transform.localScale.x < 0 ||
		horizontal < 0 && transform.localScale.x > 0)
		{
			Flip();
		}

		anim.SetFloat("horizontal", Mathf.Abs(horizontal));
		anim.SetFloat("vertical", vertical);

		rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
	}

	// Fungsi Internal: Menghitung arah pergerakan otomatis ke target tanah
	void HandleAutoMovement()
	{
		Vector2 currentPos = transform.position;
		Vector2 direction = (targetPosition - currentPos).normalized;
		float distanceLeft = Vector2.Distance(currentPos, targetPosition);

		if (distanceLeft > 0.1f)
		{
			rb.linearVelocity = direction * speed;

			if (direction.x > 0 && transform.localScale.x < 0 ||
				direction.x < 0 && transform.localScale.x > 0)
			{
				Flip();
			}

			// Sinkronisasi parameter animasi jalan milikmu
			anim.SetFloat("horizontal", Mathf.Abs(direction.x));
			anim.SetFloat("vertical", direction.y);
		}
		else
		{
			// JIKA PLAYER SUDAH SAMPAI DI LOKASI TANAH
			rb.linearVelocity = Vector2.zero;
			isMovingToTarget = false; // Keyboard kamu langsung aktif kembali otomatis!

			anim.SetFloat("horizontal", 0);
			anim.SetFloat("vertical", 0);

			// Pemicu otomatis penanaman pohon
			if (onReachTargetAction != null)
			{
				onReachTargetAction.Invoke();
				onReachTargetAction = null;
			}
		}
	}

	// Fungsi Publik: Dipanggil dari script SoilClick kawanmu
	public void PerintahJalanKeTanah(Vector2 koordinatTanah, Action fungsiTanamPohon)
	{
		targetPosition = koordinatTanah;
		onReachTargetAction = fungsiTanamPohon;
		isMovingToTarget = true;
	}

	void Flip()
	{
		facingDirection *= -1;
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}
}