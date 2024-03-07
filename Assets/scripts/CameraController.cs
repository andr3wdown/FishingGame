using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public CameraMode mode = CameraMode.boat;
	public Transform player;
	public Lure lure;
	public float cameraSpeed;
	float cameraZ = -10;

	float currentShake = 0;
	float maxShake = 0.5f;
	[Range(0f, 0.5f)]
	public float shakeIntensity = 0.2f;
	public float recoverySpeed;
	public float shakeSpeed;
	static CameraController cc;
	float vel;
	private void Start()
	{
		cc = this;
	}
	void FixedUpdate()
	{
		currentShake = Mathf.SmoothDamp(currentShake, 0f, ref vel, recoverySpeed);
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 7f - currentShake, shakeSpeed * Time.deltaTime);
		if (lure == null)
		{
			mode = CameraMode.boat;
		}
		switch (mode)
		{
			case CameraMode.boat:
				transform.position = Vector3.Lerp(transform.position, player.position + new Vector3(0, 0, cameraZ), cameraSpeed * Time.fixedDeltaTime);
				break;
			case CameraMode.lure:
				transform.position = Vector3.Lerp(transform.position, lure.transform.position + new Vector3(0, 0, cameraZ), cameraSpeed * Time.fixedDeltaTime);
				break;
		}
	}
	public void SetLure(Lure l)
	{
		lure = l;
		mode = CameraMode.lure;
	}
	public static void ScreenShake()
	{
		cc.currentShake += cc.shakeIntensity;
		cc.currentShake = Mathf.Clamp(cc.currentShake, 0f, cc.maxShake);
	}
}
public enum CameraMode
{
	lure,
	boat
}