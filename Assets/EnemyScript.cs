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

	private void Start()
	{
		currentHealth = maxHealth;
		healthFill.fillAmount = 1;
		this.transform.LookAt(attackPosition.transform.position);
	}

	public void BeginTurn()
	{
		// choose to attack, defend, or heal
		Attack();
	}

	public void Attack()
	{
		StartCoroutine(AttackCoroutine());
	}

	public void Defend()
	{

	}

	public void Heal()
	{

	}

	private void FixedUpdate()
	{
		anim.SetFloat("Run Blend", rb.velocity.magnitude);
	}

	public IEnumerator AttackCoroutine()
	{
		// Moves the enemy to the attack position
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
				_playerScript.TakeDamage(5);
				break;
			case 2:
				_playerScript.TakeDamage(10);
				break;
			case 3:
				_playerScript.TakeDamage(15);
				break;
			default:
				break;
		}
		yield return new WaitForSeconds(0.6f); // Wait for attack animation to end

		// Turns the enemy to look at the idle position
		this.transform.LookAt(idlePosition.transform.position);

		// Moves the enemy to the idle position
		while (Vector3.Distance(transform.position, idlePosition.transform.position) > 0.1f)
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
