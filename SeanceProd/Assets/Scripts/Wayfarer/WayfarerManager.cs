/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using FishNet.Connection;
using FishNet.Object;
using Seance.Interactions;
using Seance.Networking;
using Seance.PostProcess;
using Seance.TurnSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Wayfarer
{
	public class WayfarerManager : NetworkBehaviour
	{
		[Header("Params")]
		[SerializeField] float _headMovementTime;

		[Header("References")]
		[SerializeField] Transform _renderer;
		[SerializeField] Transform[] _positions;
		[SerializeField] AudioClip[] _punishClips;
		int _lastClip = -1;

		LobbyManager _lobby;
		PostProcessManager _postProcessManager;

		QuaternionLerper _lerper = new();
		int _currentTarget;
		public int CurrentTarget { get { return _currentTarget; } }
		bool _isRotating;
		public bool IsRotating { get { return _isRotating; } }

		#region Singleton

		public static WayfarerManager Instance;

		private void Awake()
		{
			Instance = this;
		}

		#endregion

		private void Start()
		{
			_lobby = LobbyManager.Instance;
			_postProcessManager = PostProcessManager.Instance;
		}

		public void MoveToPosition(int positionIndex)
		{
			MoveToPosition(positionIndex, false);
		}

		public void MoveToPosition(int positionIndex, bool forcePosition)
		{
			if (positionIndex == _currentTarget && !forcePosition)
				return;

			if (_isRotating)
				_lerper.Cancel();

			_isRotating = true;

			if (positionIndex == _lobby._ownedConnectionReferencePosition)
				_postProcessManager.SetPostProcess(PostProcessType.Watched);
			else
				_postProcessManager.SetPostProcess(PostProcessType.Base);

			_currentTarget = positionIndex;

			Quaternion origin = _renderer.rotation;

			int index = _lobby._playerInstances[positionIndex]._worldPositionIndex;

			_lerper.Lerp(origin, _positions[index].rotation, _headMovementTime, delta => _renderer.rotation = delta, () => _isRotating = false);
		}

		[TargetRpc]
		public void TargetPunishPlayer(NetworkConnection conn)
		{
			Dice20.Instance.DecreaseDiceValue(2);
			_postProcessManager.SetPostProcess(PostProcessType.Spotted);
			int clip = Random.Range(0, _punishClips.Length);
			while (clip == _lastClip)
			{
				clip = Random.Range(0, _punishClips.Length);
			}
			_lastClip = clip;
			AudioManager.Instance.PlayMJVoice(_punishClips[clip]);
			DialogManager.Instance.PlaySingleLine("Dialogs/PunishText.txt", clip);
		}
	}
}