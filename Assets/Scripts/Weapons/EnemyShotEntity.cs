using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    class EnemyShotEntity
    {
        public int ShooterId { get; set; }
        public float Time { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }
    }
}
