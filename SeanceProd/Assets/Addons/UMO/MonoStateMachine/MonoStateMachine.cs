using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet;
using FishNet.Object;

public abstract class MonoStateMachine : NetworkBehaviour
{
	[SerializeField] InitMode _initMode = InitMode.InitOnAwake;
	[SerializeField] MonoState _firstState;

	bool _initialized = false;
	bool _isActive = false;

	MonoState[] _monoStates;
	MonoState _activeState;

	public void SetState(string newState)
	{
		SetState(newState, false);
	}

	public void SetState(string newState, bool forceState)
	{
		foreach(MonoState monoState in _monoStates)
		{
			if(monoState.StateName == newState)
			{
				SetState(monoState, forceState);
				break;
			}
		}
	}

	public void SetState(MonoState newState)
	{
		SetState(newState, false);
	}

	public void SetState(MonoState newState, bool forceState)
	{
		if (newState == null)
			throw new ArgumentNullException();

		if(newState != _activeState || forceState)
		{
			if(_activeState != null)
				_activeState.OnStateExit();
			_activeState = newState;
			_activeState.OnStateEnter();
		}
	}

	[ObserversRpc]
	public void Init()
	{
		if (_initialized)
			return;

		_monoStates = GetComponents<MonoState>();

		if (_monoStates.Length == 0)
			throw new EmptyCollectionException("No MonoState found for this MonoStateMachine.");

		Play();

		SetState(_firstState);

		_initialized = true;
	}

	public void Play()
	{
		_isActive = true;
	}

	public void Pause()
	{
		_isActive = false;
	}

	private void Awake()
	{
		if (_initMode == InitMode.InitOnAwake)
			Init();
	}

	private void Start()
	{
		if (_initMode == InitMode.InitOnStart)
			Init();

		if (!_isActive || !_initialized)
			return;
	}

	private void Update()
	{
		if (_initMode == InitMode.InitOnFirstUpdate)
			Init();

		if (!_isActive || !_initialized)
			return;

		_activeState.OnStateUpdate();
	}

	private void LateUpdate()
	{
		if (_initMode == InitMode.InitOnFirstLateUpdate)
			Init();

		if (!_isActive || !_initialized)
			return;

		_activeState.OnStateLateUpdate();
	}

	private void FixedUpdate()
	{
		if (_initMode == InitMode.InitOnFirstFixedUpdate)
			Init();

		if (!_isActive || !_initialized)
			return;

		_activeState.OnStateFixedUpdate();
	}

	enum InitMode
	{
		InitOnAwake = 0,
		InitOnStart = 1,
		InitOnFirstUpdate = 2,
		InitOnFirstLateUpdate = 3,
		InitOnFirstFixedUpdate = 4,
		InitManually = 5
	}
}

class EmptyCollectionException : SystemException
{
	public EmptyCollectionException() : base("Collection is empty.")
	{

	}

	public EmptyCollectionException(string message) : base(message)
	{

	}

	public EmptyCollectionException(string message, Exception inner) : base(message, inner)
	{

	}
}