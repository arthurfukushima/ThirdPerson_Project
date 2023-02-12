using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);

        context.onLanded += OnLanded;
        context.Jump();
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        Vector2 moveInput = context.GetMovementInput();
        Vector3 moveDirection = context.GetMoveDirection();

        context.CalculateVerticalVelocity();
        context.ApplyMovement(moveDirection, context.GravityForce);
        context.UpdateCharacterRotation(context.TargetRotation);
    }

    public override void End()
    {
        base.End();

        context.onLanded -= OnLanded;
    }

    private void OnLanded()
    {
        machine.ChangeState<LandState>();
        return;
    }

    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.Box("Grounded: " + context.grounded);
    }
}
