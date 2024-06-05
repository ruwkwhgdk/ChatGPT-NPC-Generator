using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public Canvas inputUICanvas;

    public NPCManager NPCManager;

    void Start()
    {
        // 초기에는 Input UI Canvas를 숨기고 Main Menu Canvas만 보이게 합니다.
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
        // 데이터 추가 버튼 클릭 시 Input UI Canvas를 활성화하고 Main Menu Canvas를 숨깁니다.
        inputUICanvas.gameObject.SetActive(true);
        mainMenuCanvas.gameObject.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        // Back 버튼 클릭 시 Main Menu Canvas를 활성화하고 Input UI Canvas를 숨깁니다.
        inputUICanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }
}