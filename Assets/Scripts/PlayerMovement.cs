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

		float horizontal = 0f;
		float vertical = 0f;

		if (joystick != null)
		{
			horizontal = joystick.Horizontal;
			vertical = joystick.Vertical;
		}
		else
		{
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");
		}

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