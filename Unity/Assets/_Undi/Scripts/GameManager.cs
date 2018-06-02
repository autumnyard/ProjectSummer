using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


public class GameManager : MonoBehaviour
{
	#region Variables
	// Game management
	[SerializeField] private int scoreP1;
	[SerializeField] private int scoreP2;
	[SerializeField] private int mapCurrent;
	private Structs.GameMode mode;

	// Input
	private Keyboard keyboard;
	private int keyEnter;
	private int keySpacebar;
	private int keyEscape;

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

	private void LateUpdate()
	{
		CheckGlobalInput();
	}
	#endregion

	#region Game Management
	public void Initialize()
	{
		mapCurrent = 0;
		ScoreReset();

		keyboard = ReInput.controllers.Keyboard;
		keyEnter = keyboard.GetButtonIndexByKeyCode( KeyCode.KeypadEnter );
		keySpacebar = keyboard.GetButtonIndexByKeyCode( KeyCode.Space );
		keyEscape = keyboard.GetButtonIndexByKeyCode( KeyCode.Escape );
	}
	#endregion


	#region Global input management
	private void CheckGlobalInput()
	{
		switch( Director.Instance.currentScene )
		{
			case Structs.GameScene.Menu:
				//if( ReInput.players.GetPlayer( 0 ).GetButtonDown( "Start" ) )
				//if( keyboard.GetButtonDownById( keyEnter ) || keyboard.GetButtonDownById( keySpacebar ) )
				//{
				//	Director.Instance.GameBegin();
				//}
				break;

			case Structs.GameScene.Ingame:
				if( keyboard.GetButtonDownById( keyEscape ) )
				{
					Director.Instance.GameEnd();
				}
				break;

			default:
			case Structs.GameScene.Splash:
			case Structs.GameScene.Initialization:
			case Structs.GameScene.LoadingGame:
			case Structs.GameScene.GameReset:
			case Structs.GameScene.GameEnd:
			case Structs.GameScene.Score:
			case Structs.GameScene.Credits:
			case Structs.GameScene.Exit:
				break;
		}

	}
	#endregion


	#region UI button management
	public void ButtonPlay2Players()
	{
		Debug.Log( "asdfasdfsd" );
		mode = Structs.GameMode.Mode2Players;

		Director.Instance.GameBegin();
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

	private void ScoreReset()
	{
		scoreP1 = 0;
		scoreP2 = 0;
		Director.Instance.managerUI.SetScore( 0, scoreP1 );
		Director.Instance.managerUI.SetScore( 1, scoreP2 );
	}
	#endregion
}
