/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Interactions;
using Seance.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Level
{
    public class LevelElements : MonoBehaviour
    {
        [Header("References")]
		[SerializeField] Transform[] _playersSpawnPositions;
        public Transform[] PlayersSpawnPositions { get => _playersSpawnPositions; }
        [SerializeField] Dice20 _playerHealthDice;
        public Dice20 PlayerHealthDice { get => _playerHealthDice; }
	}
}
