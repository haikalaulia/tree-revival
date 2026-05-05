using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5;
	public int facingDirection = 1;
	public Rigidbody2D rb;
	public Animator anim;

	void Update()
	{
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

	void Flip()
	{
		facingDirection *= -1;
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}
}

