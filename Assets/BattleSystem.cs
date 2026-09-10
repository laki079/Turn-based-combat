using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject ememyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

	public BattleState state;

    private void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGo.GetComponent<Unit>();

        GameObject enemyGo = Instantiate(ememyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches you...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();

    }

    IEnumerator PlayerAttack()
    {
        // damage the enemy
        //check if enemy dead
        bool isdead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "attack succes";


        yield return new WaitForSeconds(2f);

        if (isdead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            // enemty turn
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        //change state based on what happened
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "Healed for 5 Hp";
        yield return new WaitForSeconds(2f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }


    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + "attaks!";
        yield return new WaitForSeconds(1f);
        bool isdead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);
        yield return new WaitForSeconds(1f);

        if (isdead)
        {
            //cheack if player is dead
            state = BattleState.LOST;
            EndBattle();

        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "You Won the Battle!";

        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "DEFEAT";
        }
            

    }



    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }


    public void OnAttakButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}
