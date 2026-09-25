using System;
using UnityEngine;
using UnityEngine.Events;

namespace MentalHome.CustomURLScheme
{
	public class CustomURLSchemeListener : MonoBehaviour
	{
		[Serializable]
		public class StringEvent : UnityEvent<string>
		{
		}

		public StringEvent urlOpenedEvent;

		public bool dontDestroyOnLoad;

		private void Start()
		{
		}

		public void URLOpened(string url)
		{
		}
	}
}
