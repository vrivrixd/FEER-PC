using UnityEngine;

public class MainCamera : MonoBehaviour
{
	public Transform moonTransform;

	public Transform trackTransform;

	public Transform cameraTransform;

	public Transform playerCharacterTransform;

	public Transform standbyTransform;

	public Renderer backgroundRenderer;

	protected bool moonOnCamera = true;

	protected bool camOnPlayer;

	public Animator m_Animator;

	public void SetBackgroundMaterial(Material material)
	{
		backgroundRenderer.material = material;
	}

	public void StartNewGame()
	{
		Jump(false);
		Slide(false);
		FallBack(false);
		Run(false);
		FallInHole(false);
		FixCameraOnPlayer(true);
		FixMoonOnCamera(true);
		m_Animator.Play("static");
		m_Animator.speed = 1f;
		Run(true);
	}

	public void Reset(bool running = true)
	{
		Jump(false);
		Slide(false);
		FallBack(false);
		Run(false);
		FallInHole(false);
		FixMoonOnCamera(true);
		m_Animator.Play("static");
		m_Animator.speed = 1f;
		Run(running);
	}

	public void StopGame()
	{
		Jump(false);
		Slide(false);
		FallBack(false);
		Run(false);
		FallInHole(false);
		FixMoonOnCamera(true);
		FixCameraOnPlayer(false);
		m_Animator.Play("static");
		m_Animator.speed = 1f;
	}

	public void Jump(bool jumping)
	{
		FixMoonOnCamera(!jumping);
		m_Animator.SetBool("isJumping", jumping);
	}

	public void Slide(bool sliding)
	{
		m_Animator.SetBool("isSliding", sliding);
	}

	private void Run(bool running)
	{
		m_Animator.SetBool("isRunning", running);
	}

	public void FallBack(bool fallingBack)
	{
		FixMoonOnCamera(!fallingBack);
		m_Animator.SetBool("isFallingDown", fallingBack);
	}

	public void FallInHole(bool fallDown)
	{
		m_Animator.SetBool("isFallingInHole", fallDown);
	}

	public void Pause(bool isPaused, bool reset = false)
	{
		if (reset)
		{
			Jump(false);
			Slide(false);
			m_Animator.Play("CameraAnimation");
		}
		m_Animator.speed = isPaused ? 0f : 1f;
	}

	public void Stumble()
	{
		m_Animator.SetTrigger("triggerStumble");
	}

	protected void FixMoonOnCamera(bool fixOnCamera)
	{
		if (fixOnCamera)
		{
			if (!moonOnCamera)
			{
				moonTransform.SetParent(cameraTransform);
				moonTransform.localPosition = new Vector3(-7.83f, 2.02f, 25.4f);
				moonTransform.localScale = new Vector3(1f, 1f, 1f);
				moonTransform.localRotation = Quaternion.identity;
				moonOnCamera = true;
			}
		}
		else if (moonOnCamera)
		{
			moonTransform.SetParent(trackTransform);
			moonTransform.localPosition = new Vector3(-7.83f, 3.22f, 25.4f);
			moonTransform.localScale = new Vector3(1f, 1f, 1f);
			moonTransform.localRotation = Quaternion.identity;
			moonOnCamera = false;
		}
	}

	protected void FixCameraOnPlayer(bool fixCamOnPlayer)
	{
		if (fixCamOnPlayer)
		{
			if (!camOnPlayer)
			{
				cameraTransform.SetParent(playerCharacterTransform);
				camOnPlayer = true;
			}
		}
		else if (camOnPlayer)
		{
			cameraTransform.SetParent(standbyTransform);
			camOnPlayer = false;
		}
	}
}
