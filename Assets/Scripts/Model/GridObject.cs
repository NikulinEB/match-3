using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridObject", menuName = "Grid Object", order = 51)]
public class GridObject : ScriptableObject
{
    public ObjectColor Color { get { return color; } }
    public ObjectShape Shape { get { return shape; } }
    public ObjectLines Lines { get { return lines; } }
    public Sprite Sprite { get { return sprite; } }

    [SerializeField] protected ObjectColor color;
    [SerializeField] protected ObjectShape shape;
    [SerializeField] protected ObjectLines lines;
    [SerializeField] protected Sprite sprite;
}
