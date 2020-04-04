using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
	public Rigidbody rb;
	public GameObject idlePosition;
	public GameObject attackPosition;
	public Animator anim;
	public float movementSpeed = 5;
	public float maxHealth = 100;
	private float currentHealth;
	public EnemyScript _enemyScript;
	public Image healthFill;
	public Text healthText;

	[Header("Action Bar Buttons")]
	public Button attackButton;
	public Button defendButton;
	public Button healButton;

	private void Start()
	{
		currentHealth = maxHealth;
		healthFill.fillAmount = 1;
		EnableActionBar(true);
		this.transform.LookAt(attackPosition.transform.position);
	}

	public void BeginTurn()
	{
		EnableActionBar(true);
	}

	public void Attack()
	{
		EnableActionBar(false);
		StartCoroutine(AttackCoroutine());
	}

	public void Defend()
	{
		EnableActionBar(false);
		BattleManager.instance.EndTurn();
	}

	public void Heal()
	{
		EnableActionBar(false);
		BattleManager.instance.EndTurn();
	}

	private void FixedUpdate()
	{
		anim.SetFloat("Run Blend", rb.velocity.magnitude);
	}

	public IEnumerator AttackCoroutine()
	{
		// Moves the player to the attack position
		while (Vector3.Distance(transform.position, attackPosition.transform.position) > 0.5f)
		{
			rb.velocity = transform.forward * movementSpeed;
			yield return null;
		}

		// Plays the attack animation
		var attackIndex = Random.Range(1, 4); // Range max is exclusive: returns 1-3
		RandomAttack(attackIndex);
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to hit
		switch (attackIndex)
		{
			case 1:
				_enemyScript.TakeDamage(5);
				break;
			case 2:
				_enemyScript.TakeDamage(10);
				break;
			case 3:
				_enemyScript.TakeDamage(15);
				break;
			default:
				break;
		}
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to end

		// Turns the player to look at the idle position
		this.transform.LookAt(idlePosition.transform.position);

		// Moves the player to the idle position
		while (Vector3.Distance(transform.position, idlePosition.transform.position) > 0.5f)
		{
			rb.velocity = transform.forward * movementSpeed;
			yield return null;
		}
		yield return new WaitForSeconds(0.1f); // Delay to allow the player to stop moving

		// Faces the player back at the enemy
		this.transform.LookAt(attackPosition.transform.position);

		// Ends the player's turn
		BattleManager.instance.EndTurn();
	}

	public void RandomAttack(int index)
	{
		switch (index)
		{
			case 1:
				anim.SetTrigger("Attack1");
				Debug.Log("Playing Attack 1");
				break;
			case 2:
				anim.SetTrigger("Attack2");
				Debug.Log("Playing Attack 2");
				break;
			case 3:
				anim.SetTrigger("Attack3");
				Debug.Log("Playing Attack 3");
				break;
			default:
				Debug.LogError("Attack index out of range");
				break;
		}
	}

	private void EnableActionBar(bool value)
	{
		attackButton.interactable = value;
		defendButton.interactable = value;
		healButton.interactable = value;
	}

	public void TakeDamage(int value)
	{
		currentHealth = Mathf.Clamp(currentHealth -= value, 0, maxHealth);
		healthFill.fillAmount = currentHealth / 100;
		healthText.text = $"HP: {currentHealth} / {maxHealth}";

		if (currentHealth == 0)
		{
			anim.SetTrigger("Dead");
			BattleManager.instance.GameOver();
		}
		else
		{
			anim.SetTrigger("Hit");
		}
	}
}
