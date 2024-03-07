using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LureCaster : MonoBehaviour
{
	public GameObject barObject;
	public GameObject lure;
	public Transform castingPoint;
	public float maxCastPower = 5f;
	public float castLoadSpeed = 1f;
	bool goingDown = false;
	float power = 0;

	public Image bar;
	public Image depthBar;
	public Image bottomBar;
	Lure currentLure = null;
	bool waiting = false;
	CameraController controller;
	LineRenderer lr;

	public AudioSource reelingAudio;
	public AudioSource dragAudio;

	[Range(2f, 8f)]
	public float lineStrenght = 6f;
	[Range(0f, 0.6f)]
	public float reelStrenght = 0.28f;

	Vector2 reelingRange = new Vector2(0.95f,1.1f);

	static LureCaster caster;
	bool blockCasting = true;
	private void Start()
	{
		caster = this;
		lr = GetComponent<LineRenderer>();
		controller = FindObjectOfType<CameraController>();
	}
	public static Vector2 position
	{
		get
		{
			return caster.transform.position;
		}
	}
	public static bool LureOut
	{
		get
		{
			return caster.currentLure != null;
		}
	}
	float currentRatio = 0;
	float currentBottom = 0;
	float currentTension = 0f;

	bool modeInput;
	bool continuousInput;
	bool castInput;

	public void ChangeMode()
	{
		//modeInput = true;
		if (!LureOut)
		{
			blockCasting = !blockCasting;
		}
	}
	void Update()
    {
#if UNITY_EDITOR && !UNITY_ANDROID || UNITY_STANDALONE
		modeInput = Input.GetKeyDown(KeyCode.B);
		continuousInput = Input.GetKey(KeyCode.Mouse0);
		castInput = Input.GetKeyUp(KeyCode.Mouse0);
#endif

#if UNITY_ANDROID
		continuousInput = Input.touchCount > 0;
		if(continuousInput)
			castInput = Input.GetTouch(0).phase == TouchPhase.Ended;
#endif
		bar.transform.parent.gameObject.SetActive(currentLure == null && !blockCasting || currentLure != null && currentLure.currentFish != null && !blockCasting);
		depthBar.transform.parent.gameObject.SetActive(currentLure != null && LureOut && !currentLure.flying && currentLure.currentFish == null);
		//barObject.SetActive(!blockCasting);
		if (!LureOut)
		{
			currentRatio = 0;
			currentBottom = 0;
			if (modeInput)
			{
				blockCasting = !blockCasting;
			}
		}
		if (!waiting && !blockCasting)
		{
			if (currentLure == null)
			{


				reelingAudio.enabled = false;
				if (continuousInput)
				{
					PowerChange();
				}
				if (castInput)
				{
					CastLure();
					power = 0;
					bar.fillAmount = power / maxCastPower;
				}
			}
			else
			{
			
				currentTension = Mathf.Lerp(currentTension, currentLure.lineTension, 1f*Time.deltaTime);
				bar.color = Color.Lerp(Color.yellow, Color.red, currentTension);
				bar.fillAmount = currentTension;
				if (!MapController.IsOnWater(currentLure.transform.position))
				{
					StartCoroutine(Wait(DestroyLure));
					NotificationsController.AddNotification(currentLure.currentFish == null ? new Notification("You can only cast on water!") : new Notification("The fish escaped!"));
				}
				if (!currentLure.flying)
				{
					if (currentLure.currentFish != null)
					{
						//print(currentTension);
						if(currentTension >= 0.99f)
						{
							StartCoroutine(Wait(DestroyLure));
							NotificationsController.AddNotification(new Notification("The fish broke the line!"));
						}
					}
					if (Vector2.Distance(transform.position, currentLure.transform.position) < 0.4f)
					{
						StartCoroutine(Wait(RetrieveLure));
					}
					else
					{
						if (continuousInput)
						{			
							if (!currentLure.FishPulling)
							{
								float pitch = currentLure.Pull();
								reelingAudio.pitch = pitch;
								reelingAudio.enabled = true;
								dragAudio.enabled = false;
							}
							else
							{
								float pitch = currentLure.Pull();
								reelingAudio.enabled = false;
								dragAudio.pitch = pitch;
								dragAudio.enabled = true;
							}
							
							
						}
						else
						{
							if (currentLure.FishPulling)
							{
								dragAudio.pitch = currentLure.LastDst();
								dragAudio.enabled = true;				
							}
							else
							{
								dragAudio.pitch = currentLure.LastDst();
								dragAudio.enabled = false;
							}
							currentLure.speed = 0;
							reelingAudio.enabled = false;
						}
					}
				}
				currentRatio = Mathf.Lerp(currentRatio, currentLure.DepthRatio, 1f * Time.deltaTime);

				float desiredRatio = MapController.GetDepth(currentLure.transform.position) / MapController.Depth;
				currentBottom = Mathf.Lerp(currentBottom, desiredRatio, 1f * Time.deltaTime); 

				bottomBar.fillAmount = 1f - currentBottom;
				depthBar.fillAmount = currentRatio;

				Vector3[] points = new Vector3[2];
				points[0] = castingPoint.position;
				points[1] = currentLure.BodyPos;
				lr.positionCount = 2;
				lr.SetPositions(points);
			}
		}
		else
		{
			
			if (castInput)
			{
				waiting = false;
			}
		}
		modeInput = false;
		continuousInput = false;
		castInput = false;
    }
	delegate void WaitFunc();
	private void DestroyLure()
	{
		CameraController.ScreenShake();
		currentTension = 0;
		bar.color = Color.green;
		bar.fillAmount = 0;
		reelingAudio.enabled = false;
		dragAudio.enabled = false;
		currentLure.DestroyLure();
		currentLure = null;
		lr.positionCount = 0;
	}
	void RetrieveLure()
	{
		currentTension = 0;
		bar.color = Color.green;
		bar.fillAmount = 0;
		reelingAudio.enabled = false;
		dragAudio.enabled = false;
		currentLure.RetrieveLure();
		currentLure = null;
		lr.positionCount = 0;
	}
	
	IEnumerator Wait(WaitFunc func)
	{
		waiting = true;
		yield return null;
		func.Invoke();
	}
	void CastLure()
	{
		float castRatio = power / maxCastPower;
		if(castRatio > 0.5f)
		{
			AudioController.PlayClip("cast", transform.position);
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector2 direction = Vector2.Lerp(castingPoint.position, mousePos, castRatio);
			direction.Normalize();
			GameObject l = Instantiate(lure, castingPoint.position, Andr3wDown.Math.MathOperations.LookAt2D(castingPoint.position, castingPoint.position + Vector3.up, -90));
			currentLure = l.GetComponent<Lure>();
			currentLure.Init(mousePos, castRatio, lineStrenght, reelStrenght);
			Debug.DrawLine(castingPoint.position, direction, Color.red, 10f);
			controller.SetLure(currentLure);
		}
		
	}
	void PowerChange()
	{
		if (goingDown)
		{
			power -= (power * 4f + castLoadSpeed) * Time.deltaTime;
			if(power <= 0)
			{
				power = 0;
				goingDown = false;
			}
		}
		else
		{
			power += (power * 4f + castLoadSpeed) * Time.deltaTime;
			if (power >= maxCastPower)
			{
				power = maxCastPower;
				goingDown = true;
			}
		}
		bar.fillAmount = power / maxCastPower;
	}
	
}
