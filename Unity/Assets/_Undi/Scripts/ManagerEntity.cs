using UnityEngine;


public class ManagerEntity : MonoBehaviour
{
    #region Variables
    // Players
    const int maxPlayers = 2;
    [SerializeField] private GameObject prefabPlayer;
	[SerializeField] private GameObject[] players = new GameObject[maxPlayers];
    [HideInInspector] public EntityBase[] playersScript = new EntityBase[maxPlayers];

    // TODO: Enemy management in a dynamic list
    // TODO: Inanimate entities, like objects, in a dynamic list
    // TODO: Pickup entities, like items, in a dynamic list
    [SerializeField] private Transform player1InitPosition;
    [SerializeField] private Transform player2InitPosition;

    #endregion


    #region Monobehaviour
    void Awake()
    {
        Director.Instance.managerEntity = this;

		players = new GameObject[maxPlayers];
		playersScript = new EntityBase[maxPlayers];

	}
    #endregion


    #region Entity Management
    public void SummonPlayers()
    {
		Vector2 initPos1 = Vector2.zero;
		if( player1InitPosition != null )
		{
			initPos1 = player1InitPosition.position;
		}
        SummonPlayer(0, initPos1);
		
		Vector2 initPos2 = Vector2.zero;
		if( player2InitPosition != null )
		{
			initPos2 = player2InitPosition.position;
		}
        SummonPlayer(1, initPos2);
    }

    public void SummonPlayer(int which, Vector2 position)
    {
        // Instantiate
        players[which] = Instantiate(prefabPlayer, position, Quaternion.identity, transform) as GameObject;
        if (players[which] == null)
        {
            Debug.LogError("Cannot instantiate player " + which);
            return;
        }

        // Get player script
        playersScript[which] = players[which].GetComponent<EntityBase>();
        if ( playersScript[which] == null)
        {
            Debug.LogError("Cannot find player script in player " + which);
            return;
        }

        // And now set it
        playersScript[which].Set( which );
    }


    private void RemoveAllPlayers()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            RemovePlayer(i);
        }
    }

    private void RemovePlayer(int which)
    {
        if (players[which] != null )
        {
            Destroy(players[which]);
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
