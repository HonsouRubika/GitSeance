/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Level
{
    public class LevelReferences : MonoBehaviour
    {
        [Header("References")]
        public Transform _activePlayerTransform, _leftPlayerTransform, _rightPlayerTransform;

		#region Singleton

		public static LevelReferences Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion
	}
}
