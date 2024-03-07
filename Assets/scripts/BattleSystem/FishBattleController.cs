using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FishBattleController : MonoBehaviour
{
	public FishBattleDNA dna;
	public FishCurrentPhase phase;
	public Texture2D fishTexture;

	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private Timer timer;

	private float accelerationTime = 0.075f;
	private float currentSpeed;
	private float targetSpeed = 5;

	private int[] calculatedStats;

	//placeholder variables
	private FishCurrentPhase[] phaseList = { FishCurrentPhase.Idling, FishCurrentPhase.Moving, FishCurrentPhase.Attacking, FishCurrentPhase.Moving };
	private int phaseIndex = 0;
	private Vector2 targetPosition;
	public Transform target;
	public GameObject impactEffect;


	private HPObject hpObject;
	private int attack;
	
	private void Start()
	{
		hpObject = GetComponent<HPObject>();
		rb = GetComponent<Rigidbody2D>();
		sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
		if(fishTexture == null)
		{
			fishTexture = sr.sprite.texture;
		}
		dna = new FishBattleDNA("test fish", _level: 5);

		calculatedStats = BattleStatCalculator.CalculateStats(dna.level, dna);

		fishTexture = FishColorReplacer.ReplaceFishColors(fishTexture, dna.colors);
		Sprite fishSprite = Sprite.Create(fishTexture, new Rect(0, 0, fishTexture.width, fishTexture.height), Vector2.one * 0.5f, 32);
		sr.sprite = fishSprite;

		hpObject.Init(calculatedStats[0]);
		attack = calculatedStats[1];
		timer = new Timer(0.5f);

		//temp
		int l = Random.Range(0, 4);
		for (int i = 0; i < l; i++)
		{
			IncrementPhaseIndex();
		}
	}

	private void Update()
	{
		switch (phase)
		{
			case FishCurrentPhase.Idling:
				SlowDown();
				bool end = timer.CountDown(true, true);
				if (end)
				{
					IncrementPhaseIndex();
				}
				break;
			case FishCurrentPhase.Moving:
				MoveToTarget(targetPosition);
				if(Vector2.Distance(transform.position, targetPosition) < 0.3f)
				{
					IncrementPhaseIndex();
				}
				break;
			case FishCurrentPhase.Attacking:
				MoveToTarget(target.position);
				if (Vector2.Distance(transform.position, target.position) < 0.6f)
				{
					if(!Attack()) IncrementPhaseIndex();
				}
				break;
			case FishCurrentPhase.Ability:

				break;
			case FishCurrentPhase.Dead:
				rb.velocity = Vector2.up * 2f;
				break;
			case FishCurrentPhase.Win:

				break;
			default:
				Debug.LogError("invalid phase");
				break;
		}
		OrientationControl();
	}

	private void MoveToTarget(Vector2 _targetPosition)
	{
		float vel = rb.velocity.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref vel, accelerationTime);
		Vector2 dir = (_targetPosition - (Vector2)transform.position).normalized;
		rb.velocity = dir * currentSpeed;
	}

	private void SlowDown()
	{
		float vel = rb.velocity.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, 0f, ref vel, accelerationTime * 0.75f);
		Vector2 dir = rb.velocity.normalized;
		rb.velocity = dir * currentSpeed;
	}

	private bool Attack()
	{
		GameObject go = Instantiate(impactEffect, target.position, Quaternion.identity);
		if (target.GetComponent<HPObject>().Damage(attack))
		{
			Win();
			return true;
		}
		return false;
		//go.transform.SetParent(target);
	}

	int previousIndex;
	private void IncrementPhaseIndex()
	{
		phaseIndex = Random.Range(0, 3);
		//phaseIndex++;
		if (phaseIndex >= phaseList.Length)
		{
			phaseIndex = 0;
		}
		//Debug.Log($"{phaseList[phaseIndex]}");
		phase = phaseList[phaseIndex];
		currentSpeed = (previousIndex == 0) ? 0 : targetSpeed * 0.4f;
		//rb.velocity = Vector2.zero;
		switch (phase)
		{
			case FishCurrentPhase.Idling:
				break;
			case FishCurrentPhase.Moving:
				targetPosition = ArenaController.GetRandomArenaPosition();
				break;
			case FishCurrentPhase.Attacking:
				break;
			case FishCurrentPhase.Ability:
				break;
			default:
				Debug.LogError("invalid phase");
				break;
		}
		previousIndex = phaseIndex;
	}

	private void Win()
	{
		phase = FishCurrentPhase.Win;
		rb.velocity = Vector2.zero;
	}

	public void Death()
	{
		phase = FishCurrentPhase.Dead;
		rb.velocity = Vector2.zero;
		sr.flipY = true;
	}


	private void OrientationControl()
	{
		if(rb.velocity.x > 0)
		{
			sr.flipX = false;
		}
		if(rb.velocity.x < 0)
		{
			sr.flipX = true;
		}
	}
}

public enum FishCurrentPhase
{
	Idling = 0,
	Moving = 1,
	Attacking = 2,
	Ability = 3,
	Dead = 4,
	Win = 5
}
