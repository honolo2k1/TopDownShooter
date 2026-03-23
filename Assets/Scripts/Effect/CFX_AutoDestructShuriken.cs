using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

// Cartoon FX  - (c) 2015 Jean Moreno

// Automatically destructs an object when it has stopped emitting particles and when they have all disappeared from the screen.
// Check is performed every 0.5 seconds to not query the particle system's state every frame.
// (only deactivates the object if the OnlyDeactivate flag is set, automatically used with CFX Spawn System)

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	// If true, deactivate the object instead of destroying it
	public bool OnlyDeactivate;
	
	void OnEnable()
	{
		CheckIfAlive().Forget();
	}
	
	private async UniTaskVoid CheckIfAlive()
	{
		ParticleSystem ps = this.GetComponent<ParticleSystem>();
		var ct = this.GetCancellationTokenOnDestroy();

		while (ps != null)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);
			if (!ps.IsAlive(true))
			{
				if (OnlyDeactivate)
				{
					this.gameObject.SetActive(false);
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
