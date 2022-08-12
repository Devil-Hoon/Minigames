using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
	[SerializeField]
	private Image imageTower;
	[SerializeField]
	private TextMeshProUGUI textDamage;
	[SerializeField]
	private TextMeshProUGUI textRate;
	[SerializeField]
	private TextMeshProUGUI textRange;
	[SerializeField]
	private TextMeshProUGUI textLevel;
	[SerializeField]
	private TextMeshProUGUI textUpgradeCost;
	[SerializeField]
	private TextMeshProUGUI textSellCost;
	[SerializeField]
	private TowerAttackRange towerAttackRange;
	[SerializeField]
	private Button buttonUpgrade;
	[SerializeField]
	private SystemTextViewer systemTextViewer;

	private TowerWeapon currentTower;

	public bool buildable = false;
	private void Awake()
	{
		OffPanel();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OffPanel();
		}
	}

	public void OnPanel(Transform towerWeapon)
	{
		currentTower = towerWeapon.GetComponent<TowerWeapon>();
		gameObject.SetActive(true);
		UpdateTowerData();
		towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.range);
	}

	public void OffPanel()
	{
		gameObject.SetActive(false);
		towerAttackRange.OffAttackRange();
	}

	private void UpdateTowerData()
	{
		if (currentTower.wType == WeaponType.Cannon)
		{
			imageTower.rectTransform.sizeDelta = new Vector2(80, 150);
			imageTower.rectTransform.anchoredPosition = new Vector3(0, 150, 0);
			imageTower.rectTransform.rotation = Quaternion.Euler(0, 0, -90);
			textDamage.text = "Damage : " + currentTower.damage
				+ "+" + "<color=red>" + currentTower.addDamage.ToString("F1") + "</color>";
		}
		else if (currentTower.wType == WeaponType.Laser)
		{
			imageTower.rectTransform.sizeDelta = new Vector2(150, 100);
			imageTower.rectTransform.anchoredPosition = new Vector3(0, 150, 0);
			imageTower.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
			textDamage.text = "Damage : " + currentTower.damage
				+ "+" + "<color=red>" + currentTower.addDamage.ToString("F1") + "</color>";
		}
		else
		{
			if (currentTower.wType == WeaponType.Slow)
			{
				imageTower.rectTransform.sizeDelta = new Vector2(80, 80);
				imageTower.rectTransform.anchoredPosition = new Vector3(0, 150, 0);
				imageTower.rectTransform.rotation = Quaternion.identity;
				textDamage.text = "Slow : " + currentTower.slow * 100 + "%";
			}
			else if (currentTower.wType == WeaponType.Buff)
			{
				imageTower.rectTransform.sizeDelta = new Vector2(80, 80);
				imageTower.rectTransform.anchoredPosition = new Vector3(0, 150, 0);
				imageTower.rectTransform.rotation = Quaternion.identity;
				textDamage.text = "Buff : " + currentTower.buff * 100 + "%";
			}
		}
		imageTower.sprite = currentTower.towerSprite;
		textRate.text = "Rate : " + currentTower.rate;
		textRange.text = "Range : " + currentTower.range;
		textLevel.text = "Level : " + currentTower.lev;
		textUpgradeCost.text = currentTower.upgradeCost.ToString();
		textSellCost.text = currentTower.sellCost.ToString();

		if (buildable)
		{
			buttonUpgrade.interactable = currentTower.lev < currentTower.maxLevel ? true : false;
		}
	}

	public void OnClickEventTowerUpgrade()
	{
		if (buildable)
		{
			bool isSuccess = currentTower.Upgrade(); 

			if (isSuccess)
			{
				UpdateTowerData();
				towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.range);
			}
			else
			{
				systemTextViewer.PrintText(SystemType.Money);
			}
		}

		
	}

	public void OnClickEventTowerSell()
	{
		if (buildable)
		{
			currentTower.Sell();
			OffPanel();
		}
	}
}
