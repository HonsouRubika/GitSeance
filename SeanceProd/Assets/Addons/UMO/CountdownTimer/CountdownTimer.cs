/// Unity Modules - Countdown Timer
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using System;
using UnityEngine;
using UMO.Utility;

public class CountdownTimer
{
	#region Private Variables

	Action _action;

	float _time;
	float _deltaTime;

	bool _paused;
	bool _finished;
	bool _subscribed;

	#endregion

	#region public Methods

	/// <summary>
	/// Set a new time to the countdown timer.
	/// </summary>
	/// <param name="time"></param>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer SetTime(float time)
	{
		return SetTime(time, _action, _paused);
	}

	/// <summary>
	/// Set a new time to the countdown timer.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="action"></param>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer SetTime(float time, Action action)
	{
		return SetTime(time, action, false);
	}

	/// <summary>
	/// Set a new time to the countdown timer.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="startPaused"></param>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer SetTime(float time, bool startPaused)
	{
		return SetTime(time, _action, startPaused);
	}

	/// <summary>
	/// Set a new time to the countdown timer.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="action"></param>
	/// <param name="startPaused"></param>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer SetTime(float time, Action action, bool startPaused)
	{
		if (time < 0f)
		{
			Debug.LogError("Countdown timers can not handle negative time.");
			return this;
		}

		if (action == null)
		{
			Debug.LogError("Countdown timer's action can not be null.");
			return this;
		}

		_action = action;
		_time = _deltaTime = time;
		_paused = startPaused;
		_finished = false;

		MountUpdate();

		return this;
	}

	/// <summary>
	/// Set a new action to the countdown timer.
	/// </summary>
	/// <param name="action"></param>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer SetAction(Action action)
	{
		if (action == null)
			Debug.LogError("Countdown timer's action can not be null.");
		else
			_action = action;

		return this;
	}

	/// <summary>
	/// Replay the countdown timer with the same parameters.
	/// </summary>
	/// <returns>The countdown timer.</returns>
	public CountdownTimer Replay()
	{
		return SetTime(_time, _action, false);
	}

	/// <returns>The current time of the countdown timer.</returns>
	public float GetTime()
	{
		return _deltaTime;
	}

	/// <summary>
	/// Pause the countdown.
	/// </summary>
	public void Pause()
	{
		_paused = true;
	}

	/// <summary>
	/// Resume the countdown.
	/// </summary>
	public void Resume()
	{
		_paused = false;
	}

	/// <returns>True if the countdown timer is paused.</returns>
	public bool IsPaused()
	{
		return _paused;
	}

	/// <returns>True if the countdown timer is finished.</returns>
	public bool IsFinished()
	{
		return _finished;
	}

	/// <summary>
	/// End the countdown timer and call the action.
	/// </summary>
	/// <returns>True if the countdown timer was not already finished and the action is not null.</returns>
	public bool End()
	{
		if (_finished || _action == null)
			return false;

		_deltaTime = 0f;
		Tick();
		return true;
	}

	/// <summary>
	/// Cancel the countdown timer without calling the action.
	/// </summary>
	/// <returns>True if the countdown timer was not already finished.</returns>
	public bool Cancel()
	{
		if (_finished)
			return false;

		_finished = true;
		_deltaTime = 0f;
		return true;
	}

	#endregion

	#region Private Methods

	void Tick()
	{
		if (_paused || _finished)
			return;

		_deltaTime -= Time.deltaTime;

		if (_deltaTime <= 0f)
		{
			UnMountUpdate();

			_deltaTime = 0f;
			_finished = true;

			_action();
		}
	}

	void MountUpdate()
	{
		if (_subscribed)
			return;

		UpdateHandler.UpdateInstance();
		UpdateHandler.Instance.OnUpdate += Tick;

		_subscribed = true;
	}

	void UnMountUpdate()
	{
		if (!_subscribed)
			return;

		UpdateHandler.Instance.OnUpdate -= Tick;
		_subscribed = false;
	}

	#endregion
}