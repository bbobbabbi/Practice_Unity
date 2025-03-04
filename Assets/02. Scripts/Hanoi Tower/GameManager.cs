using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject donutPrefab;
    [SerializeField]private DonutBar[] _donutBars;

    public enum HanoiLevel { LV1 = 3, LV2, LV3 }
    public HanoiLevel e_hanoiLevel;
    public bool isSelected = false;

    public GameObject SelectedDonut;
    public float _lastDonutSize;
    public Action OnFinish;
   
    IEnumerator Start()
    {
        for (int i = 0; i < (int)e_hanoiLevel; i++) {
            Vector3 createPos = new Vector3((int)DonutBar.BarType.LEFT, 4, 0);
            GameObject donutObj = Instantiate(donutPrefab,createPos,Quaternion.identity);
            donutObj.name = "Donut" + i;
            donutObj.transform.localScale = (Vector3.one * (1f - (i * 0.15f)))*2;
            _donutBars[0].PushDonut(donutObj);
            yield return new WaitForSeconds(1f);
        }
       for(int i = 0; i < _donutBars.Length; i++)
        {
            _donutBars[i].SetOnFinishAction();
        }
    }

    private void SolveHanoiTowerFunc(int n, int from, int temp, int to)
    {
        if (n == 0) return;

        if (n == 1)
        {
            Debug.Log($"{n}�� ������ {from}�� ���뿡�� {to}�� ����� ������ �ű�ϴ�.");
        }
        else
        {
            SolveHanoiTowerFunc(n - 1, from, to,temp);
            Debug.Log($"{n}�� ������ {from}�� ���뿡�� {to}�� ����� ������ �ű�ϴ�.");
            SolveHanoiTowerFunc(n - 1, temp, from , to );
        }
    }

    public void SolveHanoiTower()
    {
        SolveHanoiTowerFunc((int)e_hanoiLevel, 0, 1, 2);
    }
}
