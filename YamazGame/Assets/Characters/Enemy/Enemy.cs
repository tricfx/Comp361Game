using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Callbacks;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEnemy {
    
    public abstract void Attack();

}