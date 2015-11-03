using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_EnergyBar : MonoBehaviour {

    public EnergyManager energy;
    public Image sliderForeground;
   
    public Text pointsText;
    public Text energyText;

    private Slider slider;

    public Color MaxEnergyColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinEnergyColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);


    public void ChangeBarApperance(float energyValue)
    {
        var energyPercent = energyValue / energy.maxEnergyVolume;

        energyText.text = string.Format("{0} / {1}", (int)energyValue, energy.maxEnergyVolume);
        slider.value = energyPercent;
        sliderForeground.color = Color.Lerp(MaxEnergyColor, MinEnergyColor, energyPercent);
    }

    private void ChangePointsApperance(float points)
    {
        if (pointsText != null) 
        {
            pointsText.text = string.Format("Points: {0}", (int)points);
        }
        
    }

    public void Start()
    {
        slider = GetComponent<Slider>();

        EnergyManager.onEnergyChanged += ChangeBarApperance;
        EnergyManager.onPointsChanged += ChangePointsApperance;
    }
    public void OnDestroy()
    {
        EnergyManager.onEnergyChanged -= ChangeBarApperance;
        EnergyManager.onPointsChanged += ChangePointsApperance;
    }
}
