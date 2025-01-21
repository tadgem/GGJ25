using Godot;
using System;
using System.Collections.Generic;

public class FSM<_Trigger, _State>
	where _State : Enum
	where _Trigger : Enum
{
	public struct TriggerInfo : IEquatable<TriggerInfo>
	{
		_State _src_state;
		_Trigger _trigger;


		public TriggerInfo(_State src, _Trigger t)
		{
			_src_state = src;
			_trigger = t;
		}

		public override bool Equals(object obj) => obj is TriggerInfo other && Equals(other);

		public bool Equals(TriggerInfo other)
		{
			return _src_state.Equals(other._src_state) &&
				   _trigger.Equals(other._trigger);
		}

		public override int GetHashCode()
		{
			var hash = new HashCode();
			hash.Add(_src_state);
			hash.Add(_trigger);
			return hash.ToHashCode();
		}

	}

	internal struct TransitionInfo
	{
		internal _State _dst_state;
		internal OnStateTransition _proc;

		internal TransitionInfo(_State s, OnStateTransition proc)
		{
			_dst_state = s;
			_proc = proc;
		}
	}

	public delegate _Trigger OnStateUpdate();
	public delegate void OnStateTransition();

	private Dictionary<_State, OnStateUpdate> _state_updates = new();
	private Dictionary<TriggerInfo, TransitionInfo> _state_transitions = new();

	private _State _current_state = default;

	private void HandleTrigger(_Trigger trigger)
	{
		var exitTransition = new TriggerInfo(_current_state, trigger);
		if (_state_transitions.ContainsKey(exitTransition))
		{
			// process entries
			_state_transitions[exitTransition]._proc();
			_current_state = _state_transitions[exitTransition]._dst_state;
		}
	}

	public void SetStartingState(_State s)
	{
		_current_state = s;
	}

	public void ExternalTrigger(_Trigger trigger)
	{
		HandleTrigger(trigger);
	}

	public void Process()
	{
		// do all FSM work
		if (!_state_updates.ContainsKey(_current_state))
		{
			return;
		}

		_Trigger trigger = _state_updates[_current_state]();

		HandleTrigger(trigger);
	}

	public void AddState(_State state, OnStateUpdate action)
	{
		if (_state_updates.ContainsKey(state))
		{
			GD.PrintErr($"{typeof(FSM<_Trigger, _State>).Name} : Already contains an action for state update : {state}");
			return;
		}
		_state_updates.Add(state, action);
	}

	public void AddStateTransition(_Trigger trigger, _State src_state, _State dst_state, OnStateTransition transition_action)
	{
		TriggerInfo info = new TriggerInfo(src_state, trigger);
		if (_state_transitions.ContainsKey(info))
		{
			GD.PrintErr($"{typeof(FSM<_Trigger, _State>).Name} : Already contains a transition for {src_state} -> {dst_state}");
			return;
		}
		_state_transitions.Add(info, new TransitionInfo(dst_state, transition_action));
	}
}