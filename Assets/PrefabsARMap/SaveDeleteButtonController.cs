using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attach this script to Prefab to instantiate!
public class SaveDeleteButtonController : MonoBehaviour
{
	public void onSaveNormalObject()
	{
        StaticObject.AddClicked = false;
		AddGameObject.alreadyAdd = false;
		GameObject g = Instantiate(Resources.Load<GameObject>(gameObject.name + "Simple"), transform.position, transform.rotation);
        g.transform.localScale = transform.localScale;
		string g_GUID = StaticObject.getGUID();
        firestore.PrecisARList.Add(g);  //firestore.PrecisARList.Insert(StaticObject.currentOrder, g);
        //StaticObject.currentOrder++;
        List<float> myPosition = new List<float>() { g.transform.position.x, g.transform.position.y, g.transform.position.z,
					 g.transform.rotation.x, g.transform.rotation.y, g.transform.rotation.z,g.transform.rotation.w };
		StaticObject.addedGO.Add(myPosition, new List<string>() { gameObject.name + "Simple", null, null, g_GUID, Auth.UserSelfId, true.ToString(),StaticObject.currentOrder.ToString(),g.transform.localScale.ToString() });
        StaticObject.currentOrder++;
        print(g.transform.localScale.ToString());
        print("add to addedGO  "+ gameObject.name + "Simple");
		//StaticObject.addedGO.Add(myPosition, new List<string>() { InfoPanel2Instantiate.name, title, contents, g_GUID, Auth.UserSelfId, Ispublic.ToString() });
		GameObject target = Instantiate(CompassController.radarPrefab, g.transform.position, Quaternion.identity);
		target.tag = "Target";
		target.name = g.name + "Radar";
		CompassController.radarList.Add(target);
		GameObject targetBorder = Instantiate(CompassController.radarPrefab, g.transform.position, Quaternion.identity);
		CompassController.borderList.Add(targetBorder);
		targetBorder.tag = "Target";
		targetBorder.name = g.name + "Border";
		print("just destroyed!");
		Destroy(gameObject);
	}
	public void OnDelete()
	{
        StaticObject.AddClicked = false;
        AddGameObject.alreadyAdd = false;
        Destroy(gameObject);

	}
}
