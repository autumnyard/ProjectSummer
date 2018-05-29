using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class EntityPlayer : EntityBase
{
	#region Variables
	// Rewired
	private Player player; // The Rewired Player
	private Vector2 moveVecPoll;

	// Management
	private bool canMove;

	// Design
	[Header("Attack & defense")]
	[SerializeField] private float preparationInitDelay = 0.6f;
	[SerializeField] private float preparationAttackDelay = 0.2f;
	[SerializeField] private float preparationDefenseDelay = 0.1f;
	[SerializeField] private float preparationRecoveryDelay = 0.4f;


	// Graphics
	[Header("Objects")]
	[SerializeField] private GameObject preparationInit;
	[SerializeField] private GameObject preparationAttack;
	[SerializeField] private GameObject preparationDefense;
	[SerializeField] private GameObject preparationRecovery;

	#endregion




	protected override void Initialize()
	{
		base.Initialize();

		// Rewired
		player = ReInput.players.GetPlayer( 0 );

		canMove = true;
	}

	private void Update()
	{
		if( canMove )
		{
			// Rewired
			moveVecPoll.x = player.GetAxis( "MoveHorizontal" ); // get input by name or action id
			moveVecPoll.y = player.GetAxis( "MoveVertical" );
			rigidbody.AddForce( moveVecPoll * runSpeed, ForceMode.Force );

			if( player.GetButtonDown( "Attack" ) )
			{
				AttackBegin();
			}
			
			if( player.GetButtonDown( "Defense" ) )
			{
				DefenseBegin();
			}
		}
	}

	#region Attack
	private void AttackBegin()
	{
		Debug.Log( " + Player attack begin " );
		StartCoroutine( AttackPreparation() );
	}

	private IEnumerator AttackPreparation()
	{
		// Init
		Debug.Log( "Player attack preparation " );
		SetActiveTry( preparationInit, true );
		yield return new WaitForSeconds( preparationInitDelay );

		// Attack
		canMove = false;
		SetActiveTry( preparationInit, false );
		SetActiveTry( preparationAttack, true );
		Debug.Log( "Player attack action " );
		yield return new WaitForSeconds( preparationAttackDelay );

		// Recovery 
		SetActiveTry( preparationAttack, false );
		SetActiveTry( preparationRecovery, true );
		Debug.Log( "Player attack recovery " );
		yield return new WaitForSeconds( preparationRecoveryDelay );

		// Finish
		SetActiveTry( preparationRecovery, false );
		canMove = true;
		AttackFinish();
	}

	private void AttackFinish()
	{
		Debug.Log( " + Player attack finish " );
	}

	#endregion

	#region Defense
	private void DefenseBegin()
	{
		Debug.Log( "Player defense begin " );
		StartCoroutine( DefensePreparation() );
	}

	private IEnumerator DefensePreparation()
	{
		canMove = false;
		Debug.Log( "Player defense preparation " );
		SetActiveTry( preparationDefense, true );
		Debug.Log( "Player defense action " );
		yield return new WaitForSeconds( preparationDefenseDelay );
		SetActiveTry( preparationDefense, false );
		SetActiveTry( preparationRecovery, true );
		Debug.Log( "Player defense recovery " );
		yield return new WaitForSeconds( preparationRecoveryDelay );
		SetActiveTry( preparationRecovery, false );
		canMove = true;
		DefenseFinish();
	}

	private void DefenseFinish()
	{
		Debug.Log( "Player defense finish " );
	}
	#endregion




	#region Helpers
	private void SetActiveTry( GameObject go, bool to )
	{
		if( go != null )
		{
			go.SetActive( to );
		}
	}
	#endregion

}
