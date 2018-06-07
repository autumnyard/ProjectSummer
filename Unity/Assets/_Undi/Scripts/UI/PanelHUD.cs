using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHUD : PanelBase
{

	//[SerializeField] private Text p1health;
	//[SerializeField] private Text p2health;
	//[SerializeField] private Text p3health;
	//[SerializeField] private Text p4health;
	//[SerializeField] private Text p1score;
	//[SerializeField] private Text p2score;
	[SerializeField] private Text[] healths;
	[SerializeField] private Text[] scores;

	public void Set()
	{
		switch( Director.Instance.currentGameMode )
		{
			default:
			case Structs.GameMode.Mode2Players:
				TextActivate(healths[2], false);
				TextActivate(healths[3], false);
				TextActivate(scores[2], false);
				TextActivate(scores[3], false);
				break;
			case Structs.GameMode.Mode3Players:
				TextActivate(healths[2], true);
				TextActivate(healths[3], false);
				TextActivate(scores[2], true);
				TextActivate(scores[3], false);
				break;
			case Structs.GameMode.Mode4Players:
				TextActivate(healths[2], true);
				TextActivate(healths[3], true);
				TextActivate(scores[2], true);
				TextActivate(scores[3], true);
				break;
		}
	}

	private void TextActivate( Text text, bool activate )
	{
		text.gameObject.SetActive( activate );
	}

	public void SetHealth( int player, int to )
	{
		if( to < 0 )
		{
			to = 0;
		}

		if( healths[player] != null )
		{
			healths[player].text = to.ToString();
		}
		else
		{
			Debug.LogError( "Trying to change health to inexistent player: " + player );
		}

		/*
		switch( player )
		{
			case 0:
				p1health.text = to.ToString();
				break;

			case 1:
				p2health.text = to.ToString();
				break;

			default:
				Debug.LogError("Trying to change health to inexistent player.");
				break;
		}
		*/
	}

	public void SetScore( int player, int to )
	{
		if( to < 0 )
		{
			to = 0;
		}

		if( scores[player] != null )
		{
			scores[player].text = to.ToString();
		}
		else
		{
			Debug.LogError( "Trying to change score to inexistent player: " + player );
		}

		//switch( player )
		//{
		//	case 0:
		//		p1score.text = to.ToString();
		//		break;

		//	case 1:
		//		p2score.text = to.ToString();
		//		break;

		//	default:
		//		Debug.LogError( "Trying to change score to inexistent player." );
		//		break;
		//}
	}
}
