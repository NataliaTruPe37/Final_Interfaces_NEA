using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class MustExpenseRow
{
    public TMP_InputField inputName;
    public TMP_InputField inputAmount;
}

public class FinanceAppManager : MonoBehaviour
{
    public GameObject panelWelcome;
    public GameObject panelSalary;
    public GameObject panelMust;
    public GameObject panelSimulator;
    public GameObject panelResult;
    public GameObject panelUser;

    public TMP_InputField inputUserName;
    public Button btnStart;

    public TMP_Text txtUserDisplay;
    public Button btnOpenUserProfile;
    public Button btnWeekly;
    public Button btnBiweekly;
    public Button btnMonthly;
    public TMP_InputField inputSalary;
    public Button btnSalaryNext;

    public TMP_InputField inputEditUserName;
    public TMP_InputField inputEditUserSalary;
    public Button btnUserBack;
    public Button btnUserNext;

    public List<MustExpenseRow> mustRows = new List<MustExpenseRow>();
    public Button btnMustBack;
    public Button btnMustNext;

    public TMP_Text txtSimulatorSalary;
    public Transform contentRed;
    public Transform contentOrange;
    public Transform contentYellow;
    public GameObject expenseCardPrefab;
    public Button btnOpenNewBill;
    public Button btnSimulatorBack;
    public Button btnSimulatorNext;

    public GameObject panelNewBill;
    public TMP_InputField inputNewBillName;
    public TMP_InputField inputNewBillAmount;
    public Button btnSelectRed;
    public Button btnSelectOrange;
    public Button btnSelectYellow;
    public Button btnConfirmAddExpense;
    public Button btnCloseNewBill;

    public GameObject panelWarningAlert;
    public Button btnCloseWarningOk;

    public GameObject resultGoodPanel;
    public TMP_Text txtGoodSalary;
    public Button btnGoodContinue;

    public GameObject resultMediumPanel;
    public TMP_Text txtMediumSalary;
    public Button btnMediumContinue;

    public GameObject resultBadPanel;
    public TMP_Text txtBadSalary;
    public Button btnBadContinue;

    public Color cardRedColor    = new Color(0.85f, 0.35f, 0.35f);
    public Color cardOrangeColor = new Color(0.85f, 0.58f, 0.28f);
    public Color cardYellowColor = new Color(0.85f, 0.78f, 0.25f);

    private string userName = "User";
    private string selectedPeriod = "";
    private float initialSalary = 0f;
    private float currentRemainingSalary = 0f;
    private int newBillCategory = 0;
    private List<ExpenseCardUI> activeCards = new List<ExpenseCardUI>();

    void Start()
    {
        ShowOnlyPanel(panelWelcome);
        panelNewBill.SetActive(false);
        panelWarningAlert.SetActive(false);
        inputSalary.interactable = false;

        btnStart.onClick.AddListener(OnStartClicked);

        if (btnOpenUserProfile != null)
            btnOpenUserProfile.onClick.AddListener(OpenUserProfile);

        btnWeekly.onClick.AddListener(() => SelectPeriod("Weekly"));
        btnBiweekly.onClick.AddListener(() => SelectPeriod("Biweekly"));
        btnMonthly.onClick.AddListener(() => SelectPeriod("Monthly"));
        btnSalaryNext.onClick.AddListener(OnSalaryNextClicked);

        if (btnUserBack != null)
            btnUserBack.onClick.AddListener(() => ShowOnlyPanel(panelSalary));
        if (btnUserNext != null)
            btnUserNext.onClick.AddListener(SaveUserProfile);

        btnMustBack.onClick.AddListener(() => ShowOnlyPanel(panelSalary));
        btnMustNext.onClick.AddListener(OnMustNextClicked);

        btnOpenNewBill.onClick.AddListener(OpenNewBillModal);
        btnSimulatorBack.onClick.AddListener(() => ShowOnlyPanel(panelMust));
        btnSimulatorNext.onClick.AddListener(OnSimulatorNextClicked);

        btnSelectRed.onClick.AddListener(() => newBillCategory = 0);
        btnSelectOrange.onClick.AddListener(() => newBillCategory = 1);
        btnSelectYellow.onClick.AddListener(() => newBillCategory = 2);
        btnConfirmAddExpense.onClick.AddListener(OnConfirmNewBill);
        if (btnCloseNewBill != null)
            btnCloseNewBill.onClick.AddListener(() => panelNewBill.SetActive(false));

        btnCloseWarningOk.onClick.AddListener(() => panelWarningAlert.SetActive(false));

        btnGoodContinue.onClick.AddListener(() => ShowOnlyPanel(panelWelcome));
        btnMediumContinue.onClick.AddListener(() => ShowOnlyPanel(panelWelcome));
        btnBadContinue.onClick.AddListener(() => ShowOnlyPanel(panelWelcome));
    }

