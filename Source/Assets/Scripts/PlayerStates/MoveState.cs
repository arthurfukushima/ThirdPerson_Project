using UnityEngine;

public class MoveState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);

        context.onStartFall += OnStartFall;
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();
        Vector3 moveDirection = context.GetMoveDirection();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveDirection, context.GravityForce);
        context.UpdateCharacterRotation(context.TargetRotation);

        if (context.input.aim)
        {
            machine.ChangeState<AimState>();
            return; 
        }

        if (context.CanJump() && context.input.jump)
        {
            machine.ChangeState<JumpState>();
            return;
        }

        if (context.input.dash)
        {
            machine.ChangeState<DashState>();
            return;
        }

        if (moveInput == Vector2.zero)
        {
            machine.ChangeState<IdleState>();
            return;
        }
    }

    public override void End()
    {
        base.End();

        context.onStartFall -= OnStartFall;
    }

    public void OnStartFall()
    {
        machine.ChangeState<FallState>();
    }
}
