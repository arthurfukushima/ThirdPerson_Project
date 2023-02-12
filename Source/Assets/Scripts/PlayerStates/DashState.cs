using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : BaseState<PlayerScript>
{
    public override void Begin(object[] pParams = null)
    {
        base.Begin(pParams);
    }

    public override void Update(float pDeltaTime)
    {
        base.Update(pDeltaTime);

        machine.ChangeState<IdleState>();
    }
}
