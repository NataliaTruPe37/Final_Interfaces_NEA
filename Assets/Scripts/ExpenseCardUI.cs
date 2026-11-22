using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpenseCardUI : MonoBehaviour
{
    public TMP_Text txtName;
    public TMP_Text txtAmount;
    public Image cardBackground;
    public Button cardButton;

    [HideInInspector] public string expenseName;
    [HideInInspector] public float amount;
    [HideInInspector] public int category;
    [HideInInspector] public bool isSelected = false;

    private Color baseColor;
    private FinanceAppManager manager;

    public void Setup(string name, float cost, int cat, Color color, FinanceAppManager appManager)
    {
        expenseName = name;
        amount = cost;
        category = cat;
        baseColor = color;
        manager = appManager;

        txtName.text = name;
        txtAmount.text = $"${cost:F2}";
        cardBackground.color = baseColor;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(ToggleSelection);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;

        cardBackground.color = isSelected 
            ? new Color(baseColor.r * 0.65f, baseColor.g * 0.65f, baseColor.b * 0.65f, 1f) 
            : baseColor;

        manager.RecalculateSimulatorBalance();
    }
}
