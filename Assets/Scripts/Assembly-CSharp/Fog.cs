using UnityEngine;

public class Fog : MonoBehaviour
{
	public Transform parentTransform;

	public ParticleSystem staticBackgroundFog;

	public Transform[] fogElements;

	public ParticleSystem[] fogParticleSystem;

	protected bool m_IsMoving;

	protected float m_SpeedScaleFactor = 2f;

	protected float m_SpeedMoveFactor = -3f;

	private void Update()
	{
		if (m_IsMoving)
		{
			MoveFog();
		}
	}

	public void MoveFog()
	{
		float speed = CustomGameManager.instance.m_Speed;
		float scale = m_SpeedScaleFactor;
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < fogElements.Length; i++)
		{
			fogElements[i].Translate(new Vector3(0f, 0f, 0f - speed / scale * deltaTime));
			if (fogElements[i].localPosition.z <= m_SpeedMoveFactor)
			{
				Vector3 localPosition = fogElements[i].localPosition;
				localPosition.z = 15f;
				fogElements[i].localPosition = localPosition;
			}
		}
	}

	public void PauseFog()
	{
		m_IsMoving = false;
		for (int i = 0; i < fogParticleSystem.Length; i++)
		{
			fogParticleSystem[i].Pause();
		}
		staticBackgroundFog.Pause();
	}

	public void UnpauseFog(bool resetFog = true)
	{
		ShowFog();
		for (int i = 0; i < fogParticleSystem.Length; i++)
		{
			fogParticleSystem[i].Play();
		}
		staticBackgroundFog.Play();
		if (resetFog)
		{
			m_SpeedScaleFactor = 2f;
			m_SpeedMoveFactor = -3f;
			SetElementsHeight(0.5f);
		}
		m_IsMoving = true;
	}

	private void SetElementsHeight(float height)
	{
		for (int i = 0; i < fogElements.Length; i++)
		{
			Vector3 localPosition = fogElements[i].transform.localPosition;
			localPosition.y = height;
			fogElements[i].transform.localPosition = localPosition;
		}
	}

	public void SlowDownFog()
	{
		m_SpeedScaleFactor = 6f;
		m_SpeedMoveFactor = -6f;
		SetElementsHeight(1.8f);
	}

	public void SpeedUpFog()
	{
		m_SpeedScaleFactor = 2f;
		m_SpeedMoveFactor = -3f;
		SetElementsHeight(0.5f);
	}

	private void SetStartColor(ParticleSystem particleSystem, Color min, Color max)
	{
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = new ParticleSystem.MinMaxGradient(min, max);
	}

	public void ResetFog()
	{
		ShowFog();
		m_IsMoving = false;
		for (int i = 0; i < fogParticleSystem.Length; i++)
		{
			fogParticleSystem[i].Stop();
			fogParticleSystem[i].Clear();
			SetStartColor(fogParticleSystem[i], new Color(1f, 1f, 1f, 0.39607844f), new Color(1f, 1f, 1f, 0.88235295f));
		}
		SetElementZ(0, 10f);
		SetElementZ(1, 17f);
		SetElementZ(2, 24f);
		m_SpeedScaleFactor = 2f;
		m_SpeedMoveFactor = -3f;
		SetElementsHeight(0.5f);
		for (int j = 0; j < fogParticleSystem.Length; j++)
		{
			fogParticleSystem[j].Play();
			fogParticleSystem[j].Pause();
		}
		staticBackgroundFog.Play();
		staticBackgroundFog.Pause();
	}

	private void SetElementZ(int index, float z)
	{
		Vector3 localPosition = fogElements[index].localPosition;
		localPosition.z = z;
		fogElements[index].localPosition = localPosition;
	}

	public void ColorizeFog(ConsumableType consumable)
	{
		Color min;
		Color max;
		if (consumable == ConsumableType.Boost)
		{
			min = new Color(0.10980392f, 1f, 1f, 0.39607844f);
			max = new Color(1f, 1f, 1f, 0.78431374f);
		}
		else
		{
			min = new Color(1f, 1f, 1f, 0.39607844f);
			max = new Color(1f, 1f, 1f, 0.88235295f);
		}
		for (int i = 0; i < fogParticleSystem.Length; i++)
		{
			SetStartColor(fogParticleSystem[i], min, max);
		}
	}

	public void HideFog()
	{
		parentTransform.localPosition = new Vector3(100f, 0f, 0f);
	}

	public void ShowFog()
	{
		parentTransform.localPosition = Vector3.zero;
	}
}
