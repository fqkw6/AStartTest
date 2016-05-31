using UnityEngine;
using System.Collections;

public class ModelAnimatorEventHandler : MonoBehaviour
{
    public AnimationClip _defaultAnimationClip;
    public AnimationClip[] _animationClips;
    private Animator _animator;
    private int _currentAnimationIndex = 0;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnMouseUpAsButton()
    {
        _currentAnimationIndex = _currentAnimationIndex + 1 >= _animationClips.Length ? 0 : ++_currentAnimationIndex;
        // 只能从 idle 状态切换到动作
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_defaultAnimationClip.name))
        _animator.CrossFade(_animationClips[_currentAnimationIndex].name, 0.1f);
        if(!IsInvoking("BackToIdel"))
        Invoke("BackToIdel", _animationClips[_currentAnimationIndex].length);
    }

    private void BackToIdel()
    {
        _animator.CrossFade(_defaultAnimationClip.name, 0.1f);
    }
}
