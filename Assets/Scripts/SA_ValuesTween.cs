using System;
using System.Runtime.CompilerServices;
using StansAssets.Animation;
using UnityEngine;

public class SA_ValuesTween : MonoBehaviour
{
	public bool DestoryGameObjectOnComplete = true;

	private float FinalFloatValue;

	private Vector3 FinalVectorValue;

	[method: MethodImpl(32)]
	public event Action OnComplete = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<float> OnValueChanged = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Action<Vector3> OnVectorValueChanged = delegate
	{
	};

	public static SA_ValuesTween Create()
	{
		return new GameObject("SA_ValuesTween").AddComponent<SA_ValuesTween>();
	}

	private void Update()
	{
		this.OnValueChanged(base.transform.position.x);
		this.OnVectorValueChanged(base.transform.position);
	}

	public void ValueTo(float from, float to, float time, SA_EaseType easeType = SA_EaseType.linear)
	{
		Vector3 position = base.transform.position;
		position.x = from;
		base.transform.position = position;
		FinalFloatValue = to;
		SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("x", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
	}

	public void VectorTo(Vector3 from, Vector3 to, float time, SA_EaseType easeType = SA_EaseType.linear)
	{
		base.transform.position = from;
		FinalVectorValue = to;
		SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("position", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
	}

	public void VectorToS(Vector3 from, Vector3 to, float speed, SA_EaseType easeType = SA_EaseType.linear)
	{
		base.transform.position = from;
		FinalVectorValue = to;
		SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("position", to, "speed", speed, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
	}

	public void Stop()
	{
		SA_iTween.Stop(base.gameObject);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void onTweenComplete()
	{
		this.OnValueChanged(FinalFloatValue);
		this.OnVectorValueChanged(FinalVectorValue);
		this.OnComplete();
		if (DestoryGameObjectOnComplete)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
