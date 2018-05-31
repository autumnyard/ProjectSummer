using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Variables
	private int scoreP1;
	private int scoreP2;
	private int mapCurrent;
	#endregion

	#region Monobehaviour
	void Awake()
	{
		Director.Instance.managerGame = this;
	}

	private void Start()
	{
		Director.Instance.EverythingBeginsHere();
	}
	#endregion

	#region Game Management
	public void Initialize()
	{
		mapCurrent = 0;
		ScoreReset();
	}
	#endregion


	#region Score
	public void ScoreIncrease( int player )
	{
		switch( player )
		{
			case 0:
				scoreP1++;
				Director.Instance.managerUI.SetScore( player, scoreP1 );
				break;

			case 1:
				scoreP2++;
				Director.Instance.managerUI.SetScore( player, scoreP2 );
				break;

			default:
				Debug.LogError( "Trying to change score to inexistent player." );
				break;
		}
	}

	public void ScoreReset()
	{
		scoreP1 = 0;
		scoreP2 = 0;
		Director.Instance.managerUI.SetScore( 0, scoreP1 );
		Director.Instance.managerUI.SetScore( 1, scoreP2 );
	}
	#endregion
}
