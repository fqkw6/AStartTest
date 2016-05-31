using System;
using UnityEngine;
public static class MyPhysics
{
	public static float DistPointLine(Vector3 lineStart, Vector3 lineEnd, Vector3 targetPoint)
	{
		Vector3 vector = lineEnd - lineStart;
		Vector3 vector2 = targetPoint - lineStart;
		Vector3 vector3 = targetPoint - lineEnd;
		float num = Vector3.Dot(vector2, vector);
		if (num <= 0f)
		{
			return Vector3.Dot(vector2, vector2);
		}
		float num2 = Vector3.Dot(vector, vector);
		if (num >= num2)
		{
			return Vector3.Dot(vector3, vector3);
		}
		return Vector3.Dot(vector2, vector2) - num * num / num2;
	}

	public static bool TestSphereCapsule(Vector3 sphereCenter, float sphereR, Vector3 capsuleStart, Vector3 capsuleEnd, float capsuleR)
	{
		float num = MyPhysics.DistPointLine(capsuleStart, capsuleEnd, sphereCenter);
		float num2 = sphereR + capsuleR;
		return num <= num2 * num2;
	}

	public static bool TestPointSphere(Vector3 point, Vector3 sphereCenter, float sphereR)
	{
		return (point - sphereCenter).magnitude <= sphereR;
	}

	public static bool TestLineSphere(Vector3 lineStart, Vector3 lineEnd, Vector3 sphereCenter, float sphereR)
	{
		Vector3 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		if (magnitude < 0.001f)
		{
			return MyPhysics.TestPointSphere(lineStart, sphereCenter, sphereR);
		}
		vector /= magnitude;
		Vector3 vector2 = lineStart - sphereCenter;
		float num = Vector3.Dot(vector2, vector);
		float num2 = Vector3.Dot(vector2, vector2) - sphereR * sphereR;
		if (num2 > 0f && num > 0f)
		{
			return false;
		}
		float num3 = num * num - num2;
		if (num3 < 0f)
		{
			return false;
		}
		float num4 = -num - Mathf.Sqrt(num3);
		return num4 < 0f || num4 <= magnitude;
	}

	public static bool TestRaySphere(Vector3 linePos, Vector3 lineVector, Vector3 spherePos, float sphereR)
	{
		lineVector.Normalize();
		Vector3 vector = linePos - spherePos;
		float num = Vector3.Dot(vector, vector) - sphereR * sphereR;
		if (num <= 0f)
		{
			return true;
		}
		float num2 = Vector3.Dot(vector, lineVector);
		if (num2 > 0f)
		{
			return false;
		}
		float num3 = num2 * num2 - num;
		return num3 >= 0f;
	}

	public static bool RaycastNoTrigger(Vector3 origin, Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask)
	{
		hitinfo = default(RaycastHit);
		bool result = false;
		float num = 100000f;
		RaycastHit[] array = Physics.RaycastAll(origin, direction, distance, layerMask);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (!raycastHit.collider.isTrigger && raycastHit.distance < num)
			{
				num = raycastHit.distance;
				hitinfo = raycastHit;
				result = true;
			}
		}
		return result;
	}

	public static float AngleAxisX(Vector3 fromVector, Vector3 toVector)
	{
		fromVector.x = 0f;
		toVector.x = 0f;
		float num = Vector3.Angle(fromVector, toVector);
		if (Vector3.Cross(toVector, fromVector).x > 0f) {
			num = 180f + (180f - num);
		}

		return num;
	}

	public static Vector3 AdustVectorAxisX(Vector3 targetVector, Vector3 fromVector, Vector3 toVector)
	{
		targetVector.x = 0f;
		fromVector.x = 0f;
		toVector.x = 0f;
		float num = MyPhysics.AngleAxisX(fromVector, toVector);
		float num2 = MyPhysics.AngleAxisX(fromVector, targetVector);
		if (num2 <= num)
		{
			return targetVector;
		}
		if (360f - num2 < num2 - num)
		{
			return fromVector;
		}
		return toVector;
	}
}
