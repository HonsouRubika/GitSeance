/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using System;
using UnityEngine;
using UMO.Utility;

namespace Seance.Utility
{
	public class Sequencer
	{
		#region Private Variables

		List<TimedEvent> _events = new();

		float _deltaTime;

		int _index = -1;

		bool _paused;
		bool _finished = true;
		bool _subscribed;

		#endregion

		#region public Methods

		public Sequencer Append(float duration, Action startAction)
		{
			return Append(duration, startAction, null);
		}

		public Sequencer Append(float duration)
		{
			return Append(duration, null, null);
		}

		public Sequencer Append(float duration, Action startAction, Action endAction)
		{
			_events.Add(new TimedEvent(duration, startAction, endAction));
			return this;
		}

		public Sequencer Clear()
		{
			_events.Clear();
			return this;
		}

		public Sequencer Play()
		{
			if (_events.Count == 0)
				throw new ArgumentNullException("This sequence does not contain any event.");

			_index = -1;

			SetNextEvent();

			_finished = false;
			MountUpdate();

			return this;
		}

		/// <summary>
		/// Pause the sequence.
		/// </summary>
		public void Pause()
		{
			_paused = true;
		}

		/// <summary>
		/// Resume the sequence.
		/// </summary>
		public void Resume()
		{
			_paused = false;
		}

		/// <returns>True if the sequence is paused.</returns>
		public bool IsPaused()
		{
			return _paused;
		}

		/// <returns>True if the sequence is finished.</returns>
		public bool IsFinished()
		{
			return _finished;
		}

		/// <summary>
		/// Cancel the sequence without calling the action.
		/// </summary>
		/// <returns>True if the sequence was not already finished.</returns>
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
				_deltaTime = 0f;

				_events[_index]._endAction?.Invoke();

				SetNextEvent();
			}
		}

		void SetNextEvent()
		{
			_index++;
			if (_index < _events.Count)
			{
				_events[_index]._beginAction?.Invoke();
				_deltaTime = _events[_index]._duration;
			}
			else
			{
				_finished = true;
				UnMountUpdate();
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

		struct TimedEvent
		{
			public TimedEvent(float duration, Action beginAction, Action endAction)
			{
				_beginAction = beginAction;
				_endAction = endAction;
				_duration = duration;
			}

			public TimedEvent(float duration, Action action)
			{
				_beginAction = action;
				_duration = duration;
				_endAction = null;
			}

			public float _duration;
			public Action _beginAction;
			public Action _endAction;
		}
	}
}