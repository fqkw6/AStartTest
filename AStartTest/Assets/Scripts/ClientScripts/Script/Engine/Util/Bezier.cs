using UnityEngine;

[System.Serializable]

public class Bezier : System.Object
	
{
	private Vector3 oa;
	private Vector3 aa;
	private Vector3 bb;
	private Vector3 cc;

	public Bezier( Vector3 a, Vector3 b, Vector3 c, Vector3 d )
		
	{
		oa = a;
		aa = (-a + 3*(b-c) + d);
		bb = 3*(a+c) - 6*b;
		cc = 3*(b-a);
	}
	
	// 0.0 >= t <= 1.0
	
	public Vector3 GetPointAtTime( float t )
		
	{
		Vector3 p = ((aa* t + (bb))* t + cc)* t + oa;
		
		return p;
		
	}
}