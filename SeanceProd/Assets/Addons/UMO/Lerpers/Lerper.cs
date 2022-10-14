/// Unity Modules - Lerpers
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.0
/// Version release date (dd/mm/yyyy): 29/07/2022

using System;
using UMO.Utility;
using UnityEngine;

namespace UMO.Lerpers
{
	public abstract class Lerper
	{
		#region variables

		protected Action _completedAction;

		protected LerpProcessType _processType;

		protected float _time;
		protected float _deltaTime;
		protected float _completion;

		protected bool _paused;
		protected bool _finished;
		protected bool _subscribed;

		#endregion

		#region Public Methods

		/// <returns>The current progression time of the lerper.</returns>
		public float GetTime()
		{
			return _deltaTime;
		}

		/// <returns>The current completion (between 0 and 1) of the lerper.</returns>
		public float GetCompletion()
		{
			return _completion;
		}

		/// <summary>
		/// Pause the lerp.
		/// </summary>
		public void Pause()
		{
			_paused = true;
		}

		/// <summary>
		/// Resume the lerp.
		/// </summary>
		public void Resume()
		{
			_paused = false;
		}

		/// <returns>True if the lerper is paused.</returns>
		public bool IsPaused()
		{
			return _paused;
		}

		/// <returns>True if the lerper is completed.</returns>
		public bool IsFinished()
		{
			return _finished;
		}

		/// <summary>
		/// Cancel the lerper without calling the completedAction.
		/// </summary>
		/// <returns>True if the lerper was not already finished.</returns>
		public bool Cancel()
		{
			if (_finished)
				return false;

			_finished = true;
			_deltaTime = 0f;
			return true;
		}

		#endregion

		#region Protected Methods

		protected void Tick()
		{
			if (_deltaTime < _time)
			{
				_deltaTime += Time.deltaTime;
				_completion = _deltaTime / _time;
				ExecuteProcess();
			}
			else
			{
				UnMountUpdate();
				_completedAction?.Invoke();
				_finished = true;
			}
		}

		protected abstract void ExecuteProcess();

		#endregion

		#region Enums

		protected enum LerpProcessType
		{
			/// <summary>
			/// Handles: float, Vector2, Vector3, Vector4, Quaternion, Color, Color32.
			/// </summary>
			Lerp,
			/// <summary>
			/// Handles: Vector3, Quaternion.
			/// </summary>
			Slerp
		}

		#endregion

		#region Update Handling

		protected void MountUpdate()
		{
			if (_subscribed)
				return;

			UpdateHandler.UpdateInstance();
			UpdateHandler.Instance.OnUpdate += Tick;

			_subscribed = true;
		}

		protected void UnMountUpdate()
		{
			if (!_subscribed)
				return;

			UpdateHandler.Instance.OnUpdate -= Tick;
			_subscribed = false;
		}

		#endregion
	}
}