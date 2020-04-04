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
	public int defence;

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
		// defence drains by 2 per turn
		defence = Mathf.Clamp(defence -= 2, 0, 15);
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
		StartCoroutine(DefendCoroutine());
	}

	public void Heal()
	{
		EnableActionBar(false);
		StartCoroutine(HealCoroutine());
	}

	private void FixedUpdate()
	{
		anim.SetFloat("Run Blend", rb.velocity.magnitude);
	}

	public IEnumerator AttackCoroutine()
	{
		// Moves the player to the attack position
		while (Vector3.Distance(transform.position, attackPosition.transform.position) > 0.2f)
		{
			rb.velocity = transform.forward * movementSpeed;
			yield return null;
		}

		// Plays the attack animation
		RandomAttackAnimation();
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to hit
		_enemyScript.TakeDamage(Random.Range(12,25) - _enemyScript.defence); // 12-24 damage
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to end

		// Turns the player to look at the idle position
		this.transform.LookAt(idlePosition.transform.position);

		// Moves the player to the idle position
		while (Vector3.Distance(transform.position, idlePosition.transform.position) > 0.2f)
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

	IEnumerator DefendCoroutine()
	{
		anim.SetTrigger("Defend");
		yield return new WaitForSeconds(1f); // Wait for animation to play
		// Add 5 defence, capped at 15
		defence = Mathf.Clamp(defence += 5, 0, 15);
		BattleManager.instance.EndTurn();
	}

	IEnumerator HealCoroutine()
	{
		anim.SetTrigger("Defend");  // No heal animation, so using this
		yield return new WaitForSeconds(0.3f); // Wait for animation to play
		int healValue = Random.Range(10, 21); // 10-20
		currentHealth = Mathf.Clamp(currentHealth += healValue, 0, maxHealth);
		healthFill.fillAmount = currentHealth / 100;
		healthText.text = $"HP: {currentHealth} / {maxHealth}";
		yield return new WaitForSeconds(0.7f); // Wait for animation to end
		BattleManager.instance.EndTurn();
	}

	public void RandomAttackAnimation()
	{
		var attackIndex = Random.Range(1, 4); // Range max is exclusive: returns 1-3
		switch (attackIndex)
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
