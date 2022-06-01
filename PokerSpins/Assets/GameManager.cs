using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PAYOUT_TYPE
{
    none,
    high_ace,
    one_pair,
    two_pairs,
    three_of_a_kind,
    straight,
    flush,
    full_house,
    four_of_a_kind,
    straight_flush,
    royal_flush
}

public enum GAME_STATUS
{
    getInfo,
    ready,
    bet,
    spin,
    result,
    claim
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    BonusData dataGame;
    [SerializeField]
    List<SlotScroll> listSlot = new List<SlotScroll>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    Dictionary<int, Sprite> listCardSprite = new Dictionary<int, Sprite>();
    List<CardData> listResult = new List<CardData>();
    int durationRoll = 0;
    double betAmount = 0;

    GAME_STATUS _status;
    GAME_STATUS status { 
        get { return _status; }
        set
        {
            _status = value;
            switch (_status)
            {
                case GAME_STATUS.getInfo:
                    UIManager.instance.ShowLoading();
                    StartCoroutine(GetInfo());
                    break;
                case GAME_STATUS.ready:
                    UIManager.instance.HideLoading();
                    break;
                case GAME_STATUS.bet:
                    break;
                case GAME_STATUS.spin:
                    break;
                case GAME_STATUS.result:
                    break;
                case GAME_STATUS.claim:
                    UIManager.instance.ShowLoading();
                    break;
                default:
                    break;
            }
        }
    }
    DataGame data = new DataGame();
    // Start is called before the first frame update
    void Start()
    {
        status = GAME_STATUS.getInfo;
    }

