using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameText _prefab;

    public static TextManager Instacnce;

    private void Awake()
    {
        Instacnce = this;
    }

    public void PopUpText(string value, Vector3 pos, Color color)
    {
        GameText text = Instantiate(_prefab, pos, Quaternion.identity);
        text.SetPopUp(value, pos, color);
    }
}

