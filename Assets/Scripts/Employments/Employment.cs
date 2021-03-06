﻿using UnityEngine;

[System.Serializable]
public class Employment
{
    [HideInInspector] public Workplace workplace;
    [HideInInspector] public Citizen employee;
    public string employmentName = "worker";

    [SerializeField] BodypartSpriteGroup workClothing = null; public BodypartSpriteGroup WorkClothing { get => workClothing; }

    public void Employ(Citizen citizen) => employee = citizen;
    public void QuitJob() => employee = null;

    public bool PositionFilled { get => employee != null; }
    public virtual Task GetWorkTask(Citizen citizen) => workplace.GetWorkTask(citizen);
    public virtual bool ShiftActive => workplace.ShiftActive;
}
