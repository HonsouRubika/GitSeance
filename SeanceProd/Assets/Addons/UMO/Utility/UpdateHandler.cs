/// Unity Modules - Utility
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using System;
using UnityEngine;

namespace UMO.Utility
{
	/// <summary>
	/// The UpdateHandler class is used for both UMO.CountdownTimer and UMO.Lerper
	/// </summary>
	public class UpdateHandler : MonoBehaviour
	{
		public static UpdateHandler Instance = null;

		public Action OnUpdate;

		public static void UpdateInstance()
		{
			if (Instance == null)
			{
				UpdateHandler handler = new GameObject("[Update Handler]").AddComponent<UpdateHandler>();
				Instance = handler;
				handler.gameObject.hideFlags = HideFlags.HideInHierarchy;
				DontDestroyOnLoad(handler.gameObject);
			}
		}

		private void Update()
		{
			OnUpdate?.Invoke();
		}
	}
}