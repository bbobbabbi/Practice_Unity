using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DonutBar : MonoBehaviour
{
    public enum BarType { LEFT = -3, CENTER = 0 , RIGHT = 3 }
    public BarType e_barType;
    public Stack<GameObject> donutStack;
    public List<GameObject> viewStack;
    public GameManager gameManager;
    public Action Onfinish;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        donutStack = new Stack<GameObject>();    
    }
    private void OnMouseDown()
    {
        Onfinish?.Invoke();
    }
    public void SetOnFinishAction() {
        Onfinish =()=> {
            if (!gameManager.isSelected) // 도넛을 가져올 기둥을 아직 선택하지 않았을 때 (도넛을 가져올 기둥을 선택했을 때)
            {
                if (donutStack.Count == 0)
                {
                    Debug.Log($"{gameObject.name}막대는 비어있어 꺼낼  도넛이 없습니다.");
                    return;
                }
                gameManager.SelectedDonut = PopDonut();
                gameManager.isSelected = true;
            }
            else // 도넛을 넣을 기둥을 선택했을 때
            {
                // 기둥의 스택에 저장된 도넛값 넣기
                //도넛의 위치 이동
                if (donutStack.Count != 0)
                {
                    if (gameManager._lastDonutSize > donutStack.Peek().transform.localScale.x)
                    {
                        Debug.Log("기존 막대에 있는 도넛보다 작은 도넛만 옮길 수 있습니다.");
                        return;
                    }
                }
                PushDonut(gameManager.SelectedDonut);
                gameManager.isSelected = false;

            }
        };
    }

    public void PushDonut(GameObject donut)
    {
        donutStack.Push(donut);
        donut.transform.position = new Vector3((int)e_barType, 4, 0);
        donut.transform.rotation = Quaternion.identity;
        viewStack = donutStack.ToList();
    }
    public GameObject PopDonut()
    {
        GameObject  obj = donutStack.Pop();;
        gameManager._lastDonutSize = obj.transform.localScale.x;
        viewStack = donutStack.ToList();
        return obj;
    }
}
