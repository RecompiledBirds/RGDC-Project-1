using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager<T> where T : Object
{
    private static Dictionary<string,T> resources = new Dictionary<string,T>();
    public static T LoadResource(string path)
    {
        if(resources.ContainsKey(path)) return resources[path];
        T resource = (T)Resources.Load(path);
        resources[path] = resource;
        return resource;
    }

}
