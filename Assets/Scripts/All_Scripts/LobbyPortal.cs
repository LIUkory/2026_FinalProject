using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyPortal : MonoBehaviour
{
    [Header("���d�]�w")]
    // �b Inspector ��J�A�����C���Ĥ@���������W�� (�Ҧp Level_1)
    public string targetSceneName = "Dungeon_01";

    private bool isPlayerInRange = false;

    void Update()
    {
        // �����a�b�ǰe�}���A�B���U E ��
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("�i�ǰe���j�ҰʡI���b�e���U�@���G" + targetSceneName);

            // ����־���G�ǰe�e���⪰�W�Y�W�� [E] ���áA�קK�ݯd��U�@��
            if (InteractUI_manager.instance != null)
            {
                InteractUI_manager.instance.HidePrompt();
            }

            // �� �֤��]�k�G���������A�����}�l�C���I
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �u���K�� Player ���Ҫ��D����i�Ӥ~Ĳ�o
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // �I�s���W�Y�W�� [E] �B�{
            if (InteractUI_manager.instance != null)
            {   
                InteractUI_manager.instance.ShowPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // ���a���}�ǰe�}�A���� [E]
            if (InteractUI_manager.instance != null)
            {
                InteractUI_manager.instance.HidePrompt();
            }
        }
    }
}
