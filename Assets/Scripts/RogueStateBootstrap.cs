using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RogueStateBootstrap : MonoBehaviour
{
    Material ground, road, building, blue, orange, playerMat;
    Transform player;
    RogueStateWorldState state;
    Text prompt, stats, missionText;
    GameObject panel;

    void Start()
    {
        ground = Mat(new Color(.20f,.28f,.25f)); road = Mat(new Color(.12f,.14f,.17f));
        building = Mat(new Color(.38f,.42f,.46f)); blue = Mat(new Color(.12f,.34f,.44f));
        orange = Mat(new Color(1f,.57f,.12f)); playerMat = Mat(new Color(.1f,.65f,.9f));
        BuildWorld(); BuildUI(); Setup();
    }
    Material Mat(Color c)
    {
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        if (s == null) s = Shader.Find("Standard");
        Material m = new Material(s); m.color = c; return m;
    }
    GameObject Prim(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat)
    {
        var g = GameObject.CreatePrimitive(type); g.name=name; g.transform.position=pos; g.transform.localScale=scale;
        var r=g.GetComponent<Renderer>(); if(r && mat) r.material=mat; return g;
    }
    void BuildWorld()
    {
        Prim("Ground",PrimitiveType.Plane,new Vector3(0,-.12f,4),new Vector3(3,1,3),ground);
        Prim("MainRoad",PrimitiveType.Cube,new Vector3(0,-.01f,4),new Vector3(7,.12f,24),road);
        Prim("LeftSidewalk",PrimitiveType.Cube,new Vector3(-4.2f,.03f,4),new Vector3(1.2f,.18f,24),blue);
        Prim("RightSidewalk",PrimitiveType.Cube,new Vector3(4.2f,.03f,4),new Vector3(1.2f,.18f,24),blue);
        for(int i=0;i<8;i++) Prim("RoadMarking_"+i,PrimitiveType.Cube,new Vector3(0,.06f,-6+i*3),new Vector3(.14f,.02f,1.1f),orange);
        Prim("Building_A",PrimitiveType.Cube,new Vector3(-7,2,1),new Vector3(4,4,5),building);
        Prim("Building_B",PrimitiveType.Cube,new Vector3(7,2,5),new Vector3(4,4,5),building);
        Prim("Building_C",PrimitiveType.Cube,new Vector3(-7,2,12),new Vector3(4,4,5),building);
        Prim("Building_D",PrimitiveType.Cube,new Vector3(7,2,15),new Vector3(4,4,5),building);
        player=Prim("Player",PrimitiveType.Capsule,new Vector3(-1.7f,1,-6),Vector3.one,playerMat).transform;
        player.gameObject.AddComponent<PlayerMovement>();
        Prim("InformationPoint",PrimitiveType.Cube,new Vector3(1.8f,.65f,4),new Vector3(.8f,1.3f,.8f),orange);
        Camera cam=Camera.main;
        if(!cam){GameObject cg=new GameObject("Main Camera");cg.tag="MainCamera";cam=cg.AddComponent<Camera>();}
        cam.transform.position=player.position+new Vector3(0,8,-9); cam.transform.LookAt(player.position);
        var follow=cam.GetComponent<CameraFollow>(); if(!follow) follow=cam.gameObject.AddComponent<CameraFollow>(); follow.target=player;
        var light=FindObjectOfType<Light>();
        if(!light){var lg=new GameObject("Directional Light"); light=lg.AddComponent<Light>(); light.type=LightType.Directional; lg.transform.rotation=Quaternion.Euler(50,-30,0);}
    }
    void BuildUI()
    {
        var cgo=new GameObject("RogueStateCanvas"); var c=cgo.AddComponent<Canvas>(); c.renderMode=RenderMode.ScreenSpaceOverlay;
        var scaler=cgo.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1280,720);
        cgo.AddComponent<GraphicRaycaster>();
        var es=new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>();
        stats=Txt(cgo.transform,"Stats","",19,TextAnchor.MiddleLeft);
        Anchor(stats.rectTransform,new Vector2(0,1),new Vector2(1,1),new Vector2(0,-12),new Vector2(-40,40));
        prompt=Txt(cgo.transform,"Prompt","",26,TextAnchor.MiddleCenter);
        Anchor(prompt.rectTransform,new Vector2(.2f,.12f),new Vector2(.8f,.22f),Vector2.zero,Vector2.zero);
        panel=new GameObject("MissionPanel"); panel.transform.SetParent(cgo.transform,false);
        panel.AddComponent<Image>().color=new Color(.035f,.05f,.075f,.97f);
        Anchor(panel.GetComponent<RectTransform>(),new Vector2(.16f,.15f),new Vector2(.84f,.85f),Vector2.zero,Vector2.zero);
        missionText=Txt(panel.transform,"MissionText","",20,TextAnchor.UpperLeft);
        Anchor(missionText.rectTransform,new Vector2(.06f,.49f),new Vector2(.94f,.94f),Vector2.zero,Vector2.zero);
        Button(panel.transform,"Report","REPORT",.04f,.28f,.29f,.43f,()=>state.Choose("Report"));
        Button(panel.transform,"Sell","SELL INFO (+$200)",.29f,.28f,.51f,.43f,()=>state.Choose("Sell"));
        Button(panel.transform,"Investigate","INVESTIGATE (-$20)",.52f,.28f,.75f,.43f,()=>state.Choose("Investigate"));
        Button(panel.transform,"Keep","KEEP SECRET",.76f,.28f,.96f,.43f,()=>state.Choose("Keep"));
        Button(panel.transform,"Close","CLOSE (Esc)",.38f,.04f,.62f,.19f,()=>state.Close());
        panel.SetActive(false);
    }
    Text Txt(Transform parent,string name,string value,int size,TextAnchor align)
    {
        var g=new GameObject(name);g.transform.SetParent(parent,false);var t=g.AddComponent<Text>();
        t.text=value;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=size;t.color=Color.white;t.alignment=align;
        t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;return t;
    }
    void Anchor(RectTransform r,Vector2 min,Vector2 max,Vector2 pos,Vector2 size)
    {r.anchorMin=min;r.anchorMax=max;r.anchoredPosition=pos;r.sizeDelta=size;r.offsetMin=pos;r.offsetMax=pos+size;}
    void Button(Transform p,string name,string label,float x1,float y1,float x2,float y2,UnityEngine.Events.UnityAction act)
    {
        var g=new GameObject(name);g.transform.SetParent(p,false);var im=g.AddComponent<Image>();im.color=new Color(.1f,.3f,.39f,1);
        var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(act);
        var r=g.GetComponent<RectTransform>();r.anchorMin=new Vector2(x1,y1);r.anchorMax=new Vector2(x2,y2);r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;
        var t=Txt(g.transform,"Label",label,14,TextAnchor.MiddleCenter);t.rectTransform.anchorMin=Vector2.zero;t.rectTransform.anchorMax=Vector2.one;t.rectTransform.offsetMin=new Vector2(4,4);t.rectTransform.offsetMax=new Vector2(-4,-4);
    }
    void Setup()
    {
        state=gameObject.AddComponent<RogueStateWorldState>();state.panel=panel;state.missionText=missionText;state.statsText=stats;
        var i=GameObject.Find("InformationPoint").AddComponent<OpportunityInteractable>();
        i.player=player;i.worldState=state;i.prompt=prompt;
    }
}