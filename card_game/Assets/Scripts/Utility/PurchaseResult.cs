using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PurchaseResult
{
    public enum ResourceType
    {
        Gold,
        Pack,
        Card
    }
    public string Id;
    [JsonConverter(typeof(StringEnumConverter))]
    public ResourceType Resource;
    public uint Amount;
}
