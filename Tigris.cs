using GrindFest.BotApi;
using UnityEngine;
using System;

public enum HeroState
{
	DirectControl,
	Navigating,
	Farming,
	GoHome,
	Test
}

public abstract class State
{
	public abstract void OnEnterState(Tigris hero);
	public abstract void UpdateState(Tigris hero);
	public abstract void OnExitState(Tigris hero);
}

public class DirectControlState : State
{
	public override void OnEnterState(Tigris hero)
	{
		hero.Say("Entering Direct Control State");
	}

	public override void UpdateState(Tigris hero)
	{
		// Logic for direct control
	}

	public override void OnExitState(Tigris hero)
	{
		hero.Say("Exiting Direct Control State");
	}
}

public class NavigatingState : State
{
	public override void OnEnterState(Tigris hero)
	{
		hero.Say("Entering Navigating State");
		hero.GoToArea("Stony Plains");
	}

	public override void UpdateState(Tigris hero)
	{
		if (hero.CurrentArea?.Name == "Stony Plains") {
			hero.ChangeState(HeroState.Farming);
		}
	}

	public override void OnExitState(Tigris hero)
	{
		hero.Say("Exiting Navigating State");
	}
}

public class FarmingState : State
{
	float PotionHealthThreshold = 0.6f;
	public override void OnEnterState(Tigris hero)
	{
		hero.Say("Entering Farming State");
	}

	public override void UpdateState(Tigris hero)
	{
		hero.Say("Updating Farming State");
		var nearestEnemy = hero.FindNearestEnemy(5);
		if (nearestEnemy != null && hero.Health < hero.MaxHealth * PotionHealthThreshold) {
			hero.Say("Health below potion health " + PotionHealthThreshold * 100 + "% , drinking potion");
			hero.DrinkHealthPotion();
			hero.RunAwayFromNearestEnemy(6);
			return;
		}

		if (!hero.AttackNearestEnemy(5)) {
			var nearestItem = hero.FindNearestItem();
			if (nearestItem != null && nearestItem?.name != "Fireworks") {
				hero.Say("Picked up: " + nearestItem?.name);
				hero.PickUp(nearestItem);
			}
			else {
				hero.RunAroundInArea();
			}
		}
	}

	public override void OnExitState(Tigris hero)
	{
		hero.Say("Exiting Farming State");
	}
}

public class GoHomeState : State
{
	Vector3 homePosition = new Vector3(-191, 11, 569);
	public override void OnEnterState(Tigris hero)
	{
		hero.Say("Going Home");
		hero.GoTo(homePosition);
	}

	public override void UpdateState(Tigris hero)
	{
		hero.Say("UpdateState Going Home");
		hero.Say(hero.CurrentArea?.Name);
		if (hero.CurrentArea?.Name == "Burned down Settlement") {
			hero.ChangeState(HeroState.DirectControl);
		}
	}

	public override void OnExitState(Tigris hero)
	{
		hero.Say("Exited Going Home");
	}
}

public class Tigris : AutomaticHero
{
	private State currentState;
	private HeroState currentHeroState;

	private readonly DirectControlState directControlState = new DirectControlState();
	private readonly NavigatingState navigatingState = new NavigatingState();
	private readonly FarmingState farmingState = new FarmingState();
	private readonly GoHomeState goHomeState = new GoHomeState();

	private Rect buttonRect = new Rect(10, 10, 200, 200);

	public void Start()
	{
		ChangeState(HeroState.Farming);
	}

	public void Update()
	{
		if (IsBotting && currentState != null) {
			currentState.UpdateState(this);
		}
	}

	public void ChangeState(HeroState newState)
	{
		currentState?.OnExitState(this);
		currentHeroState = newState;
		switch (newState) {
			case HeroState.DirectControl:
				currentState = directControlState;
				break;
			case HeroState.Navigating:
				currentState = navigatingState;
				break;
			case HeroState.Farming:
				currentState = farmingState;
				break;
			case HeroState.GoHome:
				currentState = goHomeState;
				break;
		}
		currentState?.OnEnterState(this);
	}

	void OnGUI()
	{
		buttonRect = GUILayout.Window(0, buttonRect, WindowFunction, "State Control");
	}

	void WindowFunction(int id)
	{
		GUILayout.Label("Change State");

		if (GUILayout.Button("Direct Control")) {
			ChangeState(HeroState.DirectControl);
		}
		if (GUILayout.Button("Navigating")) {
			ChangeState(HeroState.Navigating);
		}
		if (GUILayout.Button("Farming")) {
			ChangeState(HeroState.Farming);
		}
		if (GUILayout.Button("Go Home")) {
			ChangeState(HeroState.GoHome);
		}

		GUI.DragWindow();
	}
}

public class MyParty : AutomaticParty
{
	private bool m_AllDead = false;

	void Update()
	{
		if (m_AllDead) {
			CreateHero("Tigris", "Hero");
			m_AllDead = false;
		}
	}

	public override void OnAllHeroesDied()
	{
		m_AllDead = true;
	}
}
