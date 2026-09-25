using UnityEngine;

public class MainCamera : MonoBehaviour
{
	public Transform moonTransform;

	public Transform trackTransform;

	public Transform cameraTransform;

	public Transform playerCharacterTransform;

	public Transform standbyTransform;

	public Renderer backgroundRenderer;

	protected bool moonOnCamera;

	protected bool camOnPlayer;

	public Animator m_Animator;

	public void SetBackgroundMaterial(Material material)
	{
	}

	public void StartNewGame()
	{
	}

	public void Reset(bool running = true)
	{
	}

	public void StopGame()
	{
	}

	public void Jump(bool jumping)
	{
	}

	public void Slide(bool sliding)
	{
	}

	private void Run(bool running)
	{
	}

	public void FallBack(bool fallingBack)
	{
	}

	public void FallInHole(bool fallDown)
	{
	}

	public void Pause(bool isPaused, bool reset = false)
	{
	}

	public void Stumble()
	{
	}

	protected void FixMoonOnCamera(bool fixOnCamera)
	{
	}

	protected void FixCameraOnPlayer(bool fixCamOnPlayer)
	{
	}
}
