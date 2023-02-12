using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : BaseState<PlayerScript>
{
    private float animationLenght = 0.9f;

    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);

        context.GetAnimationController().PlayState("Throw");
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();
        Vector3 moveDirection = context.GetMoveDirection();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveDirection, context.GravityForce);
        context.UpdateCharacterRotation(context.GetCameraDirection());

        if(timeOnState >= animationLenght)
        {
            machine.ChangeState<IdleState>();
            return;
        }
    }

    public override void End()
    {
        base.End();
    }
}
