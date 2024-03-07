using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Andr3wDown.Math;

public class DriveBoat : MonoBehaviour
{
	public Image gasBar;
	public float maxSpeed;
	public float speed;
	public float turnRate;
	float currentZ = 0;
	public static Rigidbody2D rb;
	public bool boatActive = true;
	AudioSource motorSounds;
	public float minPitch;
	public float maxPitch;

	public GameObject[] driveButtons;

	private void Start()
	{
		motorSounds = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody2D>();
	}
	bool modeInput;
	bool driveInput;
	bool leftInput;
	bool rightInput;
	public void ChangeMode()
	{
		if (!LureCaster.LureOut)
		{
			boatActive = !boatActive;
		}
	}

	public void TurnInput(float value)
	{
		if(value < 0)
		{
			leftInput = true;
		}
		if(value > 0)
		{
			rightInput = true;
		}
	}
	public void EndTurnInput(float value)
	{
		if (value < 0)
		{
			leftInput = false;
		}
		if (value > 0)
		{
			rightInput = false;
		}
	}

	public void DriveInput()
	{
		driveInput = true;
	}
	public void EndDrive()
	{
		driveInput = false;
	}
	public void AddGas(float amount)
	{
		speed += amount;
		speed = Mathf.Clamp(speed, 0, maxSpeed);
	}
	private void Update()
	{
#if UNITY_EDITOR && !UNITY_ANDROID || UNITY_STANDALONE
		modeInput = Input.GetKeyDown(KeyCode.B);
		driveInput = Input.GetKey(KeyCode.Space);
#endif
		gasBar.transform.parent.gameObject.SetActive(boatActive);
		motorSounds.enabled = boatActive;
		if (boatActive)
		{
			if (driveInput)
			{
				Drive();
			}

			Turn();
			speed = Mathf.Clamp(speed + Input.GetAxisRaw("Vertical") * Time.deltaTime, 0f, maxSpeed);
			gasBar.fillAmount = speed / maxSpeed;
			//print(rb.velocity.magnitude);
			motorSounds.pitch = MathOperations.Map(rb.velocity.magnitude, 0, 5.7f, minPitch, maxPitch);
		}
		if (!LureCaster.LureOut && modeInput)
		{
			boatActive = !boatActive;
		}

#if UNITY_EDITOR || UNITY_ANDROID
		foreach (GameObject g in driveButtons)
		{
			g.SetActive(boatActive);
		}
#endif
		//modeInput = false;
		//driveInput = false;
	}
	void Drive()
	{
		rb.AddForce(transform.right * speed);
		//transform.position += transform.right * speed * Time.deltaTime;
	}
	void Turn()
	{
#if UNITY_EDITOR && !UNITY_ANDROID || UNITY_STANDALONE
		leftInput = Input.GetKey(KeyCode.A);
		rightInput = Input.GetKey(KeyCode.D);
#endif
		if (leftInput)
		{
			currentZ -= turnRate * Time.deltaTime;
		}
		if (rightInput)
		{
			currentZ += turnRate * Time.deltaTime;
		}
		transform.rotation = Quaternion.Euler(0, 0, currentZ);
		//leftInput = false;
		//rightInput = false;
	}
}
