using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }
public class Enemy : MonoBehaviour
{
    private int wayPointCount;
    private Transform[] wayPoints;
    private int currentIndex = 0;
    private Movement2D movement2D;
    private EnemySpawner enemySpawner;

    [SerializeField]
    private int gold = 10;
    [SerializeField]
    private int score = 10;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
	{
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // 적 이동 경로 WayPoints 정보 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫번째 wayPoint 위치로 설정
        transform.position = wayPoints[currentIndex].position;

        //적 이동/목표지저머 설정 코루틴함수 시작
        StartCoroutine("OnMove");
	}

    private IEnumerator OnMove()
	{
        NextMoveTo();

		while (true)
		{
            //적 오브젝트 회전
            //transform.Rotate(Vector3.forward * 10);

			//적의 현재위치와 목표위치의 거리가 0.02 * moveMont2D.moveSpeed 보다 적을때 if조건문 실행
			//Tip. movement2D.movSpeed를 곱해주는 이유는 속도가 빠르면 한 프레임에 0.02보다 크게 움직이기 때문에
			//if 조건문에 걸리지않고 경로를 탈주하는 오브젝트가 발생할 수 있다.
			if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.movSpeed)
			{
                // 다음 이동방향 설정
                NextMoveTo();
			}

            yield return null;
		}

	}

    private void NextMoveTo()
	{
		//아직 이동할 wayPoints가 남아있다면
		if (currentIndex < wayPointCount - 1)
		{
            //적의 위치를 정확하게 목표 위치로 설정
            transform.position = wayPoints[currentIndex].position;
            //이동 방향 설정=> 다음 목표지점(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
		}
        //현재 위치가 마지막 wayPoints이면
		else
		{
            gold = 0;
            //적 오브젝트 삭제
            OnDie(EnemyDestroyType.Arrive);

		}
	}

    public void OnDie(EnemyDestroyType type)
	{
        enemySpawner.DestroyEnemy(type, this, gold, score);
	}
}
