﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class HammerSoldier : Soldier
{
    float myHealth;
    Animator hammerSoldierAnimator;
    bool startWalking = false;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        SetMyProperties();
        myHealth = health;
        hammerSoldierAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((currentState == States.WALK || currentState == States.SET) && !startWalking)
        {
            startWalking = true;
        }
        healthSlider.GetComponent<Image>().fillAmount = health / myHealth;
        //Debug.Log(currentState);
        if (currentState == States.ATTACK && currentTarget != null)
        {
            startWalking = false;
            if (Time.time > lastShootTime + timeBetweenShoots)
            {
                lastShootTime = Time.time;
                Shoot();
            }
            else
            {
                //this.gameObject.GetComponent<Renderer>().material = originalMaterial;
            }
        }
        if (currentState == States.SET && this.transform.position == agent.destination + Vector3.up * 0.4f && currentTarget != null)
        {
            currentState = States.ATTACK;
        }
        if (startWalking)
        {
            hammerSoldierAnimator.ResetTrigger("attackTrigger");
            hammerSoldierAnimator.SetTrigger("moveTrigger");
        }
    }

    protected override bool CheckCondition(Transform t, int[] d)
    {// checks condition to add tower in list
        if (d[Constants.HAMMER_SOLDIER] < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        base.OnSoldierColliderEntry(col);
    }

    void Shoot()
    {
        hammerSoldierAnimator.ResetTrigger("moveTrigger");
        hammerSoldierAnimator.SetTrigger("attackTrigger");
        Entity entity = currentTarget.gameObject.GetComponent<Entity>();
        int type_of_target = GameManager.Instance().GetDefenseType(entity.GetMyName());
        entity.TakeDamage(damage * damagePercentage[type_of_target]/100);
    }

    protected override void SetMyProperties()
    {
        int level = StatisticsManager.SM.GetDetails("Hammer_Soldier_State");
        int type = Constants.HAMMER_SOLDIER;
        if (level >= 1)
        {
            if (level > 3)
                level = 1;
            myProperties = StatisticsManager.SM.GetSoldierProperties(type, level);
            myFirstName = myProperties.myFirstname;
            health = myProperties.health;
            //originalMaterial = myProperties.originalMaterial;
            timeBetweenShoots = myProperties.timeBetweenShoots;
            damage = myProperties.damage;
            damagePercentage = myProperties.damagePercentage;
            target_priority = myProperties.priority;
        }
        else
        {
            Debug.LogError("You have still not bought the Arrow Soldier");
        }
    }
}
