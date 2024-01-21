using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public float stamina;
    public int numOfPotions;

    public Image[] potions;
    public Sprite fullPotion;
    public Sprite halfPotion;
    public Sprite emptyPotion;

    private float getNextStamina = 0.0f;
    public float getStamina = 1.0f;

    void Update()
    {
        displayHealth();
        displayStamina();
        if(Time.time > getNextStamina)
        {
            getNextStamina += getStamina;
            if(stamina < numOfPotions - 1)
            {
                stamina++;
            }
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
        if (stamina > numOfPotions)
        {
            stamina = numOfPotions;
        }
        for (int i = 0; i < potions.Length; i++)
        {
            if (i < stamina)
            {
                if (i == stamina - 0.5f)
                {
                    potions[i].sprite = halfPotion;
                }
                else
                {
                    potions[i].sprite = fullPotion;
                }

            }
            else
            {
                potions[i].sprite = emptyPotion;
            }

            if (i < numOfPotions)
            {
                potions[i].enabled = true;
            }
            else
            {
                potions[i].enabled = false;
            }
        }
    }

 
}