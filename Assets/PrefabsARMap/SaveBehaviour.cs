using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SaveBehaviour : MonoBehaviour
{
    

    public void onSaveClick()
    {
        //save each panel
            firestore.checkFile--;
            AddGameObject.alreadyAdd = false; 
            GameObject root = GameObject.Find("InfoPanel"); 
            GameObject media = GameObject.Find("InfoPanel/Canvas/Media");
            bool Ispublic = GameObject.Find("InfoPanel/Canvas/PublicToggle").GetComponent<Toggle>().isOn;
            Texture2D mediaPhoto = media.GetComponent<CanvasRenderer>().GetMaterial().mainTexture as Texture2D;
            string title = GameObject.Find("InfoPanel/Canvas/TitlePanel").GetComponentInChildren<InputField>().text;
            string contents = GameObject.Find("InfoPanel/Canvas/TextPanel").GetComponentInChildren<InputField>().text;
            GameObject InfoPanel2Instantiate;
            GameObject m_InfoPanel;
            if (StaticObject.photoAdded)
            {
                InfoPanel2Instantiate = Resources.Load<GameObject>("InfoPanelOut");
                m_InfoPanel = Instantiate(InfoPanel2Instantiate, root.transform.position, root.transform.rotation);
                m_InfoPanel.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = mediaPhoto;
            }
            else
            {
                 InfoPanel2Instantiate = Resources.Load<GameObject>("InfoPanelNoImage");
                 m_InfoPanel = Instantiate(InfoPanel2Instantiate, root.transform.position, root.transform.rotation);
            }

            m_InfoPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = title;
            m_InfoPanel.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = contents;
            Destroy(root);
            string g_GUID = StaticObject.getGUID();
            m_InfoPanel.name = g_GUID;
            List<float> myPosition = new List<float>() { m_InfoPanel.transform.position.x, m_InfoPanel.transform.position.y, m_InfoPanel.transform.position.z,
                     m_InfoPanel.transform.rotation.x, m_InfoPanel.transform.rotation.y, m_InfoPanel.transform.rotation.z,m_InfoPanel.transform.rotation.w };
            StaticObject.addedGO.Add(myPosition, new List<string>() { InfoPanel2Instantiate.name, title, contents, g_GUID,Auth.UserSelfId,Ispublic.ToString(),StaticObject.currentOrder.ToString(),m_InfoPanel.transform.localScale.ToString() });
            StaticObject.currentOrder++;

            mediaPhoto = StaticObject.duplicateTexture(mediaPhoto);

            byte[] mediaImageFile = ImageConversion.EncodeToJPG(mediaPhoto);
            
            StaticCoroutine.DoCoroutine(firestore.WriteFile(mediaImageFile, "Images/" + g_GUID + ".jpg"));
            CommonVariables.writeNewPlace2(g_GUID,title,contents,CommonVariables.ConvertARCoordinate2GPS(root.transform.position), Auth.UserSelfId, Ispublic.ToString());
            //StaticObject.currentOrder++;
            firestore.PrecisARList.Add(m_InfoPanel);//firestore.PrecisARList.Insert(StaticObject.currentOrder,m_InfoPanel);
            //StaticObject.currentOrder++;
            GameObject target = Instantiate(CompassController.radarPrefab, m_InfoPanel.transform.position, Quaternion.identity);
            target.tag = "Target";
            target.name = m_InfoPanel.name + "Radar";
            CompassController.radarList.Add(target);
            GameObject targetBorder = Instantiate(CompassController.radarPrefab, m_InfoPanel.transform.position, Quaternion.identity);
            CompassController.borderList.Add(targetBorder);
            targetBorder.tag = "Target";
            targetBorder.name = m_InfoPanel.name + "Border";
    }

        
    public void onDelete()
    {
        AddGameObject.alreadyAdd = false;
        Destroy(GameObject.Find("InfoPanel"));
    }


    
}



