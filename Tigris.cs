using GrindFest.BotApi;
using UnityEngine;
public class Tigris : AutomaticHero
{
	public void Update()
	{
		if (IsBotting) {
			if (CurrentArea?.Name != "Stony Plains") {
				GoToArea("Stony Plains");
			}
			else {

				bool hasHealthPotion = HasHealthPotion();

				var nearestEnemy = FindNearestEnemy(5);

				// // if i have bellow 60% health, run away
				if (hasHealthPotion && nearestEnemy != null && Health < MaxHealth * 0.6) {
					DrinkHealthPotion();
					RunAwayFromNearestEnemy(6);
					return;
				}


				bool attacking = AttackNearestEnemy(5);

				if (!attacking) {
					if (hasHealthPotion) {
						if (Health < MaxHealth) {
							DrinkHealthPotion();
							return;
						}
					}

					var nearestItem = FindNearestItem();
					if (HealthPotionCount() < 5) {
						nearestItem = FindNearestItem("Vial of Health");
					}

					if (nearestItem == null) {
						nearestItem = FindNearestItem("Gold Coins");
					}

					if (nearestItem != null) {
						PickUp(nearestItem);
						return;
					}

					RunAroundInArea();
				}
			}
		}
	}

	//Navigation
	void CurrentAreaName()
	{
		//MainPathBehaviour.FindNearestWaypoint(this.transform.position);
		Say(CurrentArea.name);


	}

	void GoHome()
	{
		GoToArea("Burning Campfire");
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
