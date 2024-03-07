using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsController : MonoBehaviour
{
	public Text text;
	public Animator anim;
	static NotificationsController controller;
	Queue<Notification> notifications = new Queue<Notification>();
	bool notifying = false;
	private void Start()
	{
		controller = this;
		AddNotification(new Notification("Hello and welcome!"));
		AddNotification(new Notification("Cast by holding down the left mouse button!"));
		AddNotification(new Notification("Hold the left mouse button to reel! the farther your mouse is from the lure the faster you will reel!"));
		AddNotification(new Notification("Press B to toggle boat mode!"));
	}
	public static void AddNotification(Notification n)
	{
		controller.notifications.Enqueue(n);
	}
	void DisplayNext()
	{
		Notification n = notifications.Dequeue();
		notifying = true;
		text.text = n.message;
		StartCoroutine(Display(n.duration));
	}
	IEnumerator Display(float delay)
	{
		yield return new WaitForSeconds(delay);
		notifying = false;
	}
	private void Update()
	{
		if(!notifying && notifications.Count > 0)
		{
			DisplayNext();
		}
		anim.SetBool("Notification", notifying);
	}
}
public struct Notification
{
	public string message;
	public float duration;
	public Notification(string m, float d=4f)
	{
		message = m;
		duration = d;
	}
}