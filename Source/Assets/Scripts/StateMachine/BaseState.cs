using UnityEngine;

public abstract class BaseState<T>
{
    protected bool isCurrentState;
    protected float timeOnState;
	protected int mecanimStateHash;
	protected BaseStateMachine<T> machine;
	protected T context;
	
	
	public BaseState()
	{}


	/// <summary>
	/// constructor that takes the mecanim state name as a string
	/// </summary>
	public BaseState( string pMecanimStateName ) : this( Animator.StringToHash( pMecanimStateName ) )
	{}
	
	
	/// <summary>
	/// constructor that takes the mecanim state hash
	/// </summary>
	public BaseState( int pMecanimStateHash )
	{
        mecanimStateHash = pMecanimStateHash;
	}
	
	internal void SetMachineAndContext( BaseStateMachine<T> pMachine, T pContext )
	{
        machine = pMachine;
		context = pContext;

		OnInitialized();
	}

	/// <summary>
	/// called directly after the machine and context are set allowing the state to do any required setup
	/// </summary>
	public virtual void OnInitialized()
	{}

    public virtual void Begin(object[] pParams = null)
	{
        isCurrentState = true;
        timeOnState = 0.0f;
    }
	
	public virtual void Update( float pDeltaTime )
    {
        timeOnState += pDeltaTime ;  
    }

	public virtual void LateUpdate(){}

	public virtual void FixedUpdate(){}

	public virtual void End()
	{
        isCurrentState = false;
    }

    public virtual void OnGUI()
    {
       
    }
}
