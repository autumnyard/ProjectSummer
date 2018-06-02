using UnityEngine;


public class ManagerEntity : MonoBehaviour
{
	#region Variables
	// Players
	private int numPlayers = 2;
	[SerializeField] private GameObject prefabPlayer;
	[SerializeField] private GameObject[] players;
	[HideInInspector] public EntityPlayer[] playersScript;

	// TODO: Enemy management in a dynamic list
	// TODO: Inanimate entities, like objects, in a dynamic list
	// TODO: Pickup entities, like items, in a dynamic list
	[SerializeField] private Transform player1InitPosition;
	[SerializeField] private Transform player2InitPosition;
	[SerializeField] private Transform player3InitPosition;
	[SerializeField] private Transform player4InitPosition;
	[SerializeField] private Transform[] playersInitPositions;

	#endregion


	#region Monobehaviour
	void Awake()
	{
		Director.Instance.managerEntity = this;

		//players = new GameObject[numPlayers];
		//playersScript = new EntityPlayer[numPlayers];

	}
	#endregion


	#region Entity Management
	public bool SummonPlayers()
	{
		// Set number of players
		if( Director.Instance.currentGameMode == Structs.GameMode.Mode2Players )
		{
			numPlayers = 2;
			
			players = new GameObject[numPlayers];
			playersScript = new EntityPlayer[numPlayers];
		}

		// Load and summon them
		bool wentRight = true;

		for( int i = 0; i < numPlayers; i++ )
		{
			Vector2 initPos = Vector2.zero;
			if( playersInitPositions[i] != null )
			{
				initPos = playersInitPositions[i].position;
			}
			wentRight |= SummonPlayer( i, initPos );
		}

		return wentRight;
	}

	public bool SummonPlayer( int which, Vector2 position )
	{
		if( !PabloTools.TryForNullDebugError( prefabPlayer, "There's no prefab for the player " + which.ToString() ) )
		{
			return false;
		}

		// Instantiate
		players[which] = Instantiate( prefabPlayer, position, Quaternion.identity, transform ) as GameObject;

		if( !PabloTools.TryForNullDebugError( players[which], "Cannot instantiate player " + which.ToString() ) )
		{
			return false;
		}

		// Get player script
		playersScript[which] = players[which].GetComponent<EntityPlayer>();

		if( playersScript[which] == null )
		{
			Debug.LogError( "Cannot find player script in player " + which );
			return false;
		}

		// And now set it
		playersScript[which].Set( which );
		return true;
	}


	private void RemoveAllPlayers()
	{
		for( int i = 0; i < numPlayers; i++ )
		{
			RemovePlayer( i );
		}
	}

	private void RemovePlayer( int which )
	{
		if( players[which] != null )
		{
			Destroy( players[which] );
			playersScript[which] = null;
			players[which] = null;
		}
	}
	#endregion



	#region General management
	public void Reset()
	{
		RemoveAllPlayers();
	}
	#endregion
}
