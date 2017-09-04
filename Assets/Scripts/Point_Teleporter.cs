//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: teleport the user to where the controller is pointing.
//
//=============================================================================

// IMPORTANT: this script is modified from Valve's SteamVR plugin, 
// SteamVR_Teleporter.cs, located in SteamVR/Extras

using UnityEngine;
using System.Collections;

public class Point_Teleporter : MonoBehaviour
{
	public enum TeleportType
	{
		TeleportTypeUseTerrain,
		TeleportTypeUseCollider,
		TeleportTypeUseZeroY
	}

    public ModelManipulator m_modelManipulator;
    public GameObject ReticlePrefab;
    private GameObject m_reticle;
    private SteamVR_TrackedController m_trackedController;

    public bool teleportOnPadUnclick = false;
	public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;

	Transform reference
	{
		get
		{
			var top = SteamVR_Render.Top();
			return (top != null) ? top.origin : null;
		}
	}

	void Start()
	{
		m_trackedController = GetComponent<SteamVR_TrackedController>();
		if (m_trackedController == null)
		{
            m_trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
		}

        m_trackedController.PadUnclicked += new ClickedEventHandler(DoPadUnclick);

        if (teleportType == TeleportType.TeleportTypeUseTerrain)
		{
			// Start the player at the level of the terrain
			var t = reference;
			if (t != null)
				t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
        }

        m_reticle = Instantiate(ReticlePrefab);
        m_reticle.SetActive(false);
    }

	void DoPadUnclick(object sender, ClickedEventArgs e)
	{
        if (m_modelManipulator.m_manipulationMode) {
            return;
        }

		if (teleportOnPadUnclick)
		{
			// First get the current Transform of the the reference space (i.e. the Play Area, e.g. CameraRig prefab)
			var t = reference;
			if (t == null)
				return;

			// Get the current Y position of the reference space
			float refY = t.position.y;

			// Create a plane at the Y position of the Play Area
			// Then create a Ray from the origin of the controller in the direction that the controller is pointing
			Plane plane = new Plane(Vector3.up, -refY);
			Ray ray = new Ray(this.transform.position, transform.forward);

			// Set defaults
			bool hasGroundTarget = false;
			float dist = 0f;
			if (teleportType == TeleportType.TeleportTypeUseTerrain) // If we picked to use the terrain
			{
				RaycastHit hitInfo;
				TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
				hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
				dist = hitInfo.distance;
			}
			else if (teleportType == TeleportType.TeleportTypeUseCollider) // If we picked to use the collider
			{
				RaycastHit hitInfo;
				hasGroundTarget = Physics.Raycast(ray, out hitInfo);
				dist = hitInfo.distance;
			}
			else // If we're just staying flat on the current Y axis
			{
				// Intersect a ray with the plane that was created earlier
				// and output the distance along the ray that it intersects
				hasGroundTarget = plane.Raycast(ray, out dist);
			}

			if (hasGroundTarget)
			{
				// Get the current Camera (head) position on the ground relative to the world
				Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.position.x, refY, SteamVR_Render.Top().head.position.z);

				// We need to translate the reference space along the same vector
				// that is between the head's position on the ground and the intersection point on the ground
				// i.e. intersectionPoint - headPosOnGround = translateVector
				// currentReferencePosition + translateVector = finalPosition
				t.position = t.position + (ray.origin + (ray.direction * dist)) - headPosOnGround;
			}
		}
	}

    private void Update() {
        m_reticle.SetActive(false);
        if (m_modelManipulator.m_manipulationMode) {
            return;
        }

        if (m_trackedController.padPressed) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
                if (hit.transform.tag == "Floor") {
                    m_reticle.SetActive(true);
                    m_reticle.transform.position = new Vector3(hit.point.x, 0.01f, hit.point.z);
                }
            }
        }
    }
}

