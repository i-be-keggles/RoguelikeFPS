using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{

	public Transform camTransform;

	public float maxShake = 1f;

	Vector3 originalPos;

	public List<ShakeInstance> instances = new List<ShakeInstance>();


	void OnEnable()
	{
		if (camTransform == null) camTransform = transform;
		originalPos = camTransform.localPosition;
	}

	void FixedUpdate()
	{
		if (instances.Count > 0)
		{
			float s = 0;
			foreach(ShakeInstance i in instances)
            {
				s += i.a;
				i.dTime();
            }

			foreach (ShakeInstance i in instances)
			{
				if (i.t < 0) instances.Remove(i);
			}

			s = Mathf.Min(s, maxShake);

			camTransform.localPosition = originalPos + Random.insideUnitSphere * s;
		}
		else
		{
			camTransform.localPosition = originalPos;
		}
	}

	public void Shake(float time = 1f, float strength = 0.7f)
    {
		instances.Add(new ShakeInstance(time, strength));
    }
	public void Shake(Vector3 pos, float radius, float time = 0.5f, float strength = 0.7f)
	{
		float m = Explosion.GetFalloff(Vector3.Distance(camTransform.position, pos), radius);
		instances.Add(new ShakeInstance(time, strength * m * 2f));
	}

	[System.Serializable]
	public class ShakeInstance
	{
		public float t;
		public float a;

		float ot;
		float at;

		public ShakeInstance(float _t, float _a)
		{
			t = _t;
			a = _a;

			ot = t;
			at = a;
		}

		public void dTime()
        {
			t = t - Time.fixedDeltaTime;
			a = Mathf.Lerp(0, at, t / ot);
        }
	}
}