    void ShowOnlyPanel(GameObject target)
    {
        panelWelcome.SetActive(target == panelWelcome);
        panelSalary.SetActive(target == panelSalary);
        panelMust.SetActive(target == panelMust);
        panelSimulator.SetActive(target == panelSimulator);
        panelResult.SetActive(target == panelResult);
        if (panelUser != null)
            panelUser.SetActive(target == panelUser);
    }

    void OnStartClicked()
    {
        userName = string.IsNullOrWhiteSpace(inputUserName.text) ? "USER" : inputUserName.text.Trim();
        txtUserDisplay.text = userName.ToUpper();
        ShowOnlyPanel(panelSalary);
    }

    void OpenUserProfile()
    {
        inputEditUserName.text = userName;
        inputEditUserSalary.text = inputSalary.text;
        ShowOnlyPanel(panelUser);
    }

    void SaveUserProfile()
    {
        if (!string.IsNullOrWhiteSpace(inputEditUserName.text))
        {
            userName = inputEditUserName.text.Trim();
            txtUserDisplay.text = userName.ToUpper();
        }

        if (float.TryParse(inputEditUserSalary.text, out float updatedSalary) && updatedSalary > 0f)
        {
            initialSalary = updatedSalary;
            inputSalary.text = updatedSalary.ToString("F2");
        }

        ShowOnlyPanel(panelSalary);
    }

    void SelectPeriod(string period)
    {
        selectedPeriod = period;
        inputSalary.interactable = true;
    }

    void OnSalaryNextClicked()
    {
        if (string.IsNullOrEmpty(selectedPeriod)) return;

        float.TryParse(inputSalary.text, out initialSalary);
        if (initialSalary <= 0f) return;

        ShowOnlyPanel(panelMust);
    }

    void OnMustNextClicked()
    {
        foreach (var card in activeCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        activeCards.Clear();

        foreach (MustExpenseRow row in mustRows)
        {
            if (row.inputName != null && row.inputAmount != null)
            {
                string expName = row.inputName.text.Trim();
                float.TryParse(row.inputAmount.text, out float expCost);

                if (!string.IsNullOrEmpty(expName) && expCost > 0f)
                {
                    SpawnExpenseCard(expName, expCost, 0);
                }
            }
        }

        currentRemainingSalary = initialSalary;
        UpdateSimulatorSalaryUI();
        ShowOnlyPanel(panelSimulator);
    }

    void SpawnExpenseCard(string expName, float expCost, int category)
    {
        Transform parentContainer = contentRed;
        Color chosenColor = cardRedColor;

        if (category == 1)
        {
            parentContainer = contentOrange;
            chosenColor = cardOrangeColor;
        }
        else if (category == 2)
        {
            parentContainer = contentYellow;
            chosenColor = cardYellowColor;
        }

        GameObject newObj = Instantiate(expenseCardPrefab, parentContainer);
        ExpenseCardUI cardUI = newObj.GetComponent<ExpenseCardUI>();
        cardUI.Setup(expName, expCost, category, chosenColor, this);
        activeCards.Add(cardUI);
    }

    void OpenNewBillModal()
    {
        inputNewBillName.text = "";
        inputNewBillAmount.text = "";
        newBillCategory = 0;
        panelNewBill.SetActive(true);
    }

    void OnConfirmNewBill()
    {
        string newName = inputNewBillName.text.Trim();
        float.TryParse(inputNewBillAmount.text, out float newCost);

        if (!string.IsNullOrEmpty(newName) && newCost > 0f)
        {
            SpawnExpenseCard(newName, newCost, newBillCategory);
            panelNewBill.SetActive(false);
        }
    }

    public void RecalculateSimulatorBalance()
    {
        float spent = 0f;
        foreach (var card in activeCards)
        {
            if (card.isSelected)
                spent += card.amount;
        }

        currentRemainingSalary = initialSalary - spent;
        UpdateSimulatorSalaryUI();
    }

    void UpdateSimulatorSalaryUI()
    {
        txtSimulatorSalary.text = $"Salary: ${currentRemainingSalary:F2}";
    }

    void OnSimulatorNextClicked()
    {
        foreach (var card in activeCards)
        {
            if (card.category == 0 && !card.isSelected)
            {
                panelWarningAlert.SetActive(true);
                return;
            }
        }

        float ratio = (initialSalary > 0f) ? (currentRemainingSalary / initialSalary) : 0f;
        string finalSalaryText = $"Remaining Salary: ${currentRemainingSalary:F2}";

        resultGoodPanel.SetActive(false);
        resultMediumPanel.SetActive(false);
        resultBadPanel.SetActive(false);

        if (ratio > 0.50f)
        {
            resultGoodPanel.SetActive(true);
            txtGoodSalary.text = finalSalaryText;
        }
        else if (ratio >= 0.20f)
        {
            resultMediumPanel.SetActive(true);
            txtMediumSalary.text = finalSalaryText;
        }
        else
        {
            resultBadPanel.SetActive(true);
            txtBadSalary.text = finalSalaryText;
        }

        ShowOnlyPanel(panelResult);
    }
}