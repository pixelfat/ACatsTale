﻿using UnityEngine; public class AutoRotate : MonoBehaviour{ 	public float m_Speed = 100f;	public bool isRotateX = false;	public bool isRotateY = false;	public bool isRotateZ = false;	void Update ()	{		if (isRotateX)			transform.Rotate (m_Speed * Time.deltaTime, 0, 0);		if (isRotateY)			transform.Rotate (0, m_Speed * Time.deltaTime, 0);		if (isRotateZ)			transform.Rotate (0, 0, m_Speed * Time.deltaTime);	}}