using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }
public class TowerWeapon : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private TowerTemplate towerTemplate;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform spawnPoint2;
    [SerializeField]
    private WeaponType weaponType;
    
    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform hitEffect;
    [SerializeField]
    private LayerMask targetLayer;

    [Header("Slow")]
    [SerializeField]
    private CircleCollider2D circleCollider2D;
    [SerializeField]
    private GameObject slowEffect;

    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private RuntimeAnimatorController animatorController;
    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private Tile ownerTile;

    private float addedDamage;
    private int buffLevel;
    private float rotDegree = 0.0f;
    public Sprite towerSprite => towerTemplate.weapon[level].sprite;
    public Animator anim => animator;
    public float damage => towerTemplate.weapon[level].damage;
    public float rate => towerTemplate.weapon[level].rate;
    public float range => towerTemplate.weapon[level].range;
    public int upgradeCost => lev < maxLevel ? towerTemplate.weapon[level + 1].cost : 0;
    public int sellCost => towerTemplate.weapon[level].sell;
    public int lev => level + 1;
    public int maxLevel => towerTemplate.weapon.Length;
    public float slow => towerTemplate.weapon[level].slow;
    public float buff => towerTemplate.weapon[level].buff;
    public WeaponType wType => weaponType;
    public float addDamage
	{
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
	}
    public int bLevel
	{
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
	}



    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
		if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Buff)
        {
            animator = GetComponent<Animator>();
            animatorController = towerTemplate.weapon[level].animatorController;
        }
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

		if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
	}

    public void SetEnemySpawenr(EnemySpawner enemySpawner)
	{
        this.enemySpawner = enemySpawner;
	}
    public void ChangeState(WeaponState newState)
	{
        StopCoroutine(weaponState.ToString());

        weaponState = newState;

        StartCoroutine(weaponState.ToString());
	}
    // Update is called once per frame
    void Update()
    {
		if (attackTarget != null)
		{
            RotateToTarget();
		}

		if (weaponType == WeaponType.Slow)
		{
            rotDegree += Time.deltaTime * 200.0f;
			if (rotDegree > 360.0f)
			{
                rotDegree = 0.0f;
			}
            transform.rotation = Quaternion.Euler(0, 0, rotDegree);
		}
		else if (weaponType == WeaponType.Buff)
		{
			if (level == towerTemplate.weapon.Length - 1)
            {
                rotDegree += Time.deltaTime * 200.0f;
                if (rotDegree > 360.0f)
                {
                    rotDegree = 0.0f;
                }
                transform.rotation = Quaternion.Euler(0, 0, rotDegree);
            }
		}
        
    }

    private void RotateToTarget()
	{
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
		if (weaponType == WeaponType.Cannon)
		{
            float degree = Mathf.Atan2(dx, dy) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, -degree);
        }
		else
		{
            
            float degree = Mathf.Atan2(-dx, -dy) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, -degree);
		}
	}

    private IEnumerator SearchTarget()
	{
		while (true)
		{
            //         float closestDistSqr = Mathf.Infinity;
            //for (int i = 0; i < enemySpawner.enemyList.Count ; i++)
            //{
            //             float distance = Vector3.Distance(enemySpawner.enemyList[i].transform.position, transform.position);

            //	if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            //	{
            //                 closestDistSqr = distance;
            //                 attackTarget = enemySpawner.enemyList[i].transform;
            //	}
            //}

            attackTarget = FindClosestAttackTarget();

			if (attackTarget != null)
			{
				switch (weaponType)
				{
					case WeaponType.Cannon:
                        ChangeState(WeaponState.TryAttackCannon);
                        break;
					case WeaponType.Laser:
                        ChangeState(WeaponState.TryAttackLaser);
                        break;
					default:
						break;
				}
			}

            yield return null;
		}
	}

    //private IEnumerator AttackToTarget()
	private IEnumerator TryAttackCannon()
    {
		while (true)
		{
			//if (attackTarget == null)
			//{
			//             ChangeState(WeaponState.SearchTarget);
			//             break;
			//}

			//         float distance = Vector3.Distance(attackTarget.position, transform.position);
			//if (distance > towerTemplate.weapon[level].range)
			//{
			//             attackTarget = null;
			//             ChangeState(WeaponState.SearchTarget);
			//             break;
			//}
			if (!IsPossibleToAttackTarget())
			{
                ChangeState(WeaponState.SearchTarget);
                break;
			}


            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
		}
	}

    private IEnumerator TryAttackLaser()
	{
        EnableLaser();

		while (true)
		{
			if (!IsPossibleToAttackTarget())
			{
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
			}

            SpawnLaser();

            yield return null;
		}
	}
    public void OnBuffAroundTower()
	{
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

		for (int i = 0; i < towers.Length; i++)
		{
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

			if (weapon.bLevel > lev)
			{
                continue;
			}

			if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
			{
				if (weapon.wType == WeaponType.Cannon || weapon.wType == WeaponType.Laser)
				{
                    weapon.addDamage = weapon.damage * (towerTemplate.weapon[level].buff);
                    weapon.bLevel = lev;
				}
			}
		}
	}
    private Transform FindClosestAttackTarget()
	{
        float closestDistSqr = Mathf.Infinity;
		for (int i = 0; i < enemySpawner.enemyList.Count ; ++i)
		{
            float distance = Vector3.Distance(enemySpawner.enemyList[i].transform.position, transform.position);

			if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
			{
                closestDistSqr = distance;
                attackTarget = enemySpawner.enemyList[i].transform;
			}
		}

        return attackTarget;
	}

    private bool IsPossibleToAttackTarget()
	{
        if (attackTarget == null)
		{
            return false;
		}

        float distance = Vector3.Distance(attackTarget.position, transform.position);
		if (distance > towerTemplate.weapon[level].range)
		{
            attackTarget = null;
            return false;
		}

        return true;
	}

    private void SpawnProjectile()
	{
        animator.SetTrigger("Shoot");
		if (level  > 0)
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint2.position, Quaternion.identity);
            float damage = towerTemplate.weapon[level].damage + addDamage;
            clone.GetComponent<Projectile>().Setup(attackTarget, damage);
        }
		else
		{
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            float damage = towerTemplate.weapon[level].damage + addDamage;
            clone.GetComponent<Projectile>().Setup(attackTarget, damage);
        }
	}

    private void EnableLaser()
	{
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
	}

    private void DisableLaser()
	{
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
	}

    private void SpawnLaser()
	{
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range);

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i].transform == attackTarget)
			{
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;
                float damage = towerTemplate.weapon[level].damage + addDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
			}
		}
	}
	public bool Upgrade()
	{
		if (playerGold.curGold < towerTemplate.weapon[level + 1].cost)
		{
			return false;
		}
		level++;
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;

        playerGold.curGold -= towerTemplate.weapon[level].cost;
        if (weaponType == WeaponType.Cannon)
        {
            animator.runtimeAnimatorController = towerTemplate.weapon[level].animatorController;
        }
		else if (weaponType == WeaponType.Laser)
		{
            lineRenderer.startWidth = 0.07f + level * 0.07f;
            lineRenderer.endWidth = 0.07f;
		}
		else if (weaponType == WeaponType.Slow)
		{
            circleCollider2D.radius = towerTemplate.weapon[level].range;
            float effectRadius = 0.5f * towerTemplate.weapon[level].range;
            slowEffect.transform.localScale = new Vector3(effectRadius, effectRadius, effectRadius);
        }
		else
		{
            animator.runtimeAnimatorController = towerTemplate.weapon[level].animatorController;
		}
        towerSpawner.OnBuffAllTowers();
		return true;
	}

    public void Sell()
	{
        playerGold.curGold += towerTemplate.weapon[level].sell;
        ownerTile.isBuildTower = false;
		
        Destroy(transform.parent.gameObject);

        towerSpawner.OnBuffAllTowers();
    }
}
