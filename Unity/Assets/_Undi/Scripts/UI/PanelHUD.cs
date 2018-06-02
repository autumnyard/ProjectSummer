using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHUD : PanelBase
{

	[SerializeField] private Text p1health;
	[SerializeField] private Text p2health;
	[SerializeField] private Text p3health;
	[SerializeField] private Text p4health;
	[SerializeField] private Text[] healths;
	[SerializeField] private Text p1score;
	[SerializeField] private Text p2score;

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
		switch( player )
		{
			case 0:
				p1score.text = to.ToString();
				break;

			case 1:
				p2score.text = to.ToString();
				break;

			default:
				Debug.LogError( "Trying to change score to inexistent player." );
				break;
		}
	}
}
