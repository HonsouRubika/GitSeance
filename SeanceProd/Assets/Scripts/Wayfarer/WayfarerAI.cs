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
	public class WayfarerAI : NetworkBehaviour
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

		private void Start()
		{
			_lobby = GameManager.Lobby;
			_postProcessManager = GameManager.PostProcessManager;
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

			int index = _lobby._playerInstances[positionIndex].WorldPositionIndex;

			_lerper.Lerp(origin, _positions[index].rotation, _headMovementTime, delta => _renderer.rotation = delta, () => _isRotating = false);
		}

		[TargetRpc]
		public void TargetPunishPlayer(NetworkConnection conn)
		{
			GameManager.LevelElements.PlayerHealthDice.DecreaseDiceValue(2);
			_postProcessManager.SetPostProcess(PostProcessType.Spotted);
			int clip = Random.Range(0, _punishClips.Length);
			while (clip == _lastClip)
			{
				clip = Random.Range(0, _punishClips.Length);
			}
			_lastClip = clip;
			GameManager.AudioManager.PlayMJVoice(_punishClips[clip]);
			GameManager.DialogManager.PlaySingleLine("Dialogs/PunishText.txt", clip);
		}
	}
}