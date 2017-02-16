using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectParticleSeeking : MonoBehaviour 
{
	/// <summary>
	/// 
	///  	The explicit particle lifespan is actually the Duration field in the Inspector on the particle prefab itself.
	/// 	The following particleLifespan variable will determine the lifespan you [the designer] want. 
	/// 	However, if the particleLifespan below surpasses the 60 seconds that I have set, then you will have to update the 
	/// 	Duration field mentioned earlier.
	/// 
	/// </summary>
	public float particleLifespan;
	public float particleSpeed; // For editing the speed of the particles

	private ParticleSystem particleSystem; // For storing the particle system
	private ParticleSystem.Particle[] particles; // Array for storing the particles being created by the particle system
	[SerializeField]
	private GameObject particleAttractor; // Gameobject for storing the object that will attract the particles
	[SerializeField]
	private ParticleSystem psPrefab; // For storing the particle prefab
	private Transform targetTf; // For storing the target object's transform
	private bool seeking = false;
	[SerializeField]
	UnityEvent onStartParticles;
	[SerializeField]
	UnityEvent onStopParticles;

	void Start()
	{
		/// TEST Finds my placeholder Player reference in the scene
		particleAttractor = (GameObject.Find ("HeroForm"));

		targetTf = particleAttractor.GetComponent<Transform> ();
	}

	public void PerformParticleSeeking () 
	{
		if (!seeking) 
		{
			// this.targetTf is this class' variable while the targetTf is the paramater above
			this.targetTf = targetTf; 

			StartCoroutine (SendParticles (targetTf)); // Kick off the coroutine
			seeking = true; // Coroutine is running
		}
	}

	private IEnumerator SendParticles(Transform targetPos)
	{
		bool running = true;

		// Instantiate the particle system at the [this] object's location. 
		particleSystem = Instantiate (psPrefab, this.transform.position, this.transform.rotation) as ParticleSystem;

		// Set array size equal to the total amount of particles being created
		particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

		onStartParticles.Invoke (); // Invoke an event once the particle system starts emitting

		while (running) 
		{
			// Variable for storing all of the particles being created
			int numParticlesAlive = particleSystem.GetParticles (particles);

			// For each particle in the array
			for (int i = 0; i < numParticlesAlive; i++) 
			{
				// Lerp the particle from its source to [this] object
				particles [i].position = Vector3.Lerp (particles [i].position, targetPos.position, Time.deltaTime * particleSpeed);
			}
			// Fill the particles array with particles that have been created
			particleSystem.SetParticles (particles, numParticlesAlive);

			// For Debugging
//			 Debug.Log (particleSystem.time);

			// Stop the particle system once it reaches its lifespan (-1 because particleSystem.time doesn't always reach the exact value)
			// The -1 also cuts the emitter off at just the right time to provide a cool effect
			if (particleSystem.time >= (particleLifespan - 1)) 
			{
				particleSystem.Stop ();
				onStopParticles.Invoke (); // Invoke an event when the particle system stops emitting
				running = false; // Set running to false to escape this hell
			}
			yield return new WaitForEndOfFrame ();
		}
		seeking = false; // Coroutine is no longer running
	}
}