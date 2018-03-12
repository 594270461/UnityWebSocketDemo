﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;

public class MyTransformReceiver : DataReceiver {
  // Local store for incoming object data
  private List<MyTransform> data;
  private MyTransform currentValue;

  [SerializeField]
  private float AnimationTime = 0.5f; // seconds
  private float currentTime = 0f;

  private Vector3 originalPosition;
  private Quaternion originalRotation;
  private Vector3 originalScale;

  private void reset() {
    originalPosition = transform.localPosition;
    originalRotation = transform.localRotation;
    originalScale = transform.localScale;
  }

  // Use this for initialization
  void Start() {
    reset();
  }

  // Update is called once per frame
  void Update() {
    if (data == null || data.Count == 0) {
      return;
    }

    if (currentValue == null) {
      // Set current value and remove from local store
      currentValue = data[0];
      data.RemoveAt(0);
    }

    // Lerp transform: position, rotation and scale
    if (currentTime <= AnimationTime) {
      currentTime += Time.deltaTime;

      float x, y, z, rate;
      rate = currentTime / AnimationTime;
      x = Mathf.Lerp(originalPosition.x, currentValue.position.x, rate);
      y = Mathf.Lerp(originalPosition.y, currentValue.position.y, rate);
      z = Mathf.Lerp(originalPosition.z, currentValue.position.z, rate);
      transform.localPosition = new Vector3(x, y, z);

      x = Mathf.Lerp(originalRotation.eulerAngles.x, currentValue.rotation.x, rate);
      y = Mathf.Lerp(originalRotation.eulerAngles.y, currentValue.rotation.y, rate);
      z = Mathf.Lerp(originalRotation.eulerAngles.z, currentValue.rotation.z, rate);
      transform.localRotation = Quaternion.Euler(x, y, z);

      x = Mathf.Lerp(originalScale.x, currentValue.scale.x, rate);
      y = Mathf.Lerp(originalScale.y, currentValue.scale.y, rate);
      z = Mathf.Lerp(originalScale.z, currentValue.scale.z, rate);
      transform.localScale = new Vector3(x, y, z);
    } else {
      currentTime = 0;
      reset();
      // Update current value and remove from local store
      currentValue = data[0];
      data.RemoveAt(0);
    }
  }

  public override void OnReceivedData(object sender, EventArgs args) {
    if (args == null) {
      return;
    }

    // return early if wrong type of EventArgs
    var myArgs = args as MyTransformEventArgs;
    if (myArgs == null) {
      return;
    }

    // Add object data into local store
    if (data == null) {
      data = new List<MyTransform>();
    }
    data.Add(myArgs.Data);
  }

}
