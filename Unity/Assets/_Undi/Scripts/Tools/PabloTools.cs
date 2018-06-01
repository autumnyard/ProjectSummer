using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PabloTools : MonoBehaviour
{
	#region Common helpers with checks for null
	public static bool TryForNullDebugError( GameObject go, string text )
	{
		if( go == null )
		{
			Debug.LogError( text );
			return false;
		}
		return true;
	}

	public static void TryForNullSetActive( GameObject go, bool to )
	{
		if( go != null )
		{
			go.SetActive( to );
		}
	}
	#endregion



	#region Binding

	public static void Bind( ref Constants.Method to, Constants.Method method )
	{
		Unbind( ref to );
		to += method;
	}

	public static void Unbind( ref Constants.Method from )
	{
		if( from != null )
		{
			from = null;
		}
	}

	public static void UnbindAll( ref Constants.Method[] from )
	{
		for( int i = 0; i < from.Length; i++ )
		{
			Unbind( ref from[i] );
		}
	}
	#endregion
}
