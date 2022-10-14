/// Unity Modules - Lerpers
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using System;
using UMO.Lerpers;
using UnityEngine;

public class ColorLerper : Lerper
{
	#region Private Variables

	Action<Color> _lerpAction;

	Func<Color, Color, float, Color> _processFunc;

	Color _start;
	Color _end;

	#endregion

	#region Public Methods

	/// <summary>
	/// Start a new lerp.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="time"></param>
	/// <param name="lerpAction"></param>
	/// <returns>The lerper.</returns>
	public ColorLerper Lerp(Color a, Color b, float time, Action<Color> lerpAction)
	{
		return Lerp(a, b, time, lerpAction, _completedAction);
	}

	/// <summary>
	/// Start a new lerp.
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="time"></param>
	/// <param name="lerpAction"></param>
	/// <param name="completedAction"></param>
	/// <returns>The lerper.</returns>
	public ColorLerper Lerp(Color a, Color b, float time, Action<Color> lerpAction, Action completedAction)
	{
		return Init(a, b, time, lerpAction, completedAction, LerpProcessType.Lerp);
	}

	/// <summary>
	/// Set a new lerpAction to the lerper.
	/// </summary>
	/// <param name="lerpAction"></param>
	/// <returns>The lerper.</returns>
	public ColorLerper SetLerpAction(Action<Color> lerpAction)
	{
		if (lerpAction == null)
		{
			throw new ArgumentException("Lerper's lerpAction can not be null.");
		}

		_lerpAction = lerpAction;

		return this;
	}

	/// <summary>
	/// Set a new completedAction to the lerper.
	/// </summary>
	/// <param name="completedAction"></param>
	/// <returns>The lerper.</returns>
	public ColorLerper SetCompletedAction(Action completedAction)
	{
		_completedAction = completedAction;

		return this;
	}

	/// <summary>
	/// Set both lerpAction and completedAction of the lerper.
	/// </summary>
	/// <param name="lerpAction"></param>
	/// <param name="completedAction"></param>
	/// <returns>The lerper.</returns>
	public ColorLerper SetActions(Action<Color> lerpAction, Action completedAction)
	{
		SetLerpAction(lerpAction);
		SetCompletedAction(completedAction);

		return this;
	}

	/// <summary>
	/// Replay the lerper with the same parameters.
	/// </summary>
	/// <returns>The lerper.</returns>
	public ColorLerper Replay()
	{
		return Init(_start, _end, _time, _lerpAction, _completedAction, _processType);
	}

	/// <summary>
	/// End the lerper and call the completedAction.
	/// </summary>
	/// <returns>True if the lerper was not already finished and the lerpAction is not null.</returns>
	public bool End()
	{
		if (_finished || _lerpAction == null)
			return false;

		_deltaTime = _time;
		Tick();
		return true;
	}

	#endregion

	#region Private Methods

	ColorLerper Init(Color a, Color b, float time, Action<Color> lerpAction, Action completedAction, LerpProcessType process)
	{
		if (time < 0f)
		{
			throw new ArgumentException("Lerper can not handle negative time.");
		}

		if (lerpAction == null)
		{
			throw new ArgumentException("Lerper's lerpAction can not be null.");
		}

		_start = a;
		_end = b;
		_time = time;
		_deltaTime = 0f;

		SetActions(lerpAction, completedAction);

		_processFunc = Color.Lerp;

		_finished = false;

		MountUpdate();

		return this;
	}

	protected override void ExecuteProcess()
	{
		_lerpAction(_processFunc(_start, _end, _completion));
	}

	#endregion
}