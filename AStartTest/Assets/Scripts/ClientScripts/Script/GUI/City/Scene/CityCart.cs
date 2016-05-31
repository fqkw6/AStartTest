using UnityEngine;
using System.Collections;
using DG.Tweening;

// 马车
public class CityCart : MonoBehaviour
{
    public Vector3 _originPosition;
    private bool _isInAnimation = false;
    private Animation _anim;

    void Awake()
    {
        _anim = GetComponent<Animation>();
    }

	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Idle()
    {
        _anim.Play("daiji");
    }

    public void PlayWood()
    {
        _anim.Play("mucai_zou");
    }

    public void PlayStone()
    {
        _anim.Play("shicai_zou");
    }
}
