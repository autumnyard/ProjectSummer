using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class EntityPlayer : EntityBase
{
	#region Variables

	private enum CollisionType
	{
		Attack,
		Parry
	}
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
	[SerializeField] private GameObject particlesExplosionAttack;
	[SerializeField] private GameObject particlesExplosionParry;

	[Header( "Physics" )]
	public float impactForceAttack = 50f;
	public float impactForceParry = 110f;
	public float velocityLimit = 60f;


	// Graphics
	[Header( "Objects" )]
	[SerializeField] private GameObject preparationInit;
	[SerializeField] private GameObject preparationAttack;
	[SerializeField] private GameObject preparationDefense;
	[SerializeField] private GameObject preparationRecovery;
	[SerializeField] private Material p1material;
	[SerializeField] private Material p2material;

	// Internal management
	private Coroutine currentAction;
	#endregion



	#region Class Management
	protected override void Initialize()
	{
		base.Initialize();

		// Rewired
		player = ReInput.players.GetPlayer( 0 );

		// Set objects
		preparationInit.SetActive( false );
		preparationAttack.SetActive( false );
		preparationDefense.SetActive( false );
		preparationRecovery.SetActive( false );

		// Set init variables
		health = healthMax;
		canMove = true;
		isPreparingAttack = false;
		isDefending = false;

		currentAction = null;
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
			if( !isDefending && player.GetButtonDown( "P1_Attack" ) )
			{
				ActionAttackPreparation();
			}

			if( isPreparingAttack && player.GetButtonUp( "P1_Attack" ) )
			{

				currentAction = StartCoroutine( ActionAttack() );
				//AttackPerform();
			}

			// Defense actions
			if( !isPreparingAttack && player.GetButtonDown( "P1_Defense" ) )
			{
				currentAction = StartCoroutine( ActionDefense() );
			}
		}
		else if( playerId == 1 )
		{
			// Rewired
			moveVecPoll.x = player.GetAxis( "P2_MoveHorizontal" ); // get input by name or action id
			moveVecPoll.y = player.GetAxis( "P2_MoveVertical" );
			rigidbody.AddForce( moveVecPoll * runSpeed, ForceMode.Force );

			// Attack actions
			if( !isDefending && player.GetButtonDown( "P2_Attack" ) )
			{
				ActionAttackPreparation();
			}

			if( isPreparingAttack && player.GetButtonUp( "P2_Attack" ) )
			{

				currentAction = StartCoroutine( ActionAttack() );
				//AttackPerform();
			}

			// Defense actions
			if( !isPreparingAttack && player.GetButtonDown( "P2_Defense" ) )
			{
				currentAction = StartCoroutine( ActionDefense() );
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
	private void Damage( CollisionType type )
	{
		//Damage
		switch( type )
		{
			default:
			case CollisionType.Attack:
				health--;
				break;
			case CollisionType.Parry:
				health -= 2;
				break;
		}

		// Check endgame conditions
		if( health <= 0 )
		{
			if( OnDie != null )
			{
				OnDie();
			}

			// Increase score
			Director.Instance.managerGame.ScoreIncrease( id );
		}

		// Update UI
		Director.Instance.managerUI.SetHealth( id, health );
	}
	#endregion





	#region Actions
	// Attack preparation
	private void ActionAttackPreparation()
	{
		// Prepare the attack
		PabloTools.TryForNullSetActive( preparationInit, true );
		isPreparingAttack = true;
	}

	// Attack
	private IEnumerator ActionAttack()
	{
		isPreparingAttack = false;
		canMove = false;
		PabloTools.TryForNullSetActive( preparationInit, false );
		PabloTools.TryForNullSetActive( preparationAttack, true );
		yield return new WaitForSeconds( preparationAttackDelay );

		ActionRecoveryBegin();
	}

	// Defense
	private IEnumerator ActionDefense()
	{
		canMove = false;
		isDefending = true;
		PabloTools.TryForNullSetActive( preparationDefense, true );
		yield return new WaitForSeconds( preparationDefenseDelay );

		ActionRecoveryBegin();
	}

	// Recovery 
	private void ActionRecoveryBegin()
	{
		isDefending = false;
		PabloTools.TryForNullSetActive( preparationDefense, false );
		PabloTools.TryForNullSetActive( preparationAttack, false );

		currentAction = StartCoroutine( ActionRecoveryPerform() );
	}

	private IEnumerator ActionRecoveryPerform()
	{
		PabloTools.TryForNullSetActive( preparationRecovery, true );
		yield return new WaitForSeconds( preparationRecoveryDelay );

		ActionRecoveryEnd();
	}

	private void ActionRecoveryEnd()
	{
		// Finish
		PabloTools.TryForNullSetActive( preparationRecovery, false );
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
				//Debug.Log( gameObject.name + " parries " + col.transform.parent.name );

				// TODO: Damage the other player
				Vector3 direction = CalculateDirection( transform.position, col.transform.position );
				col.transform.parent.GetComponent<EntityPlayer>().ReceiveParry( direction );

				// Force stop the defense
				isDefending = false;
			}
			else
			{
				// This player was attacked
				//Debug.Log( col.transform.parent.name + " attacks " + gameObject.name );

				// Damage and check endgame condition
				Damage( CollisionType.Attack );

				// Apply physics
				Vector3 direction = CalculateDirection( col.transform.position, transform.position );
				rigidbody.AddForce( direction * impactForceAttack, ForceMode.Impulse );

				// Feedback
				CollisionFeedback( CollisionType.Attack );
			}
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
	public void ReceiveParry( Vector3 direction )
	{
		Debug.Log( gameObject.name + " has been parried " );
		GetComponent<Rigidbody>().AddForce( direction * impactForceParry, ForceMode.Impulse );

		Damage( CollisionType.Parry );

		// Force stop the current action
		isDefending = false;
		if( currentAction != null )
		{
			StopCoroutine( currentAction );
			currentAction = null;
		}
		ActionRecoveryBegin();

		// Feedback
		CollisionFeedback( CollisionType.Parry );
	}

	private void CollisionFeedback( CollisionType type )
	{
		// Camera effects
		var camShake = Director.Instance.managerCamera.cameras[0].GetComponent<TweenShake>();

		// Sound effects
		switch( type )
		{
			default:
			case CollisionType.Attack:
				PlayParticles( particlesExplosionAttack );
				Director.Instance.managerAudio.PlaySfx( ManagerAudio.Sfx.Explosion1 );
				camShake.time = 0.5f;
				camShake.strength = Vector3.one * 0.9f;
				break;

			case CollisionType.Parry:
				PlayParticles( particlesExplosionParry );
				Director.Instance.managerAudio.PlaySfx( ManagerAudio.Sfx.Explosion3 );
				camShake.time = 0.8f;
				camShake.strength = Vector3.one * 1.2f;
				break;
		}
		//Director.Instance.managerAudio.PlayRandomExplosionSfx();

		// Play camera effects
		camShake.Play();
	}

	private void PlayParticles( GameObject particlePrefab )
	{
		if( particlePrefab != null )
		{
			var parts = Instantiate( particlePrefab, transform.position, transform.rotation );
			parts.GetComponent<ParticleSystem>().Play();
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




	#region Tools

	#endregion

}
