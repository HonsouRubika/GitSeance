/// Author: Julien Haigron
/// Last modified by: Julien Haigron

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seance.TurnSystem;

namespace Seance.Interactions
{
	public class RollDiceInteractor : Interactor
	{
		public Dice20 _dice;

		[SerializeField] AudioClip[] _diceClips;

		int _lastDiceClip = -1;

		private void Start()
		{
			_dice = GetComponentInParent<Dice20>();
		}

		// Start is called before the first frame update
		public override void Interact(PlayerTurnState turn)
		{
			if (Dice20.Instance.DiceValue < 20)
			{
				int clip = Random.Range(0, _diceClips.Length);

				while (_lastDiceClip == clip)
				{
					clip = Random.Range(0, _diceClips.Length);
				}
				_lastDiceClip = clip;
				AudioManager.Instance.PlayEffectOnTmpSource(_diceClips[clip]);
			}

			turn.PlayerCheatedDice();

			_dice.IncreaseDiceValue();
		}
	}
}
