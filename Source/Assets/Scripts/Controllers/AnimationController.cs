using UnityEngine;

public class AnimationController : MonoBehaviour 
{
	private Animator cachedAnimator;

	internal int currentAnimationHash;
    internal int currentAnimationLayer;
    internal float currentAnimationTransitionDelta;

	[System.Serializable]
	public struct AnimatorLayersInfos
	{
		internal int animationHash;
		internal float transitionDelta;
		internal float layerWeight;
	}
	private AnimatorLayersInfos[] animatorLayersInfos;

	public Animator CachedAnimator {
		get {
			if(cachedAnimator == null)
			{
				cachedAnimator = GetComponentInChildren<Animator>();

				if(animatorLayersInfos == null)
					animatorLayersInfos = new AnimatorLayersInfos[2];
			}

			return cachedAnimator;
		}
	}

	public AnimatorLayersInfos[] _AnimatorLayersInfos {
		get {
			if(animatorLayersInfos == null)
				animatorLayersInfos = new AnimatorLayersInfos[2];

			return animatorLayersInfos;
		}
	}

	public void SetLayerWeight(int pLayer, float pWeight)
	{
		_AnimatorLayersInfos[pLayer].layerWeight = pWeight;
		CachedAnimator.SetLayerWeight(pLayer, pWeight);
	}

	public float GetLayerWeight(int pLayer)
	{
		return CachedAnimator.GetLayerWeight(pLayer);
	}

	public void PlayState(string pStateName)
	{
		CachedAnimator.Play(pStateName);
	}

	public void PlayState(int pHash, int pLayer)
	{
		_AnimatorLayersInfos[pLayer].animationHash = pHash;
		_AnimatorLayersInfos[pLayer].transitionDelta = 0.0f;

		currentAnimationHash = pHash;
		currentAnimationTransitionDelta = 0.0f;
		currentAnimationLayer = pLayer;

		CachedAnimator.Play(pHash, pLayer);
	}

	public void CrossFadeInFixedTime(int pStateHash, float pTransition, int pLayer = 0)
	{
		_AnimatorLayersInfos[pLayer].animationHash = pStateHash;
		_AnimatorLayersInfos[pLayer].transitionDelta = pTransition;

		currentAnimationHash = pStateHash;
		currentAnimationTransitionDelta = pTransition;
		currentAnimationLayer = pLayer;

		CachedAnimator.CrossFadeInFixedTime(pStateHash, pTransition, pLayer);
	}

	public int GetStateHash(string pName)
	{
		return Animator.StringToHash(pName);
	}

	public void SetBool(string pName, bool pValue)
	{
		CachedAnimator.SetBool(pName, pValue);
	}

	public void SetInt(string pName, int pValue)
	{
		CachedAnimator.SetInteger(pName, pValue);
	}

	public void SetFloat(string pName, float pValue)
	{
		CachedAnimator.SetFloat(pName, pValue);
	}

	public void SetFloat(int pID, float pValue)
	{
		CachedAnimator.SetFloat(pID, pValue);
	}

	public float GetFloat (string name)
	{
		return CachedAnimator.GetFloat (name);
	}

	public float GetFloat(int pID)
	{
		return CachedAnimator.GetFloat(pID);
	}

	public void SetTrigger(string pName)
	{
		CachedAnimator.SetTrigger(pName);
	}

	public T GetBehaviour<T>() where T : StateMachineBehaviour
	{
		return CachedAnimator.GetBehaviour<T>();
	}

	public float GetCurrentAnimationLength(int pLayer)
	{
		return CachedAnimator.GetCurrentAnimatorStateInfo(pLayer).length;
	}

	public float GetNextAnimationLenght(int pLayer = 0)
	{
		return CachedAnimator.GetNextAnimatorStateInfo(pLayer).length;
	}

	public float GetAnimationLength(string pAnimationName)
	{
		for(int i = 0; i < CachedAnimator.runtimeAnimatorController.animationClips.Length; i++)
		{
			if(CachedAnimator.runtimeAnimatorController.animationClips[i].name.Equals(pAnimationName))
				return CachedAnimator.runtimeAnimatorController.animationClips[i].length;
		}

		return 0.0f;
	}
}