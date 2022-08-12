using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnchantGame_CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float smoothTimeX, smoothTimeY;  //X축과 Y축이동을 부드럽게 진행하기 위한 시간딜레이
    public Vector2 velocity;  //X,Y 축의 이동 속도
    public GameObject target;  // 카메라가 쫓을 타겟 오브젝트
    public Vector2 minPos, maxPos; //카메라가 이동가능한 최소, 최대 위치
    public bool bound;  //카메라 고정 on/off
    public GameObject zeroPos;

    public Button enhance;
    public Button sell;
    public Button shopBtn;

    public EnchantGame_SeedPlant seedPlant;
    public EnchantGame_Shop shop;

    public bool earthquake;
    void Start()
    {
        earthquake = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

	private void FixedUpdate()
	{
		if (target != null && !earthquake)
		{
            float posX = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref velocity.x, smoothTimeX);  //카메라의 X위치를 타겟의 X위치까지 부드럽게 이동
            float posY = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref velocity.y, smoothTimeY);  //카메라의 Y위치를 타겟의 Y위치까지 부드럽게 이동

            transform.position = new Vector3(posX, posY, transform.position.z);   //카메라의 위치 이동

            if (bound)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
                    Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), Mathf.Clamp(transform.position.z, transform.position.z, transform.position.z));   //카메라의 현재위치를 최소,최대값의 범위내로 고정
            }
        }
		else if(!earthquake)
		{
			
            float posX = Mathf.SmoothDamp(transform.position.x, zeroPos.transform.position.x, ref velocity.x, 1.0f);  //카메라의 X위치를 타겟의 X위치까지 부드럽게 이동
            float posY = Mathf.SmoothDamp(transform.position.y, zeroPos.transform.position.y, ref velocity.y, 1.0f);  //카메라의 Y위치를 타겟의 Y위치까지 부드럽게 이동

			if (Mathf.Approximately(posX, 0.0f))
			{
                posX = 0.0f;
			}
            if (Mathf.Approximately(posY, 0.0f))
			{
                posY = 0.0f;
			}

            transform.position = new Vector3(posX, posY, transform.position.z);

            if (bound)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
                    Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), Mathf.Clamp(transform.position.z, transform.position.z, transform.position.z));   //카메라의 현재위치를 최소,최대값의 범위내로 고정
            }

			if (transform.position.y < 0.01f)
			{
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                earthquake = true;
                shopBtn.interactable = true;

				if (DBManager.isPlant)
				{
                    if (seedPlant.enchant == seedPlant.maxEnchant)
                    {
                        enhance.interactable = false;
                        if (DBManager.seedName == "Random")
                        {
                            shop.RandomSeedComplete();
                        }
                        else
                        {
                            shop.SetIdle();
                            sell.interactable = true;
                        }
                    }
                    else
                    {
                        enhance.interactable = true;
                        sell.interactable = true;
                    }
                }
				else
				{
                    enhance.interactable = false;
                    sell.interactable = false;
				}
			}
            
        }
        
	}

    public void SetTarget(GameObject target)
	{
        this.target = target;

    }

    public void TargetRelease()
	{
        target = null;
	}
}
