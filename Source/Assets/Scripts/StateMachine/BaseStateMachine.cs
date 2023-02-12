using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseStateMachine<T>
{
	protected T context;
#pragma warning disable
	public event Action onStateChanged;
#pragma warning restore

    public BaseState<T> _CurrentState { get { return currentState; } }
	public BaseState<T> previousState;
	public float elapsedTimeInState = 0f;

    private Type currentStateType;
    public Type CurrentStateType
    {
        get
        {
            return currentStateType;
        }
    }

    private Dictionary<System.Type, BaseState<T>> states = new Dictionary<System.Type, BaseState<T>>();
	private BaseState<T> currentState;


	public BaseStateMachine( T pContext, BaseState<T> pInitialState )
	{
		this.context = pContext;

		// setup our initial state
		AddState( pInitialState );
		currentState = pInitialState;
		currentState.Begin();

        currentStateType = typeof( T );
	}


	/// <summary>
	/// adds the state to the machine
	/// </summary>
	public void AddState( BaseState<T> pState )
	{
		pState.SetMachineAndContext( this, context );
		states[pState.GetType()] = pState;
	}


	/// <summary>
	/// ticks the state machine with the provided delta time
	/// </summary>
	public void Update( float pDeltaTime )
	{
		elapsedTimeInState += pDeltaTime;
		currentState.Update( pDeltaTime );
	}

	public void LateUpdate()
	{
		currentState.LateUpdate ();
	}

	public void FixedUpdate()
	{
		currentState.FixedUpdate ();	
	}

    public void OnGUI()
    {
        currentState.OnGUI();
    }

	/// <summary>
	/// changes the current state
	/// </summary>
    public R ChangeState<R>(params object[] pParams) where R : BaseState<T>
	{
		// avoid changing to the same state
		var newType = typeof( R );
        if( currentStateType == newType )
			return currentState as R;

		// only call end if we have a currentState
		if( currentState != null )
			currentState.End();

		#if UNITY_EDITOR
		// do a sanity check while in the editor to ensure we have the given state in our state list
		if( !states.ContainsKey( newType ) )
		{
//			var error = GetType() + ": state " + newType + " does not exist. Did you forget to add it by calling addState?";
//			Debug.LogError( error );
//			throw new Exception( error );
		}
		#endif
  
        previousState = currentState;

        currentStateType = newType;

        if(states.ContainsKey(newType))
            currentState = states[newType];
        else
        {
            foreach(Type r in states.Keys)
            {
                if(r.IsSubclassOf(typeof(R))) 
                {
                    currentState = states [r];
                }
            }
        }


        // swap states and call begin
//		currentState = states[newType];
        currentState.Begin(pParams);
		elapsedTimeInState = 0f;

		// fire the changed event if we have a listener
		if( onStateChanged != null )
			onStateChanged();

		return currentState as R;
	}

}
