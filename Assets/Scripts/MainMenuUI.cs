using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public Canvas inputUICanvas;

    public NPCManager NPCManager;

    void Start()
    {
        // �ʱ⿡�� Input UI Canvas�� ����� Main Menu Canvas�� ���̰� �մϴ�.
        inputUICanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void OnSimulationButtonClicked()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        NPCManager.Simulate();
    }

    public void OnAddDataButtonClicked()
    {
        // ������ �߰� ��ư Ŭ�� �� Input UI Canvas�� Ȱ��ȭ�ϰ� Main Menu Canvas�� ����ϴ�.
        inputUICanvas.gameObject.SetActive(true);
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        // Back ��ư Ŭ�� �� Main Menu Canvas�� Ȱ��ȭ�ϰ� Input UI Canvas�� ����ϴ�.
        inputUICanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }
}