using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class FactionDataSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        FactionData data = (FactionData)obj;
        info.AddValue("name", data.factionName);
        info.AddValue("id", data.factionID);
        info.AddValue("colorR", data.factionColour.r);
        info.AddValue("colorG", data.factionColour.g);
        info.AddValue("colorB", data.factionColour.b);
        info.AddValue("colorA", data.factionColour.a);
        info.AddValue("capitulation", data.hasCapitulated);
        info.AddValue("homeSystem", data.homeSystem.nodeID);
        info.AddValue("explored", data.exploredSystemIDs);
        info.AddValue("controlled", data.ownedSystemIDs);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        FactionData data = (FactionData)obj;
        data.factionName = (string)info.GetValue("name", typeof(string));
        data.factionID = (int)info.GetValue("id", typeof(int));
        data.factionColour.r = (float)info.GetValue("colorR", typeof(float));
        data.factionColour.g = (float)info.GetValue("colorG", typeof(float));
        data.factionColour.b = (float)info.GetValue("colorB", typeof(float));
        data.factionColour.a = (float)info.GetValue("colorA", typeof(float));
        data.hasCapitulated = (bool)info.GetValue("capitulation", typeof(bool));
        data.homeSystemID = (int)info.GetValue("homeSystem", typeof(int));
        data.exploredSystemIDs = (List<int>)info.GetValue("explored", typeof(List<int>));
        data.ownedSystemIDs = (List<int>)info.GetValue("controlled", typeof(List<int>));

        obj = data;
        return obj;
    }
}
