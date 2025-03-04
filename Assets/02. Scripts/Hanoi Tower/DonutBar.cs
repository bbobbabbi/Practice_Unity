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
            if (!gameManager.isSelected) // ������ ������ ����� ���� �������� �ʾ��� �� (������ ������ ����� �������� ��)
            {
                if (donutStack.Count == 0)
                {
                    Debug.Log($"{gameObject.name}����� ����־� ����  ������ �����ϴ�.");
                    return;
                }
                gameManager.SelectedDonut = PopDonut();
                gameManager.isSelected = true;
            }
            else // ������ ���� ����� �������� ��
            {
                // ����� ���ÿ� ����� ���Ӱ� �ֱ�
                //������ ��ġ �̵�
                if (donutStack.Count != 0)
                {
                    if (gameManager._lastDonutSize > donutStack.Peek().transform.localScale.x)
                    {
                        Debug.Log("���� ���뿡 �ִ� ���Ӻ��� ���� ���Ӹ� �ű� �� �ֽ��ϴ�.");
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
