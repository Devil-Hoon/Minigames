using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMemoryGame_CamMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float smoothTimeX, smoothTimeY;  //X축과 Y축이동을 부드럽게 진행하기 위한 시간딜레이
    public Vector2 velocity;  //X,Y 축의 이동 속도
    public GameObject player;  // 카메라가 쫓을 타겟 오브젝트
    public Vector2 minPos, maxPos; //카메라가 이동가능한 최소, 최대 위치
    public bool bound;  //카메라 고정 on/off

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
		if (player != null)
		{
            float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);  //카메라의 X위치를 타겟의 X위치까지 부드럽게 이동
            float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);  //카메라의 Y위치를 타겟의 Y위치까지 부드럽게 이동

            transform.position = new Vector3(posX, posY, transform.position.z);   //카메라의 위치 이동

            if (bound)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
                    Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), Mathf.Clamp(transform.position.z, transform.position.z, transform.position.z));   //카메라의 현재위치를 최소,최대값의 범위내로 고정
            }
        }
    }
}
