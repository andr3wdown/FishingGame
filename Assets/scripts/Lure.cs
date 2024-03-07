using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andr3wDown.Math;
using UnityEngine.Tilemaps;


public class Lure : MonoBehaviour
{
	public bool flying = false;
	float height = 0f;
	float minusHeight;

	float lineStrenght = 6f;
	float reelStrenght = 0f;

	Transform lureBody;
	Vector2 direction;
	Vector2 point;
	float magnitude;
	[HideInInspector]
	public float depth = 0;
	public GameObject splash;
	public Vector2 BodyPos
	{
		get
		{
			return lureBody.position;
		}
	}
	public GameObject shadow;
	public GameObject spl;

	public Fish currentFish;
	public GameObject fish;
	static BiteEvaluator eval;
	public void Init(Vector2 dir, float strenght, float line, float reel)
	{
		if(eval == null)
		{
			eval = FindObjectOfType<BiteEvaluator>();
		}
		lineStrenght = line;
		reelStrenght = reel;
		direction = dir;
		lureBody = transform.GetChild(0);
		Vector2 start = transform.position;

		direction = Vector2.Lerp(start, dir, strenght);
		magnitude = (start - direction).magnitude;
		height = 0.2f + magnitude / 5f;
		flying = true;
	}
	float v = 0;
	public AnimationCurve flightPath;
	Vector3Int prevTile;
	bool NewTile
	{
		get
		{
			Vector3Int gridPos = MapController.GetGridPos(transform.position);
			if (prevTile != gridPos)
			{
				prevTile = gridPos;
				return true;
			}
			return false;
		}
	}
	private void Update()
	{
		
		shadow.SetActive(flying);
		splash.SetActive(!flying);
		//print(MapController.GetDepth(transform.position));
		//print(MapController.GetTemp(transform.position, depth));
		if (flying)
		{
			transform.position = Vector2.MoveTowards(transform.position, direction, (3.5f * Time.deltaTime) + (magnitude * 0.7f * Time.deltaTime));
			float currentMagnitue = ((Vector2)transform.position - direction).magnitude;
			float ratio = currentMagnitue / magnitude;
			Vector2 desiredPos = (transform.position + Vector3.up * height * 2.5f * flightPath.Evaluate(ratio));
			lureBody.transform.position = desiredPos;
			Vector2 dirToDesired = (desiredPos - (Vector2)lureBody.position).normalized;
			Vector2 dirToDir = (direction - (Vector2)transform.position).normalized;
			Vector2 comb = (dirToDesired + dirToDir).normalized;
			transform.rotation = Quaternion.Slerp(transform.rotation, MathOperations.LookAt2D(transform.position, transform.position + (Vector3)comb, -90), 5f * Time.deltaTime);
			if (ratio <= 0)
			{
				lureBody.GetComponent<SpriteRenderer>().enabled = false;
				AudioController.PlayClip("splash", transform.position);
				flying = false;
				//GameObject go = Instantiate(fish, transform.position, Quaternion.identity);
				//currentFish = go.GetComponent<Fish>().Hook(this);
				Instantiate(spl, transform.position, Quaternion.identity);
			}
		}
		else
		{
			if (currentFish == null)
			{
				DepthAction();
				if (NewTile)
				{
					eval.EvaluateBite(this);
				}	
			}	
		}
	}
	public void FishOn(FishData data)
	{
		GameObject go = Instantiate(fish, transform.position, Quaternion.identity);
		currentFish = go.GetComponent<Fish>().Hook(this, data);
	}
	public void DepthAction()
	{
		depth += 0.8f * Time.deltaTime;
		float floor = MapController.GetDepth(transform.position);
		if (depth >= floor)
		{
			depth = floor;
		}
		if(depth < 0)
		{
			depth = 0;
		}
	}
	public float DepthRatio
	{
		get
		{
			return Mathf.Clamp(depth, 0f, MapController.Depth) / MapController.Depth;
		}
	}
	public bool FishPulling
	{
		get
		{
			if(currentFish == null)
			{
				return false;
			}
		
			Vector2 toBoat = ((Vector3)LureCaster.position - transform.position).normalized;
			return Vector2.Angle(currentFish.transform.up, toBoat) > 90;


		}
	}
	public float LastDst()
	{
		float mag = 0;
		if ((Vector2)transform.position != lastPos)
		{
			mag = ((Vector2)transform.position - lastPos).magnitude;
			speed = Mathf.Sqrt(mag) * 10;
			lastPos = transform.position;
		}
		//print(mag);
		mag = MathOperations.Map(mag, 0f, 0.045f, 0.8f, 1.2f);
		return mag;
	}
	Vector2 lastPos;
	Vector2 lureDir;
	public float speed;
	public float Pull()
	{
		Vector2 playerPos = LureCaster.position;
		Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 lureToBoat = (playerPos - (Vector2)transform.position).normalized;
		Vector2 lureToMouse = (mousepos - (Vector2)transform.position).normalized;
		Vector2 combined = (lureToBoat + lureToMouse * 0.9f);
		lureDir = (Vector3)combined * (mousepos - (Vector2)transform.position).magnitude * reelStrenght * Time.deltaTime;
		Vector2 desiredPosition = transform.position + (Vector3)lureDir;
		Vector2 move = Vector2.Lerp(transform.position, desiredPosition, 100f * Time.deltaTime);
		transform.position = move;
		float mag = 0;
		if((Vector2)transform.position != lastPos)
		{
			mag = ((Vector2)transform.position - lastPos).magnitude;
			speed = Mathf.Sqrt(mag) * 10;
			lastPos = transform.position;
		}
		float lureRise = MathOperations.Map(mag, 0f, 0.047f, 0f, 8f);
		mag = MathOperations.Map(mag, 0f, 0.045f, 0.8f, 1.2f);
		
		depth -= lureRise * Time.deltaTime;
		return mag;
	}
	Vector2 fishDesired;

	[HideInInspector]
	public float lineTension = 0;
	//[HideInInspector]

	public void FishPull(Vector2 desiredDirection, float strenght)
	{
		
		desiredDirection.Normalize();
		float dst = Vector2.Distance((Vector2)transform.position + desiredDirection * (0.6f + (0.7f * currentFish.data.size)) * strenght * Time.deltaTime, (Input.GetKey(KeyCode.Mouse0) ? (Vector2)transform.position + lureDir * 1f : (Vector2)transform.position));
		lineTension = (dst) * 100f  / lineStrenght;
		transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + desiredDirection, strenght * (1f /*- (0.4f * currentFish.data.size)*/) * Time.deltaTime);
	}
	public void DestroyLure()
	{
		if(currentFish != null)
		{
			Destroy(currentFish.gameObject);
		}
		Destroy(gameObject);
	}
	public void RetrieveLure()
	{
		if (currentFish != null)
		{
			MenuController.Open(currentFish.data);
			Destroy(currentFish.gameObject);
		}
		Destroy(gameObject);
	}
}
