using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class btnclick : MonoBehaviour
{
    private GameObject clone; // 버튼 클릭시 생성되는 오트젝트 담는 변수
    private Button btn; // 현재 클릭한 버튼 확인
    private bool ischeck = false; // 흑, 백 확인용 
    private int count = 0, three = 0, four = 0; // 레이캐스트 히트한 갯수, three는 세개일 때 증가
    private int length1 = 210, length2 = 290; // 레이캐스트 길이(length1은 상하좌우, length2는 대각)
    [SerializeField] private GameObject obj1, obj2; // 바둑돌 오브젝트 (obj1은 흑, obj2는 백)
    [SerializeField] private TextMeshProUGUI textTitle; // 메이플 폰트로 사용하는 텍스트
    [SerializeField] private GameObject[] winobj; // 승리했을 때 관리하는 오브젝트 모음

    private void FixedUpdate() // 업데이트 함수 (무한 반복함)
    {   
        eightdraw();          
    }
    
    public void BtnClick() // 버튼 클릭 함수
    {
        GameObject click = EventSystem.current.currentSelectedGameObject;

        Debug.Log(click.name); // 테스트용 버튼 출력
        if(ischeck == false) // 흑, 백 번갈아서 놓기 
        {
            clone = Instantiate(obj1, new Vector3(click.transform.position.x, click.transform.position.y, -1), Quaternion.identity);
            ischeck = true;
        }
        else
        {
            clone = Instantiate(obj2, new Vector3(click.transform.position.x, click.transform.position.y, -1), Quaternion.identity);
            ischeck = false;
        }

        // 버튼 클릭 시 비활성화 
        btn = click.GetComponentInParent<Button>();
        btn.interactable = false;

        win(); // 승리 조건 확인
    }

    public void btnresetclick() // 씬 리셋 함수
    {
        SceneManager.LoadScene("omok"); // omok씬 불러오기
    }

    public void win() // 승리 조건 확인 함수
    {
        if(clone != null) // 오브젝트가 놓여있을 때
        {
            int[] num = {WinCondition(0, -31, Vector2.down, length1) + WinCondition(0, 31, Vector2.up, length1),
            WinCondition(31, 0, Vector2.right, length1) + WinCondition(-31, 0, Vector2.left, length1),
            WinCondition(21.5f, 21.5f, Vector2.one, length2) +  WinCondition(-21.5f, -21.5f, Vector2.one * -1, length2),
            WinCondition(21.5f, -21.5f, Vector2.right + Vector2.down, length2) + WinCondition(-21.5f, 21.5f, Vector2.left + Vector2.up, length2)};
            
            Debug.Log("위, 아래:" + num[0] + " 왼쪽, 오른쪽: " + num[1]);
            Debug.Log("오른쪽 위, 왼쪽 아래 대각: " + num[2] + " 오른쪽 아래, 왼쪽 위 대각: " + num[3]);

            foreach(int item in num)
            {
                if(item == 2 && ischeck == true) // 쌍삼 확인
                {
                    Debug.Log("Three Check");
                    three++;
                    Debug.Log("Three : " + three);
                    if(three == 2) 
                    {
                        Debug.Log("쌍삼");
                        destroyClone();
                    }
                }
                if(item == 3 && ischeck == true) // 쌍사 확인
                {
                    Debug.Log("Four Check");
                    four++;
                    Debug.Log("Four : " + four);
                    if(four == 2)
                    {
                        Debug.Log("쌍사");
                        destroyClone();
                    }
                }
                if(item >= 4) // 5목 이상이고 흰색일 때만 승리
                {
                    Debug.Log(check()+ "승리");
                    if(ischeck == false) textTitle.text = "흑";
                    else textTitle.text = "백";
                    winobj[0].SetActive(false);
                    winobj[1].interactable = true;
                    winobj[0].SetActive(false);

                    if(item >= 5 && ischeck == true) // 장목
                    {
                        Debug.Log("장목");
                        destroyClone();
                    }
                }  
            }
            three = 0;
            four = 0;
        }
    }

    public String check() // 흑, 백 체크 함수
    {
        if(ischeck == false) return "white";
        else return "black";
    }

    public Vector2 createVector2(float Difx, float Dify) // 벡터2 변수 만들기 함수
    {
        Vector2 vec = new Vector2(clone.transform.position.x + Difx, clone.transform.position.y + Dify);
        return vec;
    }

    public RaycastHit2D[] createRH2D(float Difx, float Dify, Vector2 vec, float length) // 히트 레이캐스트 만들기 함수
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(createVector2(Difx, Dify), vec, length);
        return hit;
    }

    public void raycastdraw(float Difx, float Dify, Vector2 vec, float length) // 레이캐스트 그리기 함수
    {
        Debug.DrawRay(createVector2(Difx, Dify), vec * length, Color.red);
    }

    public void eightdraw() // 레이캐스트를 여덟방향으로 드로우
    {   
        if(clone != null)
        {
            raycastdraw(0, -31, Vector2.down, length1); // 아래
            raycastdraw(0, 31, Vector2.up, length1); // 위
            raycastdraw(31, 0, Vector2.right, length1); // 오른쪽
            raycastdraw(-31, 0, Vector2.left, length1); // 왼쪽
            raycastdraw(21.5f, 21.5f, Vector2.one, length2); // 오른쪽 위 대각
            raycastdraw(-21.5f, -21.5f, Vector2.one * -1, length2); // 왼쪽 아래 대각
            raycastdraw(21.5f, -21.5f, Vector2.right + Vector2.down, length2); // 오른쪽 아래 대각
            raycastdraw(-21.5f, 21.5f, Vector2.left + Vector2.up, length2); // 왼쪽 위 대각
        }
    }

    public int WinCondition(float Difx, float Dify, Vector2 vec, float length) // 히트한 갯수 확인하는 함수
    {
        count = 0;
        foreach (var item in createRH2D(Difx, Dify, vec, length))
        {
            if(item.collider != null)
            {
                Debug.Log(item.collider.name + " Hit"); // 히트 갯수 테스트 용
                count++;
                Debug.Log(LayerMask.GetMask(check()) + "=" + Math.Pow(2, item.collider.gameObject.layer)); // 레이어 테스트 용 
                if(LayerMask.GetMask(check()) != Math.Pow(2, item.collider.gameObject.layer)) // 히트한 레이어가 다르면
                {
                    Debug.Log("Different Hit"); // 레이어 다른거 히트 테스트 용
                    return 0; // 종료
                }
            }
        }
        return count;
    }

    public void destroyClone()
    {
        Destroy(clone);
        btn.interactable = true;
        ischeck = false;
    }
}