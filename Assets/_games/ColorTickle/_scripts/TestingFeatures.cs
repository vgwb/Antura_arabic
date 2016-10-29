using UnityEngine;
using System.Collections;

namespace EA4S.ColorTickle
{
    public class TestingFeatures : MonoBehaviour
    {

        public LetterObjectView livinglettertxt;
        public float OutlineThickness=0;
        public float FontSize = 10f;
        public string Text = "1";
        public bool enableAutosize = false;
        public Color32 color;
       
        // Use this for initialization
        void Start()
        {
            livinglettertxt.Lable.overrideColorTags=true;
        }

        // Update is called once per frame
        void Update()
        {

            // if(Input.GetKeyUp("c"))
            livinglettertxt.Lable.outlineColor = color;
            livinglettertxt.Lable.SetText(Text);
               
                livinglettertxt.Lable.fontSize= FontSize;
                livinglettertxt.Lable.outlineWidth= OutlineThickness;
            
          
                livinglettertxt.Lable.enableAutoSizing = enableAutosize;//when true le text is set between a min and max size depending on the text container size
            
        }
    }
}
