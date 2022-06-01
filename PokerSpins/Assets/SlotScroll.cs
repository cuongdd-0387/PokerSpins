using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotScroll : MonoBehaviour
{
    [SerializeField]
    Image card0;
    [SerializeField]
    Image card1;
    float duration = 0.15f;
    [SerializeField]
    int currentCardNumber = 0;
    CardData targetCard;
    int numberCycle = 0;
    // Start is called before the first frame update
    void Start()
    {
        card0.transform.localPosition = new Vector2(0,300);
        card1.transform.localPosition = new Vector2(0, 0);
        Roll(currentCardNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Roll(int cardId)
    {
        if (Mathf.Abs(card1.transform.localPosition.y) == 300)
        {
            card0.transform.DOLocalMoveY(-300, duration).SetEase(Ease.Linear).OnComplete(()=> { 
                card0.transform.localPosition = new Vector2(0, 300);
                ReRoll();
            });
            card1.sprite = GameManager.instance.GetCardSprite(cardId);
            card1.transform.DOLocalMoveY(0, duration).SetEase(Ease.Linear);
        } else if (Mathf.Abs(card0.transform.localPosition.y) == 300)
        {
            card1.transform.DOLocalMoveY(-300, duration).SetEase(Ease.Linear).OnComplete(() => {
                card1.transform.localPosition = new Vector2(0, 300);
                ReRoll();
            });
            card0.sprite = GameManager.instance.GetCardSprite(cardId);
            card0.transform.DOLocalMoveY(0, duration).SetEase(Ease.Linear);
        }
    }

    public void ReRoll()
    {
        if (numberCycle > 0)
        {
            numberCycle--;
            if (numberCycle > 0) {
                int id = currentCardNumber;
                while (id == currentCardNumber)
                {
                    id = Random.Range(0, 52);
                }
                currentCardNumber = id;
            } else
            {
                currentCardNumber = Card.ConvertToIndex(targetCard);
            }
            Roll(currentCardNumber);
        }
    }

    public int SetResult(CardData card, int cycle = 1000)
    {
        targetCard = card;
        numberCycle = cycle == 0?Random.Range(5, 20): cycle;
        ReRoll();
        return numberCycle;
    }

    public CardData GetCard()
    {
        return targetCard;
    }
}

public enum CARD_TYPE
{
    club,
    diamond,
    heart,
    spade
}

[System.Serializable]
public class CardData
{
    public CARD_TYPE cardType;
    public int number;
    public CardData(CARD_TYPE cardType, int number)
    {
        this.cardType = cardType;
        this.number = number;
    }
}

[System.Serializable]
public class Card
{
    public static CardData ConvertToCardData(int cardID)
    {
        return new CardData((CARD_TYPE)(cardID / 13), cardID % 13);
    }

    public static int ConvertToIndex(CardData card)
    {
        return (int)(card.cardType) * 13 + card.number;
    }

    public static bool CheckContainCard(List<CardData> cards, CardData card)
    {
        foreach (var item in cards)
        {
            if(item.cardType == card.cardType && item.number == card.number)
            {
                return true;
            }
        }
        return false;
    }
}