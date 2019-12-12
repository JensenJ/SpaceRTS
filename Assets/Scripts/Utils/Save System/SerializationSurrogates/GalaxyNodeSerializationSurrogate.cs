using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GalaxyNodeSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        GalaxyNode node = (GalaxyNode)obj;
        info.AddValue("x", node.position.x);
        info.AddValue("y", node.position.y);
        info.AddValue("z", node.position.z);
        info.AddValue("ring", node.currentRing);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        GalaxyNode node = (GalaxyNode)obj;
        node.position.x = (float)info.GetValue("x", typeof(float));
        node.position.y = (float)info.GetValue("y", typeof(float));
        node.position.z = (float)info.GetValue("z", typeof(float));
        node.currentRing = (int)info.GetValue("ring", typeof(int));

        obj = node;
        return obj;
    }

}