    IEnumerator GetInfo()
    {
        while (string.IsNullOrEmpty(ReactBridgePlugin.instance.GetAuth().authToken))
        {
            yield return new WaitForSeconds(0.5f);
        }
        ApiManager.instance.GetInfo((x) =>
        {
            ResInfo info = (ResInfo)x;
            UIManager.instance.SetCollar(info.collar);
            UIManager.instance.SetCredit(info.credit);
            data.credit = info.credit;
            status = GAME_STATUS.ready;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetCardSprite(int cardId)
    {
        if(!listCardSprite.ContainsKey(cardId))
        {
            Sprite[] cards = Resources.LoadAll<Sprite>($"Textures/Card");
            Sprite card = cards.Single(s => s.name == $"Card_{cardId}");
            listCardSprite.Add(cardId, card);
        }
        return listCardSprite[cardId];
    }

    public void Roll()
    {
        if (status >= GAME_STATUS.spin)
        {
            return;
        }
        if (betAmount == 0)
        {
            UIManager.instance.ShowDialog("Please bet more than 0 Credits.");
            return;
        }
        if (durationRoll == 0)
        {
            status = GAME_STATUS.spin;
            UIManager.instance.SetTheme(PAYOUT_TYPE.none);
            //GeneratesResult();
            for (int i = 0; i < 5; i++)
            {
               listSlot[i].SetResult(new CardData(CARD_TYPE.club,0));
            }
            ApiManager.instance.GetResult(betAmount, (x) =>
            {
                data = ((ResResult)x).data;
                listResult = data.cards;
                for (int i = 0; i < 5; i++)
                {
                    int number = listSlot[i].SetResult(data.cards[i], 0);
                    if (number > durationRoll)
                    {
                        durationRoll = number;
                    }
                }
                UIManager.instance.SetCredit(data.credit);
                Invoke("FinishRoll", durationRoll * 0.15f + 0.5f);
            });
        }
    }

    void FinishRoll()
    {
        status = GAME_STATUS.result;
        if (betAmount > data.credit)
        {
            betAmount = data.credit >= 0? data.credit : 0;
        }
        UIManager.instance.SetBet(betAmount);
        UIManager.instance.SetReward(data.reward);
        UIManager.instance.SetCollar(data.collar);
        UIManager.instance.SetTheme(data.payout);
        durationRoll = 0;
        status = GAME_STATUS.ready;
    }

    //void GeneratesResult()
    //{
    //    listResult.Clear();
    //    int target = -1;
    //    for (int i = 0; i < 5; i++)
    //    {
    //        do
    //        {
    //            target = Random.Range(0, 52);
    //        } while (Card.CheckContainCard(listResult, Card.ConvertToCardData(target)));
    //        listResult.Add(Card.ConvertToCardData(target));
    //    }
    //}

    PAYOUT_TYPE CheckPayout()
    {
        //Debug.Log(JsonUtility.ToJson(new TurnData(listResult)));
        listResult.Sort((x,y)=>x.number.CompareTo(y.number));
        //Debug.Log(JsonUtility.ToJson(new TurnData(listResult)));
        int sameType = 0;
        bool isIncrease = true;
        int countSameValue = 0;
        bool isHighAce = false;
        List<int> listSameValue = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if(listResult[0].cardType == listResult[i].cardType)
            {
                sameType++;
            }
            if (i > 0)
            {
                if (isIncrease & listResult[i].number != listResult[i - 1].number + 1 && !(listResult[i-1].number ==0 && listResult[i].number ==9))
                {
                    isIncrease = false;
                }
                if(listResult[i].number == listResult[i - 1].number)
                {
                    countSameValue++;
                } else if(countSameValue>0)
                {
                    listSameValue.Add(countSameValue);
                    countSameValue = 0;
                }
            }
            if(listResult[i].number == 0)
            {
                isHighAce = true;
            }
        }
        if (countSameValue > 0)
        {
            listSameValue.Add(countSameValue);
            countSameValue = 0;
        }

        listSameValue.Sort((x,y)=>y.CompareTo(x));
        //Debug.Log(JsonUtility.ToJson(new PairData(listSameValue)));
        if (sameType==5)
        {
            if(Card.CheckContainCard(listResult,new CardData(listResult[0].cardType,0))
                && Card.CheckContainCard(listResult, new CardData(listResult[0].cardType, 9))
                && Card.CheckContainCard(listResult, new CardData(listResult[0].cardType, 10))
                && Card.CheckContainCard(listResult, new CardData(listResult[0].cardType, 11))
                && Card.CheckContainCard(listResult, new CardData(listResult[0].cardType, 12)))
            {
                return PAYOUT_TYPE.royal_flush;
            } else if(isIncrease)
            {
                return PAYOUT_TYPE.straight_flush;
            } else 
            {
                return PAYOUT_TYPE.flush;
            }
        } else
        {
            if (listSameValue.Count > 0) {
                if (listSameValue[0] == 3)
                {
                    return PAYOUT_TYPE.four_of_a_kind;
                } else if (listSameValue.Count == 2 && listSameValue[0] == 2 && listSameValue[1] == 1)
                {
                    return PAYOUT_TYPE.full_house;
                } else if (isIncrease)
                {
                    return PAYOUT_TYPE.straight;
                } else if (listSameValue[0] == 2)
                {
                    return PAYOUT_TYPE.three_of_a_kind;
                }
                else if (listSameValue.Count > 1 && listSameValue[0] == 1 && listSameValue[1] == 1)
                {
                    return PAYOUT_TYPE.two_pairs;
                } else
                {
                    return PAYOUT_TYPE.one_pair;
                }
            } else if(isHighAce)
            {
                return PAYOUT_TYPE.high_ace;
            }
        }
        return PAYOUT_TYPE.none;
    }

    public void BetDecrease()
    {
        if(status >= GAME_STATUS.spin || data.credit <= 0)
        {
            return;
        }
        if (betAmount > 0)
        {
            betAmount--;
            UIManager.instance.SetBet(betAmount);
        }
        status = GAME_STATUS.bet;
    }

    public void BetIncrease()
    {
        if (status >= GAME_STATUS.spin || data.credit <= 0)
        {
            return;
        }
        if (betAmount < 1000)
        {
            betAmount++;
            UIManager.instance.SetBet(betAmount);
        }
        status = GAME_STATUS.bet;
    }

    public void BetPercent(int amount)
    {
        if (status >= GAME_STATUS.spin || data.credit <= 0)
        {
            return;
        }
        if (data.credit > 0)
        {
            betAmount = data.credit > amount? amount: data.credit;
            UIManager.instance.SetBet(betAmount);
        }
        status = GAME_STATUS.bet;
    }

    public void Claim()
    {
        UIManager.instance.ShowLoading();
        if (data.collar == 0)
        {
            ReactBridgePlugin.instance.EndgameResult(new DataEndGame((int)data.collar));
        } else
        {
            ApiManager.instance.GetClaim((x) => {
                ResClaim resClaim = (ResClaim)x;
                if (resClaim.data.Equals("Success")) {
                    ReactBridgePlugin.instance.EndgameResult(new DataEndGame((int)data.collar));
                } else
                {
                    UIManager.instance.HideLoading();
                    UIManager.instance.ShowDialog("Claim failed! Please try again!");
                }
            });
        }
    }
}

[System.Serializable]
public class PairData
{
    public List<int> listCard;
    public PairData(List<int> cardDatas)
    {
        listCard = cardDatas;
    }
}

[System.Serializable]
public class TurnData
{
    public List<CardData> listCard;
    public TurnData(List<CardData> cardDatas)
    {
        listCard = cardDatas;
    }
}

[System.Serializable]
public class PayoutData
{
    public string textContent;
    public int valueBonus;
    public Color backgroundColor;
    public Color textColor;
}