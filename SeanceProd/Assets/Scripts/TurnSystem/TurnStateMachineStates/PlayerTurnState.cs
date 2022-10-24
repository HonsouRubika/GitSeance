/// Author: Nicolas Capelier
/// Last modified by: Julien Haigron

using Seance.Interactions;
using Seance.Networking;
using Seance.Utility;
using Seance.Wayfarer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seance.TurnSystem
{
	public class PlayerTurnState : MonoState
	{
		TurnStateMachine _machine;
		LobbyManager _lobby;
		WayfarerAI _wayfarer;

		[Header("Params")]
		[SerializeField] LayerMask _interactableLayer;

		[Header("References")]
		[SerializeField] AudioClip[] _knocksClips;
		int _lastKnockClip = -1;
		[SerializeField] AudioClip _miTurnClip;
		[SerializeField] AudioClip _fullTurnClip;
		[SerializeField] GameObject _turnIndicatorUI;

		CountdownTimer _miTurnTimer = new();
		CountdownTimer _fullTurnTimer = new();

		private void Start()
		{
			_machine = GameManager.TurnStateMachine;
			_lobby = GameManager.Lobby;
			_wayfarer = GameManager.WayfarerAI;
		}

		public override void OnStateEnter()
		{
			if (!_machine.IsPlaying)
			{
				_turnIndicatorUI.SetActive(false);
				return;
			}

			_turnIndicatorUI.SetActive(true);

			_miTurnTimer.SetTime(15f, DisplayMiTurnDialog);
			_fullTurnTimer.SetTime(30f, DisplayFullTurnDialog);
		}

		void DisplayMiTurnDialog()
		{
			GameManager.AudioManager.PlayMJVoice(_miTurnClip);
			GameManager.DialogManager.StartDialogFromFile("Dialogs/MiTurnText.txt");
		}

		void DisplayFullTurnDialog()
		{
			GameManager.AudioManager.PlayMJVoice(_fullTurnClip);
			GameManager.DialogManager.StartDialogFromFile("Dialogs/FullTurnText.txt");
			EndTurn();
		}

		public void OnClick(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			RaycastHit hit;

			Ray ray = _lobby._ownedPlayerInstance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

			Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, .8f);

			if (Physics.Raycast(ray, out hit, 50f, _interactableLayer))
			{
				Interactor interactor;

				if (hit.transform.TryGetComponent(out interactor))
				{
					interactor.Interact(this);
				}
			}
		}

		public void EndTurn()
		{
			if (!_machine.IsPlaying)
				return;

			_miTurnTimer.Cancel();
			_fullTurnTimer.Cancel();

			_machine.ServerPlayNextTurn();
		}

		public void PlayerCheatedDice()
		{
			if (_wayfarer.CurrentTarget == _lobby._ownedConnectionReferencePosition)
			{
				_machine.ServerSetWayfarerTarget(_lobby._ownedConnectionReferencePosition, true);
			}
		}

		public void PlayerKnocks()
		{
			int clip = Random.Range(0, _knocksClips.Length);

			while (_lastKnockClip == clip)
			{
				clip = Random.Range(0, _knocksClips.Length);
			}
			_lastKnockClip = clip;

			GameManager.AudioManager.PlayEffectOnTmpSource(_knocksClips[clip]);
			_machine.ServerSetWayfarerTarget(_lobby._ownedConnectionReferencePosition, false);
		}
	}
}
