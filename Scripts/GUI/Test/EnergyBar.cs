using UnityEngine;
using System.Collections;

public class EnergyBar : MonoBehaviour {

	public EnergyManager energy;
	public Transform ForegroundSprite;
	public SpriteRenderer ForegroundRenderer;
    public TextMesh pointsText;


	public Color MaxEnergyColor = new Color (255 / 255f, 63 / 255f, 63 / 255f);
	public Color MinEnergyColor = new Color(64/ 255f, 137/255f, 255/255f);

	public void ChangeBarApperance (float energyValue)
	{
		var energyPercent = energyValue / energy.maxEnergyVolume;
		
		ForegroundSprite.localScale = new Vector3 (energyPercent, 1, 1);
		ForegroundRenderer.color = Color.Lerp (MaxEnergyColor, MinEnergyColor, energyPercent);
	}

    private void ChangePointsApperance (float points)
    {
        pointsText.text = string.Format("Points: {0}", (int)points);
    }

	public void Start ()
	{
		EnergyManager.onEnergyChanged += ChangeBarApperance;
        EnergyManager.onPointsChanged += ChangePointsApperance;
	}
	public void OnDestroy ()
	{
		EnergyManager.onEnergyChanged -= ChangeBarApperance;
        EnergyManager.onPointsChanged += ChangePointsApperance;
	}

}
