using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class ClientGameManager : MonoBehaviour
{
    public async Task InitAsync()
    {
        //여기에 UGS서비스 인증파트가 들어갈 예정입니다.
    }

    public void GotoMenueScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }
}
