using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    [Header("Health Bar Elements")]
    public GameObject maxHealthBar;
    public GameObject currentHealthBar;
    public GameObject healthText;

    private float currHPWidth = 1;
    private float currHPTargetWidth = 1;

    private void Start()
    {
        float maxHPWidth = maxHealthBar.GetComponent<RectTransform>().sizeDelta.x;
        RectTransform textTransf = healthText.GetComponent<RectTransform>();
        textTransf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (maxHPWidth * 0.9f));
        textTransf.position = new Vector3(maxHealthBar.GetComponent<RectTransform>().position.x + (maxHPWidth * 0.05f), textTransf.position.y, textTransf.position.z);
    }

    private void Update()
    {
        currHPWidth = Mathf.Lerp(currHPWidth, currHPTargetWidth, Time.deltaTime * 5.0f);
        currentHealthBar.transform.localScale = new Vector3(currHPWidth, currentHealthBar.transform.localScale.y, currentHealthBar.transform.localScale.z);
    }

    public void initHPBar(float maxHealth, float currHealth)
    {
        healthText.GetComponent<Text>().text = currHealth + "/" + maxHealth;
    }

    public void updateHealth(float maxHealth, float currHealth)
    {
        currHPTargetWidth = currHealth / maxHealth;
        healthText.GetComponent<Text>().text = Mathf.CeilToInt(currHealth) + "/" + Mathf.CeilToInt(maxHealth);
    }

    public void updateMaxHealth(float previousMaxHealth, float newMaxHealth)
    {
        float newWidth = (maxHealthBar.GetComponent<RectTransform>().sizeDelta.x / previousMaxHealth) * newMaxHealth;
        maxHealthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        currentHealthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        RectTransform textTransf = healthText.GetComponent<RectTransform>();
        textTransf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (newWidth * 0.9f));
        textTransf.position = new Vector3(maxHealthBar.GetComponent<RectTransform>().position.x + (newWidth * 0.05f), textTransf.position.y, textTransf.position.z);
    }
}
