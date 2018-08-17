using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just an example of how to use the ListAsset. In this case it's a list of 'things'
/// </summary>
namespace DataAssets
{
    [CreateAssetMenu(menuName = "DataAssets/CameraListAsset")]
    public class CameraListAsset : ListAsset<Camera>
    { }
}

