using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
	[Header( "Components" ), SerializeField] private PanelHUD panelHUD;
	[SerializeField] private PanelBase panelMenu;
	//   [SerializeField]
	//   private PanelBase panelLoading;
	//[SerializeField]
	//private PanelBase panelDebug;

	// Panel HUD
	//[Header( "Ingame HUD" ), SerializeField] private UnityEngine.UI.Text health;
	//[SerializeField] private UnityEngine.UI.Text mana;
	//[SerializeField] private UnityEngine.UI.Text score;
	//[SerializeField] private UnityEngine.UI.Text enemycount;

	void Awake()
	{
		Director.Instance.managerUI = this;
	}

	private void Update()
	{
		if( Director.Instance.currentScene == Structs.GameScene.Ingame )
		{

		}
	}

	#region Panel management
	public void SetPanels()
	{
		switch( Director.Instance.currentScene )
		{
			case Structs.GameScene.Menu:
				panelHUD.Hide();
				panelMenu.Show();
				break;

			default:
			case Structs.GameScene.Ingame:
				panelMenu.Hide();
				panelHUD.Set();
				panelHUD.Show();
				break;

		}
	}
	#endregion

	#region Ingame HUD management
	public void SetHealth( int player, int health )
	{
		panelHUD.SetHealth( player, health );
	}

	public void SetScore( int player, int score )
	{
		panelHUD.SetScore( player, score );
	}
	#endregion
}
