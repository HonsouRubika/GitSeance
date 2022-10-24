/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using Seance.Audio;
using Seance.DialogSystem;
using Seance.Level;
using Seance.Networking;
using Seance.PostProcess;
using Seance.TurnSystem;
using Seance.Wayfarer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Header("References")]
	[SerializeField] LobbyManager _lobby;
	public static LobbyManager Lobby { get => Instance._lobby; }

	[SerializeField] PlayerConnector _connector;
	public static PlayerConnector Connector { get => Instance._connector; }

	[SerializeField] LevelElements _levelElements;
	public static LevelElements LevelElements { get => Instance._levelElements; }

	[SerializeField] AudioManager _audioManager;
	public static AudioManager AudioManager { get => Instance._audioManager; }

	[SerializeField] DialogManager _dialogManager;
	public static DialogManager DialogManager { get => Instance._dialogManager; }

	[SerializeField] TurnStateMachine _turnStateMachine;
	public static TurnStateMachine TurnStateMachine { get => Instance._turnStateMachine; }

	[SerializeField] WayfarerAI _wayfarerAI;
	public static WayfarerAI WayfarerAI { get => Instance._wayfarerAI; }

	[SerializeField] PostProcessManager _postProcessManager;
	public static PostProcessManager PostProcessManager { get => Instance._postProcessManager; }

	private void Awake()
	{
		CreateSingleton(true);
	}
}