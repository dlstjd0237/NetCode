using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class ClientGameManager : MonoBehaviour
{
    public async Task InitAsync()
    {
        //���⿡ UGS���� ������Ʈ�� �� �����Դϴ�.
    }

    public void GotoMenueScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }
}
