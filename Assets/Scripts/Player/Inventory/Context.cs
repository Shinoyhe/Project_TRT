using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[Serializable]
public class ContextInfo
{
    public bool relevant;
    public string context;
}

[Serializable]
public class ContextOriginPair
{
    public ContextOrigins origin;
    public ContextInfo contextInfo;
}