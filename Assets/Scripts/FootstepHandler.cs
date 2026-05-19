using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip footstepClip;

	// Fungsi ini akan dipanggil oleh Animation Event
	public void PlayFootstep()
	{
		if (audioSource != null && footstepClip != null)
		{
			audioSource.PlayOneShot(footstepClip);
		}
	}
}