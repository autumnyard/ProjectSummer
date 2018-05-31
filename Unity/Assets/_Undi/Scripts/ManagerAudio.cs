using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerAudio : MonoBehaviour
{

	[SerializeField] private AudioSource songGame;
	//[SerializeField] private AudioSource songMenu;
	[SerializeField] private AudioSource sfxExplosion01;
	[SerializeField] private AudioSource sfxExplosion02;
	[SerializeField] private AudioSource sfxExplosion03;
	[SerializeField] private AudioSource sfxStart;

	public enum Sfx
	{
		Explosion1 = 0,
		Explosion2,
		Explosion3,
		Start
	}

	void Awake()
	{
		Director.Instance.managerAudio = this;
	}


	public void PlaySongGame()
	{
		if( songGame.isPlaying )
		{
			songGame.UnPause();
		}
		else
		{
			songGame.Play();
		}
	}

	public void StopSongGame()
	{
		songGame.Pause();
	}

	//public void PlaySongMenu()
	//{
	//	songMenu.time = 0;
	//	songMenu.Play();
	//}

	//public void StopSongMenu()
	//{
	//	songMenu.Stop();
	//}

	public void PlayRandomExplosionSfx()
	{
		int rand = Random.Range( 0, 3 );
		switch( rand )
		{
			default:
			case 0:
				PlaySfx( (Sfx)rand );
				break;

			case 1:
				PlaySfx( (Sfx)rand );
				break;

			case 2:
				PlaySfx( (Sfx)rand );
				break;
		}
		//PlaySfx( (Sfx)rand );
	}

	public void PlaySfx( Sfx which )
	{
		switch( (int)which )
		{
			case (int)Sfx.Explosion1:
				PlaySfxExplosion1();
				break;

			case (int)Sfx.Explosion2:
				PlaySfxExplosion2();
				break;

			case (int)Sfx.Explosion3:
				PlaySfxExplosion3();
				break;

			case (int)Sfx.Start:
				PlaySfxStart();
				break;

			default:
				break;
		}
	}

	private void PlaySfxExplosion1()
	{
		if( sfxExplosion01 != null )
		{
			sfxExplosion01.Play();
		}
	}
	private void PlaySfxExplosion2()
	{
		if( sfxExplosion02 != null )
		{
			sfxExplosion02.Play();
		}
	}
	private void PlaySfxExplosion3()
	{
		if( sfxExplosion03 != null )
		{
			sfxExplosion03.Play();
		}
	}
	private void PlaySfxStart()
	{
		if( sfxStart != null )
		{
			sfxStart.Play();
		}
	}
}
