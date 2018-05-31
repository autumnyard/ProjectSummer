using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerUI : MonoBehaviour
{
	[Header( "Components" ), SerializeField]
	private PanelHUD panelHUD;
	//   [SerializeField]
	//   private PanelBase panelMenu;
	//   [SerializeField]
	//   private PanelBase panelLoading;
	//[SerializeField]
	//private PanelBase panelDebug;

	// Panel HUD
	[Header( "Ingame HUD" ), SerializeField] private UnityEngine.UI.Text health;
	[SerializeField] private UnityEngine.UI.Text mana;
	[SerializeField] private UnityEngine.UI.Text score;
	[SerializeField] private UnityEngine.UI.Text enemycount;

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
			//        case Structs.GameScene.Menu:
			//            panelHUD.Hide();
			//            panelMenu.Show();
			//            panelLoading.Hide();
			//            panelDebug.Hide();
			//break;

			//         case Structs.GameScene.Ingame:
			//             panelHUD.Show();
			//             panelMenu.Hide();
			//             panelLoading.Hide();
			//             panelDebug.Show();
			//             break;

			//case Structs.GameScene.LoadingGame:
			//             panelMenu.Hide();
			//             panelHUD.Hide();
			//             panelLoading.Show();
			//             panelDebug.Hide();
			//             break;

			default:
				//panelMenu.Hide();
				panelHUD.Show();
				//panelLoading.Hide();
				//panelDebug.Hide();
				break;
		}
	}
	#endregion

	#region Inagem HUD management
	public void SetHealth( int player, int health )
	{
		panelHUD.SetHealth( player, health );
		/*
        if (newHealth < 0)
        {
            health.text = "Health: --";
        }
        else
        {
            health.text = "Health: " + newHealth.ToString("00");
        }
		*/
	}
	#endregion
}
