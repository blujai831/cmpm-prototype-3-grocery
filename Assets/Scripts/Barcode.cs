using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Barcode : MonoBehaviour
{
    public bool CorrectlyOriented() {
        return Math.Abs(Math.Sin(
            transform.rotation.eulerAngles.z*Math.PI/180.0
        )) <= 0.125;
    }
}
