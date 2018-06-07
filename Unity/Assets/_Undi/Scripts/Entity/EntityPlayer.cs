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

	public enum PlayerState
	{
		NotPlaying,
		Alive,
		Dead,
		MaxValues
	}

	// Rewired
	private Player player; // The Rewired Player
	private Vector2 moveVecPoll;

	// Management
	[Header( "Main" )]
	public PlayerState state;
	private bool canMove;
	private bool isPreparingAttack;
	public bool isDefending { get; private set; }
	private bool invulnerable;
	private float invulnerabilityTime = 0.8f;

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

	[Header( "Prefabs" )]
	[SerializeField] private Material[] materials;
	//[SerializeField] private Material p2material;

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
		invulnerable = false;

		currentAction = null;

		state = PlayerState.Alive;
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
		// Stop current action, defense or attack
		ForceActionEnd();

		// Make invulnerable
		StartCoroutine( Invulnerability() );

		//Damage
		switch( type )
		{
			default:
			case CollisionType.Attack:
				Debug.Log( " - Damage Attack" );
				health--;
				break;
			case CollisionType.Parry:
				Debug.Log( " + Damage Parry" );
				health -= 2;
				break;
		}

		// Check endgame conditions
		if( health <= 0 )
		{
			// If this player has died
			state = PlayerState.Dead;

			// Callbacks
			if( OnDie != null )
			{
				OnDie();
			}

			// Increase score
			//Director.Instance.managerGame.ScoreIncrease( id );
		}
		else
		{
			// Update UI
			Director.Instance.managerUI.SetHealth( id, health );
		}

	}

	private IEnumerator Invulnerability()
	{
		invulnerable = true;
		yield return new WaitForSeconds( invulnerabilityTime );
		invulnerable = false;
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
		//Debug.Log( " x Action recovery " );
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


	private void ForceActionEnd()
	{
		isDefending = false;
		if( currentAction != null )
		{
			StopCoroutine( currentAction );
			currentAction = null;
		}
		ActionRecoveryBegin();
	}
	#endregion



	#region Collision
	void OnTriggerEnter( Collider col )
	{
		if( col.gameObject.CompareTag( "Attack" ) )
		{
			if( isDefending ) // If parry
			{
				//Debug.Log( gameObject.name + " parries " + col.transform.parent.name );

				// Damage the other player
				Vector3 direction = CalculateDirection( transform.position, col.transform.position );
				col.transform.parent.GetComponent<EntityPlayer>().ReceiveParry( direction );

				// Force stop the defense
				//isDefending = false;
				ForceActionEnd();
			}
			else if( !invulnerable ) // This player was attacked
			{

				//Debug.Log( col.transform.parent.name + " attacks " + gameObject.name );
				Vector3 direction = CalculateDirection( col.transform.position, transform.position );
				ReceiveAttack( direction );
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
		//Debug.Log( gameObject.name + " has been parried " );

		// Apply physics
		GetComponent<Rigidbody>().AddForce( direction * impactForceParry, ForceMode.Impulse );

		// Hurt player and force stop actions
		Damage( CollisionType.Parry );

		// Feedback
		CollisionUIFeedback( CollisionType.Parry );
	}

	private void ReceiveAttack( Vector3 direction )
	{

		// Damage and check endgame condition
		Damage( CollisionType.Attack );

		// Apply physics
		rigidbody.AddForce( direction * impactForceAttack, ForceMode.Impulse );

		// Feedback
		CollisionUIFeedback( CollisionType.Attack );
	}

	private void CollisionUIFeedback( CollisionType type )
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

		if( materials[id] != null )
		{
			GetComponent<Renderer>().material = materials[id];
		}

		//switch( id )
		//{
		//	default:
		//	case 0:
		//		if( p1material != null )
		//		{
		//			GetComponent<Renderer>().material = p1material;
		//		}
		//		break;

		//	case 1:
		//		if( p2material != null )
		//		{
		//			GetComponent<Renderer>().material = p2material;
		//		}
		//		break;
		//}
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
