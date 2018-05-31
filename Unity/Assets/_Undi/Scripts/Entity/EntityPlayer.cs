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
	private bool isPreparingAttack;
	public bool isDefending { get; private set; }

	//[SerializeField] private uint health;

	// Design
	[Header( "Attack & defense" )]
	[SerializeField] private float preparationInitDelay = 0.6f;
	[SerializeField] private float preparationAttackDelay = 0.2f;
	[SerializeField] private float preparationDefenseDelay = 1.4f;
	[SerializeField] private float preparationRecoveryDelay = 0.4f;
	[SerializeField] private GameObject particlesExplosion;

	[Header( "Physics" )]
	public float impactForce = 6f;
	public float velocityLimit = 60f;


	// Graphics
	[Header( "Objects" )]
	[SerializeField] private GameObject preparationInit;
	[SerializeField] private GameObject preparationAttack;
	[SerializeField] private GameObject preparationDefense;
	[SerializeField] private GameObject preparationRecovery;
	[SerializeField] private Material p1material;
	[SerializeField] private Material p2material;

	#endregion



	#region Class Management
	protected override void Initialize()
	{
		base.Initialize();

		// Rewired
		player = ReInput.players.GetPlayer( 0 );

		// Set init variables
		health = healthMax;
		canMove = true;
		isPreparingAttack = false;
		isDefending = false;

	}

	private void Update()
	{
		if( canMove )
		{
			CheckInput( id );
		}
	}

	private void CheckInput( int playerId )
	{
		if( playerId == 0 )
		{
			// Rewired
			moveVecPoll.x = player.GetAxis( "P1_MoveHorizontal" ); // get input by name or action id
			moveVecPoll.y = player.GetAxis( "P1_MoveVertical" );
			rigidbody.AddForce( moveVecPoll * runSpeed, ForceMode.Force );

			// Attack actions
			if( player.GetButtonDown( "P1_Attack" ) )
			{
				AttackPreparation();
			}

			if( isPreparingAttack && player.GetButtonUp( "P1_Attack" ) )
			{

				StartCoroutine( AttackPerform() );
				//AttackPerform();
			}

			// Defense actions
			if( player.GetButtonDown( "P1_Defense" ) )
			{
				DefenseBegin();
			}
		}
		else if( playerId == 1 )
		{
			// Rewired
			moveVecPoll.x = player.GetAxis( "P2_MoveHorizontal" ); // get input by name or action id
			moveVecPoll.y = player.GetAxis( "P2_MoveVertical" );
			rigidbody.AddForce( moveVecPoll * runSpeed, ForceMode.Force );

			// Attack actions
			if( player.GetButtonDown( "P2_Attack" ) )
			{
				AttackPreparation();
			}

			if( isPreparingAttack && player.GetButtonUp( "P2_Attack" ) )
			{

				StartCoroutine( AttackPerform() );
				//AttackPerform();
			}

			// Defense actions
			if( player.GetButtonDown( "P2_Defense" ) )
			{
				DefenseBegin();
			}
		}
	}

	public override void Set( int idP )
	{
		base.Set( idP );
		//Set name
		transform.name = "Player" + (id + 1).ToString();
		// Set material
		SetGraphics();
	}
	#endregion



	#region Health Management 
	private void Damage()
	{
	}
	#endregion





	#region Attack
	private void AttackPreparation()
	{
		// Prepare the attack
		//Debug.Log( "Player attack preparation " );
		SetActiveTry( preparationInit, true );
		isPreparingAttack = true;
	}

	private IEnumerator AttackPerform()
	{
		// Init
		//Debug.Log( "Player attack preparation " );
		//SetActiveTry( preparationInit, true );
		//yield return new WaitForSeconds( preparationInitDelay );

		// Attack
		isPreparingAttack = false;
		canMove = false;
		SetActiveTry( preparationInit, false );
		SetActiveTry( preparationAttack, true );
		//Debug.Log( "Player attack action " );
		yield return new WaitForSeconds( preparationAttackDelay );

		// Recovery 
		SetActiveTry( preparationAttack, false );
		SetActiveTry( preparationRecovery, true );
		//Debug.Log( "Player attack recovery " );
		yield return new WaitForSeconds( preparationRecoveryDelay );

		// Finish
		SetActiveTry( preparationRecovery, false );
		canMove = true;
		//Debug.Log( " + Player attack finish " );
	}
	#endregion


	#region Defense
	private void DefenseBegin()
	{
		//Debug.Log( "Player defense begin " );
		StartCoroutine( DefensePerform() );
	}

	private IEnumerator DefensePerform()
	{
		//Debug.Log( "Player defense preparation " );
		canMove = false;
		isDefending = true;
		SetActiveTry( preparationDefense, true );
		//Debug.Log( "Player defense action " );
		yield return new WaitForSeconds( preparationDefenseDelay );
		SetActiveTry( preparationDefense, false );
		isDefending = false;
		SetActiveTry( preparationRecovery, true );
		//Debug.Log( "Player defense recovery " );
		yield return new WaitForSeconds( preparationRecoveryDelay );
		SetActiveTry( preparationRecovery, false );
		canMove = true;
	}
	#endregion



	#region Collision
	void OnTriggerEnter( Collider col )
	{
		if( col.gameObject.CompareTag( "Attack" ) )
		{
			if( isDefending )
			{
				// If parry
				Debug.Log( gameObject.name + " parries " + col.transform.parent.name );
				Vector3 direction = CalculateDirection( transform.position, col.transform.position );
				col.transform.parent.GetComponent<Rigidbody>().AddForce( direction * impactForce, ForceMode.Impulse );
			}
			else
			{
				// This player was attacked
				Debug.Log( col.transform.parent.name + " attacks " + gameObject.name );

				// Apply physics
				Vector3 direction = CalculateDirection( col.transform.position, transform.position );
				rigidbody.AddForce( direction * impactForce, ForceMode.Impulse );
				
				// Damage and update UI
				health--;
				Director.Instance.managerUI.SetHealth( id, health );

				// Particles effect
				if( particlesExplosion != null )
				{
					var parts = Instantiate( particlesExplosion, transform.position, transform.rotation );
					parts.GetComponent<ParticleSystem>().Play();
				}

				// Camera effects
				var camShake = Director.Instance.managerCamera.cameras[0].GetComponent<TweenShake>();
				camShake.Play();
			}
			//var script = col.transform.parent.GetComponent<EntityPlayer>();
			//if( script != null )
			//{
			//	if( script.isDefending )
			//	{
			//		// If parry
			//		Debug.Log( " +++ Parry" );
			//	}
			//	else
			//	{
			//		Debug.Log( gameObject.name + " triggered by Attack of " + col.transform.parent.name );
			//		// Calculate direction
			//		Vector3 heading = col.transform.position - transform.position;
			//		float distance = -heading.magnitude;
			//		Vector3 direction = heading / distance;
			//		// Apply impulse
			//		rigidbody.AddForce( direction.normalized * impactForce, ForceMode.Impulse );
			//	}
			//}
		}
		else if( col.gameObject.CompareTag( "Defense" ) )
		{
			//Debug.Log( gameObject.name + " triggered by Defense of " + col.transform.parent.name );
		}
	}

	void OnCollisionEnter( Collision col )
	{
		//For example, when touching another entity
		if( col.gameObject.CompareTag( "Boundary" ) )
		{
			//Debug.Log( "Player health -- " );
			//Vector2 direction = -rigidbody.velocity.normalized;
			//rigidbody.AddForce( direction * impactForce, ForceMode.Impulse );
			//if( OnCollideWithEntity != null )
			//{
			//	OnCollideWithEntity();
			//}
		}
		else if( col.gameObject.CompareTag( "Player" ) )
		{
			//Debug.Log( "Player collided with player " );
			//rigidbody.AddForce(col.rigidbody.velocity.normalized * impactForce, ForceMode.Impulse);
		}
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

	private void SetGraphics()
	{
		switch( id )
		{
			default:
			case 0:
				if( p1material != null )
				{
					GetComponent<Renderer>().material = p1material;
				}
				break;

			case 1:
				if( p2material != null )
				{
					GetComponent<Renderer>().material = p2material;
				}
				break;
		}
	}

	private Vector3 CalculateDirection( Vector3 pointA, Vector3 pointB )
	{
		Vector3 heading = pointA - pointB;
		float distance = -heading.magnitude;
		Vector3 direction = heading / distance;
		return direction.normalized;
	}
	#endregion

}
