using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
	public Rigidbody rb;
	public GameObject idlePosition;
	public GameObject attackPosition;
	public Animator anim;
	public float movementSpeed = 5;
	public float maxHealth;
	private float currentHealth;
	public PlayerScript _playerScript;
	public Image healthFill;
	public Text healthText;
	public int defence;

	private void Start()
	{
		currentHealth = maxHealth;
		healthFill.fillAmount = 1;
		this.transform.LookAt(attackPosition.transform.position);
	}

	public void BeginTurn()
	{
		// defence drains by 2 per turn
		defence = Mathf.Clamp(defence -= 2, 0, 15);

		if (currentHealth < 50 && Random.Range(0f,1f) <= 0.70f) // 70% chance to heal if low health
		{
			StartCoroutine(HealCoroutine());
		}
		else
		{
			if (defence <= 10 && Random.Range(0,1f) <= 0.4f) // 40% chance to block if defence is below 10 defence
			{
				StartCoroutine(DefendCoroutine());
			}
			else
			{
				StartCoroutine(AttackCoroutine());
			}
		}
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
		anim.SetTrigger("Defend"); // No heal animation, so using this
		yield return new WaitForSeconds(0.3f); // Wait for animation to play
		int healValue = Random.Range(10, 21); // 10-20
		currentHealth = Mathf.Clamp(currentHealth += healValue, 0, maxHealth);
		healthFill.fillAmount = currentHealth / 100;
		healthText.text = $"HP: {currentHealth} / {maxHealth}";
		yield return new WaitForSeconds(0.7f); // Wait for animation to end
		BattleManager.instance.EndTurn();
	}

	private void FixedUpdate()
	{
		anim.SetFloat("Run Blend", rb.velocity.magnitude);
	}

	public IEnumerator AttackCoroutine()
	{
		// Moves the enemy to the attack position
		while (Vector3.Distance(transform.position, attackPosition.transform.position) > 0.2f)
		{
			rb.velocity = transform.forward * movementSpeed;
			yield return null;
		}

		// Plays the attack animation
		RandomAttackAnimation();
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to hit
		_playerScript.TakeDamage(Random.Range(12, 25) - _playerScript.defence); // 12-24 damage
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to end

		// Turns the enemy to look at the idle position
		this.transform.LookAt(idlePosition.transform.position);

		// Moves the enemy to the idle position
		while (Vector3.Distance(transform.position, idlePosition.transform.position) > 0.2f)
		{
			rb.velocity = transform.forward * movementSpeed;
			yield return null;
		}
		yield return new WaitForSeconds(0.1f); // Delay to allow the enemy to stop moving

		// Faces the enemy back at the player
		this.transform.LookAt(attackPosition.transform.position);

		// Ends the enemy's turn
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
