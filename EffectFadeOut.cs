using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFadeOut : MonoBehaviour 
{
	[SerializeField]
	float fadeDuration = 3.0f;
	[SerializeField]
	bool startFade = false;

	void Start () 
	{
		if (startFade) {
			PerformFadeOut ();
		}
	}

	void OnValidate() {
		if (startFade) {
			PerformFadeOut ();
		}
	}

	public void PerformFadeOut() {
		StartCoroutine (FadeOutEffect (this.transform, 0, true, fadeDuration));
	}

	// MATERIAL ON THE OBJECT MUST BE SET TO TRANSPARENT
	private IEnumerator FadeOutEffect (Transform tf, float targetAlpha, bool isVanish, float duration)
	{
		// Renderer variable for accessing our object's Renderer component.
		Renderer renderer = tf.GetComponent<Renderer> ();

		// Float variable for storing the difference in the object's alpha values
		float diffAlpha = (targetAlpha - renderer.material.color.a);

		float counter = 0; // Counter for keeping track of deltaTime

		// While the counter is less than the desired duration
		while (counter < duration) 
		{
			// Variable for storing the object's alpha amount & difference in alpha divided by the duration
			float alphaAmount = renderer.material.color.a + (Time.deltaTime * diffAlpha) / duration;

			// Set the object's renderer to the new amounts
			renderer.material.color = new Color (renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaAmount);

			counter += Time.deltaTime; // Keep track of deltaTime that is passing
			yield return new WaitForEndOfFrame();
//			yield return null; // Alternate return variable (ignore for now)
		}
		// Set the object's renderer to the resulting amounts (this portrays the fading effect since it is updating the values)
		renderer.material.color = new Color (renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, targetAlpha);

		// Destroy the object as it vanishes
		if (isVanish) 
		{
//			renderer.transform.gameObject.SetActive (false); // Alternate method that just disables the object
			Destroy(renderer.transform.gameObject);
		}
	}
}