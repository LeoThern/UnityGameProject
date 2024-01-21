using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthAndStamina : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public float stamina;
    public float maxStamina;

    public Slider staminaBar;

    public float getStamina = 5.0f;

    private void Start()
    {
        stamina = maxStamina;
        staminaBar.maxValue = maxStamina;
    }
    void Update()
    {
        displayHealth();
        displayStamina();
        if(stamina < maxStamina)
        {
        stamina += getStamina * Time.deltaTime;
        }
    }

   void displayHealth()
    {
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                if (i == health - 0.5f)
                {
                    hearts[i].sprite = halfHeart;
                }
                else
                {
                    hearts[i].sprite = fullHeart;
                }

            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    void displayStamina()
    {
        staminaBar.value = stamina;
    } 

    public bool checkAndConsumeStamina(float requiredStamina)
    {
        if(stamina > requiredStamina)
        {
            stamina -= requiredStamina;
            return true;
        }
        return false;
    }
    public void consumeStamina(float stamina)
    {
            stamina -= stamina;  
    }


}