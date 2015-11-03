using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_Controller : MonoBehaviour {

    //public GameObject energyIcon;

    //void Start () 
    //{
    //    EnergyManager.onEnergyChanged += OnEnergyChanged;
    //}

    //public void OnDestroy ()
    //{
    //    EnergyManager.onEnergyChanged -= OnEnergyChanged;
    //}

    //private void OnEnergyChanged ( float energyValue) 
    //{

    //}


//	private IEnumerator AbilityCooldownTimer()
//	{
//		do
//		{	
//			abilityIcon.renderer.material.SetFloat("_Cutoff", ToRange (currentAbility.CurrentCDTime, 0.0f, 1.0f, 0.0f, currentAbility.cooldownTime));
//			yield return null;
//		} while( !currentAbility.IsReady || !currentAbility.IsFreeze);
//
//	}

//	public float textFadeOutTime = 0.2f;
//	public float textAppearTime = 1.0f;
//
//	private IEnumerator ShowText(GameObject textObject)
//	{
//		yield return new WaitForSeconds(textAppearTime);
//		StartCoroutine(FadeOutText(textObject));
//	}
//	
//	private IEnumerator FadeOutText(GameObject textObject)
//	{
//		float alphaState = 1.0f;
//		List<TextMesh> tmpTextMesh = new List<TextMesh>();
//		
//		if(textObject != null)
//		{
//			tmpTextMesh = new List<TextMesh>(textObject.GetComponentsInChildren<TextMesh>());
//		}
//		else
//		{
//			return false;
//		}
//		
//		float time = 0.0f;
//		
//		while(alphaState > 0.01f)
//		{
//			time += Time.deltaTime * (1.0f / textFadeOutTime);
//			
//			for(int i = 0; i < tmpTextMesh.Count; i++)
//			{
//				if(tmpTextMesh[i] != null)
//				{
//					Color tmpColor = tmpTextMesh[i].color;
//					tmpColor.a = Mathf.Lerp(1.0f, 0.0f, Mathf.Sin(time * Mathf.PI * 0.5f));
//					tmpTextMesh[i].color = tmpColor;
//					
//					alphaState = tmpColor.a;
//				}
//				else
//				{
//					return false;
//				}
//			}
//			
//			yield return null;
//		}
//		
//		Destroy(textObject);
//	}
}
