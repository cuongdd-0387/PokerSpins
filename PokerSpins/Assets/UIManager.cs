using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    [SerializeField]
    Image imgBorder;
    [SerializeField]
    Image imgBackground;
    [SerializeField]
    Text txtPayout;
    [SerializeField]
    BonusData dataGame;
    [SerializeField]
    Text txtBetAmount;
    [SerializeField]
    Text txtReward;
    [SerializeField]
    Text txtCollar;
    [SerializeField]
    Text txtCredit;
    [SerializeField]
    GameObject loading;
    [SerializeField]
    Text txtDialogMsg;
    [SerializeField]
    GameObject dialogMsg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTheme(PAYOUT_TYPE payout)
    {
        DOTween.Kill(imgBorder);
        DOTween.Kill(txtPayout.transform);
        switch (payout)
        {
            case PAYOUT_TYPE.none:
                txtPayout.text = "";
                imgBorder.color = Color.white;
                imgBackground.color = Color.black;
                SetReward(0);
                break;
            default:
                txtPayout.text = dataGame.listPayout[(int)payout].textContent;
                txtPayout.color = dataGame.listPayout[(int)payout].textColor;
                imgBorder.color = dataGame.listPayout[(int)payout].backgroundColor;
                imgBackground.color = dataGame.listPayout[(int)payout].backgroundColor;
                txtPayout.transform.DOLocalMoveX(-400, 2f).From(400).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                imgBorder.DOColor(Color.white,0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }

    public void SetBet(double amount)
    {
        txtBetAmount.text = $"©{amount.ToString("N1")}";
    }

    public void SetReward(double reward)
    {
        txtReward.text = $"₵{reward.ToString("N1")}";
    }

    public void SetCollar(double collar)
    {
        txtCollar.text = $"₵{collar.ToString("N1")}";
    }

    public void SetCredit(double credit)
    {
        txtCredit.text = $"©{credit.ToString("N1")}";
    }

    int numberLoading = 0;
    public void ShowLoading()
    {
        numberLoading++;
        loading.SetActive(true);
    }

    public void HideLoading()
    {
        numberLoading--;
        if (numberLoading <= 0)
        {
            numberLoading = 0;
            loading.SetActive(false);
        }
    }

    public void ShowDialog(string content)
    {
        txtDialogMsg.text = content;
        dialogMsg.SetActive(true);
    }
}
