using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);

        context.GetAnimationController().SetBool("Aiming", true);
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();
        Vector3 moveDirection = context.GetMoveDirection();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveDirection, context.GravityForce);
        context.UpdateCharacterRotation(context.GetCameraDirection());

        if (!context.input.aim)
        {
            machine.ChangeState<ThrowState>();
            return;
        }
        else
        {

        }
    }

    public override void End()
    {
        base.End();

        context.GetAnimationController().SetBool("Aiming", false);
    }
}
