using UnityEngine;
using System.Collections;

public class AnimationEventHandler : MonoBehaviour
{
    public Animator[] _animators;

    void Awake()
    {
    }

    void Start()
    {

    }

    public void OnPlayAnimation(string animName)
    {
        if (_animators != null) {
            foreach (var item in _animators) {
                item.CrossFade(animName, 0.1f);
            }
        }
    }

    public void OnPlayAnimation1(string animName)
    {
        if (_animators != null && _animators.Length > 0) {
            _animators[0].CrossFade(animName, 0.1f);
        }
    }

    public void OnPlayAnimation2(string animName)
    {
        if (_animators != null && _animators.Length > 1) {
            _animators[1].CrossFade(animName, 0.1f);
        }
    }

    public void OnPlayAnimation3(string animName)
    {
        if (_animators != null && _animators.Length > 2) {
            _animators[2].CrossFade(animName, 0.1f);
        }
    }

    public void OnPlayAnimation4(string animName)
    {
        if (_animators != null && _animators.Length > 3) {
            _animators[3].CrossFade(animName, 0.1f);
        }
    }

    public void OnPlayAnimation5(string animName)
    {
        if (_animators != null && _animators.Length > 4) {
            _animators[4].CrossFade(animName, 0.1f);
        }
    }
}
