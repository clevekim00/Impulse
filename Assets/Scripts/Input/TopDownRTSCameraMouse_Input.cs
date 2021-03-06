﻿/*****************************************
 * This file is part of Impulse Framework.

    Impulse Framework is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Impulse Framework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Impulse Framework.  If not, see <http://www.gnu.org/licenses/>.
*****************************************/

using UnityEngine;
using System.Collections;

public class TopDownRTSCameraMouse_Input : MonoBehaviour
{
	// Our Camera Object
	private Camera CameraObject;

	// Movement variables
	[Range(0,40)]
	public int
		PanThreshold = 20;
	[Range(0,50)]
	public float
		MovementSpeed = 10.0f;
	private Vector3 m_initialPosition;
	private Vector3 m_targetPosition = Vector3.zero;
	private Vector3 m_CCameraMoveVel = Vector3.zero;
	private float rotationY = 0F;
	private bool Rotating = false;
	[Range(0.1f,50.0f)]
	public float
		rotationSpeed = 15F; 
	
	// Zoom variables
	[Range(0,10)]
	public float
		MaxZoom = 5.0f;
	[Range(-10,0)]
	public float
		MinZoom = -2.0f;
	[Range(0,50)]
	public float
		zoomSpeed = 10.0f;
	private float InitialZoom;
	private float m_targetZoom;
	private Vector3 m_CCameraZoomVel = Vector3.zero;
	
	// Rotation variables
	public float MaxRotation = 25;
	[Range(0,50)]
	public float
		RotationSpeed = 10.0f;
	private float InitialRotation;
	private float m_CCameraRotateVel = 0;


	// General update movement
	[Range(0.01f,2.0f)]
	public float
		CameraMoveSpeed = 0.3f;
	
	void Start ()
	{
		// Assign our camera object.
		CameraObject = GetComponentInChildren<Camera> ();

		// Set initial target position
		m_initialPosition = transform.position;
		m_targetPosition = m_initialPosition;
		
		// Get initial Camera Zoom
		InitialZoom = Mathf.Clamp(CameraObject.transform.localPosition.z,MinZoom,MaxZoom);
		m_targetZoom = InitialZoom;
		
		// Fix the camera systemheight (this never changes)
		m_targetPosition.y = transform.position.y;
		
		// Force initial camera position
		transform.position = m_targetPosition;

	}
	
	////////////////////////////////////////////////////
	/// GAMEPLAY									  //
	///////////////////////////////////////////////////
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void LateUpdate ()
	{
		// Update the camera position
		UpdatePosition ();

		// Update the camera rotation
		UpdateCameraRotation ();
		
		// Execute Camera Zoom
		UpdateZoom ();
	}

	/// <summary>
	/// Updates the camera rotation.
	/// </summary>
	private void UpdateCameraRotation ()
	{
		Rotating = false;
		if (Input.GetMouseButton (1)) {
			Rotating = true;
			rotationY += Input.GetAxis ("Mouse X") * rotationSpeed;
			transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, rotationY, 0);
		}
	}
	
	/// <summary>
	/// Updates the zoom.
	/// </summary>
	private void UpdateZoom ()
	{
		// Zoom function
		float DeltaZoom = Input.GetAxis ("Mouse ScrollWheel");
		if (!(Mathf.Approximately (DeltaZoom, 0f))) {
			if (DeltaZoom < 0) {
				m_targetZoom = Mathf.Clamp (m_targetZoom - Time.deltaTime * zoomSpeed, MinZoom, MaxZoom);
			} else {
				m_targetZoom = Mathf.Clamp (m_targetZoom + Time.deltaTime * zoomSpeed, MinZoom, MaxZoom);
			}
		}
		
		// Execute zoom
		Vector3 targetLocalPosition = CameraObject.transform.localPosition;
		targetLocalPosition.z = m_targetZoom;
		CameraObject.transform.localPosition = Vector3.SmoothDamp (CameraObject.transform.localPosition, targetLocalPosition,
		                                                          ref m_CCameraZoomVel, CameraMoveSpeed);
		
		// Get normalized required rotation
		float normRotation = Mathf.Clamp (m_targetZoom / MaxZoom, 0, 1);
		float targetEulerAngles = normRotation * (-MaxRotation);
		float currentEulerAngles = CameraObject.transform.localEulerAngles.x;
		currentEulerAngles = Mathf.SmoothDampAngle (currentEulerAngles, targetEulerAngles, ref m_CCameraRotateVel, CameraMoveSpeed);
		
		// Execute rotation
		Vector3 finalRotation = CameraObject.transform.localEulerAngles;
		finalRotation.x = currentEulerAngles;
		CameraObject.transform.localEulerAngles = finalRotation;
	}

	/// <summary>
	/// Updates the camera movement.
	/// </summary>
	private void UpdatePosition ()
	{
		// Get camera edge
		Vector2 mouseEdge = MouseScreenEdge (PanThreshold);
		
		if (!(Mathf.Approximately (mouseEdge.x, 0f))) {
			//Move your camera depending on the sign of mouse.Edge.x
			if (mouseEdge.x < 0) {
				m_targetPosition -= transform.right * Time.deltaTime * MovementSpeed;
			} else {
				m_targetPosition += transform.right * Time.deltaTime * MovementSpeed;
			}
		}
		if (!(Mathf.Approximately (mouseEdge.y, 0f))) {
			//Move your camera depending on the sign of mouse.Edge.y
			if (mouseEdge.y < 0) {
				m_targetPosition -= transform.forward * Time.deltaTime * MovementSpeed;
			} else {
				m_targetPosition += transform.forward * Time.deltaTime * MovementSpeed;
			}
		}
		
		// Fix the camera height (this never changes)
		m_targetPosition.y = transform.position.y;
		
		// Update the position
		transform.position = Vector3.SmoothDamp (transform.position, m_targetPosition, ref m_CCameraMoveVel, CameraMoveSpeed);
	}

	////////////////////////////////////////////////////
	/// HELPERS										  //
	///////////////////////////////////////////////////
	/// <summary>
	/// Gets if the mouse is hitting the edge of the screen
	/// </summary>
	/// <returns>The screen edge.</returns>
	/// <param name="margin">Margin.</param>
	Vector2 MouseScreenEdge (int margin)
	{
		if (Rotating)
			return Vector2.zero;
		
		//Margin is calculated in px from the edge of the screen
		Vector2 half = new Vector2 (Screen.width / 2, Screen.height / 2);
		
		//If mouse is dead center, (x,y) would be (0,0)
		float x = Input.mousePosition.x - half.x;
		float y = Input.mousePosition.y - half.y;   
		
		//If x is not within the edge margin, then x is 0;
		//In another word, not close to the edge
		if (Mathf.Abs (x) > half.x - margin) {
			x += (half.x - margin) * ((x < 0) ? 1 : -1);
		} else {
			x = 0f;
		}
		
		if (Mathf.Abs (y) > half.y - margin) {
			y += (half.y - margin) * ((y < 0) ? 1 : -1);
		} else {
			y = 0f;
		}
		return new Vector2 (x, y);
	}
}
