﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarKinematics : MonoBehaviour
{
    [Header("Player Controller")]
    public string m_HorizontalAxisName = "Horizontal";
    public string m_VerticalAxisName = "Vertical";
    public string m_BrakeButtonName = "Jump";

    [Header("Wheel Colliders")]
    public WheelCollider m_WheelColliderFL;
    public WheelCollider m_WheelColliderFR;
    public WheelCollider m_WheelColliderRL;
    public WheelCollider m_WheelColliderRR;

    [Header("Wheel Meshes")]
    public Transform m_WheelMeshFL;
    public Transform m_WheelMeshFR;
    public Transform m_WheelMeshRL;
    public Transform m_WheelMeshRR;

    [Header("Steering")]
    public float m_MinSteerAngle = 15.0f;
    public float m_MaxSteerAngle = 25.0f;
    public float m_MaxSpeedToSteerAngle = 60.0f;
    public float m_SmoothSteeringAngle = 10.0f;
    public bool m_UseStabilityCurves = true;

    [Header("Physics")]
    public Transform m_CenterOfGravity;
    public float m_MaxMotorTorque = 1500.0f;
    public float m_MaxReverseTorque = 500.0f;
    public float m_MaxDecelerationForce = 200.0f;
    public float m_BrakeForce = 3000.0f;

    [Header("Drive Mode")]
    public DriveMode m_DriveMode = DriveMode.All;
    public SpeedMode m_SpeedMode = SpeedMode.KilometersPerHours;
    public enum DriveMode { Front, Rear, All };
    public enum SpeedMode { MetersPerSeconds, KilometersPerHours, MilesPerHours };

    public bool IsReverse => m_WheelColliderRL.rpm < 0.0f && m_WheelColliderRR.rpm < 0.0f;

    private Rigidbody m_Body;
    private float m_HorizontalInput;
    private float m_VerticalInput;
    private bool m_BrakeInput;

    private void Start() {
        m_Body = GetComponent<Rigidbody>();
        m_Body.centerOfMass = m_CenterOfGravity.localPosition;
    }

    private void Update() {
        m_HorizontalInput = Input.GetAxis(m_HorizontalAxisName);
        m_VerticalInput = Input.GetAxis(m_VerticalAxisName);
        m_BrakeInput = Input.GetButton(m_BrakeButtonName);
    }

    private void FixedUpdate(){
        Steering();
        Accelerate();
        Braking();
        Decelerate();
        UpdateMeshes();
    }

    private void Steering()
    {
        float steerAngle = 0.0f;
        if (m_UseStabilityCurves)
        {
            float speedFactor = GetSpeed() / m_MaxSpeedToSteerAngle;
            steerAngle = Mathf.Lerp(m_MaxSteerAngle, m_MinSteerAngle, speedFactor) * m_HorizontalInput;
        }
        else
        {
            steerAngle = m_MaxSteerAngle * m_HorizontalInput;
        }

        m_WheelColliderFL.steerAngle = Mathf.Lerp(m_WheelColliderFL.steerAngle, steerAngle, Time.deltaTime * m_SmoothSteeringAngle);
        m_WheelColliderFR.steerAngle = Mathf.Lerp(m_WheelColliderFR.steerAngle,steerAngle, Time.deltaTime * m_SmoothSteeringAngle);
    }

    public float GetSpeed()
    {
        if (m_SpeedMode == SpeedMode.KilometersPerHours) return m_Body.velocity.magnitude * 3.6f;
        if (m_SpeedMode == SpeedMode.MilesPerHours) return m_Body.velocity.magnitude * 2.237f;
        return m_Body.velocity.magnitude;
    }

    private void UpdateMesh(WheelCollider collider, Transform wheel)
    {
        Vector3 position = wheel.position;
        Quaternion rotation = wheel.rotation;
        collider.GetWorldPose(out position, out rotation);
        rotation *= Quaternion.Euler(new Vector3(0.0f, 0.0f, 90.0f));
        wheel.position = position;
        wheel.rotation = rotation;
    }

    private void UpdateMeshes()
    {
        UpdateMesh(m_WheelColliderFL, m_WheelMeshFL);
        UpdateMesh(m_WheelColliderFR, m_WheelMeshFR);
        UpdateMesh(m_WheelColliderRL, m_WheelMeshRL);
        UpdateMesh(m_WheelColliderRR, m_WheelMeshRR);
    }

    private void Decelerate()
    {
        float motorTorque = m_VerticalInput * m_MaxDecelerationForce;
        if (motorTorque == 0.0f && !m_BrakeInput)
        {
            m_WheelColliderFL.brakeTorque = motorTorque;
            m_WheelColliderFR.brakeTorque = motorTorque;
            m_WheelColliderRL.brakeTorque = motorTorque;
            m_WheelColliderRR.brakeTorque = motorTorque;
        }
    }

    private void Braking()
    {
        float brakeTorque = m_BrakeInput ? m_BrakeForce : 0.0f;
        m_WheelColliderFL.brakeTorque = brakeTorque;
        m_WheelColliderFR.brakeTorque = brakeTorque;
        m_WheelColliderRL.brakeTorque = brakeTorque;
        m_WheelColliderRR.brakeTorque = brakeTorque;
    }

    private void Accelerate()
    {
        float motorTorque = m_VerticalInput > 0.0f ?
                            m_VerticalInput * m_MaxMotorTorque :
                            m_VerticalInput * m_MaxReverseTorque;

        m_WheelColliderFL.motorTorque = m_DriveMode == DriveMode.Rear ? 0.0f : motorTorque;
        m_WheelColliderFR.motorTorque = m_DriveMode == DriveMode.Rear ? 0.0f : motorTorque;
        m_WheelColliderRL.motorTorque = m_DriveMode == DriveMode.Front ? 0.0f : motorTorque;
        m_WheelColliderRR.motorTorque = m_DriveMode == DriveMode.Front ? 0.0f : motorTorque;
    }
}

