using UnityEngine;
using UnityEngine.UI;
public class RogueStateWorldState : MonoBehaviour
{
    public int cash = 100, reputation = 0, maraTrust = 50, attention = 0;
    public GameObject panel;
    public Text missionText, statsText;
    bool opened;
    void Start() { if (panel) panel.SetActive(false); Refresh(); }
    void Update() { if (opened && Input.GetKeyDown(KeyCode.Escape)) Close(); }
    public void Open()
    {
        opened = true; if (panel) panel.SetActive(true);
        if (missionText) missionText.text = "THE SILENT LEDGER\n\nYou found information suggesting a local business may be funding a political campaign. The evidence is incomplete. What will you do?";
        Refresh();
    }
    public void Choose(string choice)
    {
        string msg = "";
        switch (choice)
        {
            case "Report": reputation += 10; attention += 5; msg = "You reported the lead. Credibility rises, and authorities start paying attention."; break;
            case "Sell": cash += 200; maraTrust -= 10; attention += 3; msg = "You sold the information for $200. Mara may trust you less."; break;
            case "Investigate":
                if (cash >= 20) { cash -= 20; maraTrust += 10; msg = "You spent $20 investigating further. Mara trusts you more, but the truth is still uncertain."; }
                else msg = "You need at least $20 to investigate.";
                break;
            case "Keep": reputation += 2; attention += 1; msg = "You kept the information. You stay in control, but someone may notice your interest."; break;
        }
        maraTrust = Mathf.Clamp(maraTrust, 0, 100);
        attention = Mathf.Clamp(attention, 0, 100);
        if (missionText) missionText.text = "CHOICE: " + choice.ToUpper() + "\n\n" + msg + "\n\nClose this window and explore, or investigate again later.";
        Refresh();
    }
    public void Close() { opened = false; if (panel) panel.SetActive(false); }
    void Refresh()
    {
        if (statsText) statsText.text = "CASH $" + cash + "   REP " + reputation + "   MARA TRUST " + maraTrust + "   ATTENTION " + attention;
    }
}