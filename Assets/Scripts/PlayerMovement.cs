using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5;
	public int facingDirection = 1;
	public Rigidbody2D rb;
	public Animator anim;

	[HideInInspector] public bool isPlanting = false;

	[Header("Sistem Mobile Analog")]
	public Joystick joystick;

	private Vector2 targetPosition;
	private bool isMovingToTarget = false;
	private Action onReachTargetAction;

	void Update()
	{
		if (isMovingToTarget)
		{
			HandleAutoMovement();
			return;
		}

		// --- MODIFIKASI DIMULAI DI SINI ---
		
		// 1. Ambil input dari Keyboard (WASD / Panah)
		float hKey = Input.GetAxis("Horizontal");
		float vKey = Input.GetAxis("Vertical");

		// 2. Ambil input dari Joystick
		float hJoy = (joystick != null) ? joystick.Horizontal : 0;
		float vJoy = (joystick != null) ? joystick.Vertical : 0;

		// 3. Gabungkan keduanya
		float horizontal = hKey + hJoy;
		float vertical = vKey + vJoy;

		// 4. Batasi agar tidak lebih dari 1 (agar tidak lari super cepat jika keduanya ditekan)
		horizontal = Mathf.Clamp(horizontal, -1f, 1f);
		vertical = Mathf.Clamp(vertical, -1f, 1f);

		// --- MODIFIKASI SELESAI ---

		if (horizontal > 0 && transform.localScale.x < 0 ||
		horizontal < 0 && transform.localScale.x > 0)
		{
			Flip();
		}

		if (!isPlanting)
		{
			anim.SetFloat("horizontal", Mathf.Abs(horizontal));
			anim.SetFloat("vertical", vertical);
		}

		rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
	}

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

			if (!isPlanting)
			{
				anim.SetFloat("horizontal", Mathf.Abs(direction.x));
				anim.SetFloat("vertical", direction.y);
			}
		}
		else
		{
			rb.linearVelocity = Vector2.zero;
			isMovingToTarget = false;

			if (!isPlanting)
			{
				anim.SetFloat("horizontal", 0);
				anim.SetFloat("vertical", 0);
			}

			if (onReachTargetAction != null)
			{
				onReachTargetAction.Invoke();
				onReachTargetAction = null;
			}
		}
	}

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