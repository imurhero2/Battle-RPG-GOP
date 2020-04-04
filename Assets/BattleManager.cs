using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
	public enum GameState
	{
		PlayerTurn,
		EnemyTurn,
		GameOver
	}

	public static BattleManager instance;
	public PlayerScript _playerScript;
	public EnemyScript _enemyScript;
	public GameState _gameState;

	private void Awake()
	{
		instance = this;
		_gameState = GameState.PlayerTurn;
		_playerScript.BeginTurn();
	}

	public void EndTurn()
	{
		if (_gameState == GameState.PlayerTurn)
		{
			_gameState = GameState.EnemyTurn;
			_enemyScript.BeginTurn();
		}
		else if (_gameState == GameState.EnemyTurn)
		{
			_gameState = GameState.PlayerTurn;
			_playerScript.BeginTurn();
		}
	}

	public void GameOver()
	{
		_gameState = GameState.GameOver;
	}
}
