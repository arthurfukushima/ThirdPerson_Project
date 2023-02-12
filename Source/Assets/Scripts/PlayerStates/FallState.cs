using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();
        Vector3 moveDirection = context.GetMoveDirection();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveDirection, context.GravityForce);
        context.UpdateCharacterRotation(context.TargetRotation);
        
        if(context.grounded)
        {
            machine.ChangeState<LandState>();
            return;
        }
    }
}
