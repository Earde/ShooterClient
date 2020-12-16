using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// User input at client game time
/// tRight, tForward are camera directions
/// </summary>
class UserInput
{
    public Vector3 tRight { get; set; }
    public Vector3 tForward { get; set; }
    public bool[] inputs { get; set; }
    public float time { get; set; }
}