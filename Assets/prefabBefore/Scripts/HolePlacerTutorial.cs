using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class HolePlacerTutorial : MonoBehaviour
{
	bool isGenerated = false;
	public GameObject holesPrefab;

	void HitTest(ARPoint point)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface
			.GetARSessionNativeInterface()
			.HitTest(point, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);

		// 平面とあたっていた場合
		if (hitResults.Count > 0)
		{
			GameObject holes = Instantiate(holesPrefab);
			holes.transform.position = UnityARMatrixOps.GetPosition(hitResults[0].worldTransform);
			holes.transform.rotation = UnityARMatrixOps.GetRotation(hitResults[0].worldTransform);

			this.isGenerated = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!isGenerated && Input.touchCount > 0)
		{
			var touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
				ARPoint point = new ARPoint
				{
					x = screenPosition.x,
					y = screenPosition.y
				};

				// 平面との当たり判定
				HitTest(point);
			}
		}
	}
}