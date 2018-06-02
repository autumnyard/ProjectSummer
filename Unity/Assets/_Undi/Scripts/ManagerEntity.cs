﻿using UnityEngine;


public class ManagerEntity : MonoBehaviour
{
	#region Variables
	// Players
	const int maxPlayers = 2;
	[SerializeField] private GameObject prefabPlayer;
	[SerializeField] private GameObject[] players = new GameObject[maxPlayers];
	[HideInInspector] public EntityPlayer[] playersScript = new EntityPlayer[maxPlayers];

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

		players = new GameObject[maxPlayers];
		playersScript = new EntityPlayer[maxPlayers];

	}
	#endregion


	#region Entity Management
	public bool SummonPlayers()
	{
		Vector2 initPos1 = Vector2.zero;
		if( player1InitPosition != null )
		{
			initPos1 = player1InitPosition.position;
		}
		bool p1 = SummonPlayer( 0, initPos1 );

		Vector2 initPos2 = Vector2.zero;
		if( player2InitPosition != null )
		{
			initPos2 = player2InitPosition.position;
		}
		bool p2 = SummonPlayer( 1, initPos2 );

		return p1 | p2;
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
		for( int i = 0; i < maxPlayers; i++ )
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
