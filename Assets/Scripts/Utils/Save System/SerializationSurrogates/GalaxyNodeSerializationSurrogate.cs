﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GalaxyNodeSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        GalaxyNode node = (GalaxyNode)obj;
        info.AddValue("xPos", node.position.x);
        info.AddValue("yPos", node.position.y);
        info.AddValue("zPos", node.position.z);
        info.AddValue("id", node.nodeID);
        info.AddValue("ring", node.currentRing);
        info.AddValue("features", node.features);
        info.AddValue("resources", node.resources);
        info.AddValue("ships", node.ships);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        GalaxyNode node = (GalaxyNode)obj;
        node.position.x = (float)info.GetValue("xPos", typeof(float));
        node.position.y = (float)info.GetValue("yPos", typeof(float));
        node.position.z = (float)info.GetValue("zPos", typeof(float));
        node.nodeID = (int)info.GetValue("id", typeof(int));
        node.currentRing = (int)info.GetValue("ring", typeof(int));
        node.features = (List<GalaxyNode.SystemFeatures>)info.GetValue("features", typeof(List<GalaxyNode.SystemFeatures>));
        node.resources = (List<GalaxyNodeResourceData>)info.GetValue("resources", typeof(List<GalaxyNodeResourceData>));
        node.ships = (List<ShipData>)info.GetValue("ships", typeof(List<ShipData>));

        obj = node;
        return obj;
    }

}
