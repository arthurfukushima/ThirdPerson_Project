using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveInput, context.GravityForce);

        /*
        context.CheckJump();
        context.CalculateGravity();
        context.CalculateMove();
        //context.UpdateCharacterRotation(context.TargetRotation);
        context.ApplyMovement();
        */

        if (context.input.aim)
        {
            machine.ChangeState<AimState>();
            return;
        }

        if(moveInput != Vector2.zero)
        {
            machine.ChangeState<MoveState>();
            return;
        }
    }
}
