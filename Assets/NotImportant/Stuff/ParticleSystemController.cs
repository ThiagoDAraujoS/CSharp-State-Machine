using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemController : MonoBehaviour 
{
	[SerializeField]
	private ParticleSystem[] systemList = null;

	public void StartParticles()
	{
		foreach (ParticleSystem pS in systemList) 
		{
			pS.Play();
		}
	}

	public void StopParticles()
	{
		foreach (ParticleSystem pS in systemList) 
		{
			pS.Stop();
		}
	}
}
