using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gg
{
    public class VersionLanguage : MonoBehaviour
    {
        [SerializeField] private BibleBrain _bibleBrain;
        [SerializeField] private ButtonInstantiator _buttonInstantiator;
        
        public string CurrentBibleVersionOT;
        public string CurrentBibleVersionNT;
        
        // Language Toggles
        [SerializeField] private Toggle EnglishTog;
        
        // Bible Version Toggles GameObject Parents 
        [SerializeField] GameObject EnglishTogglesParent;


        // Start is called before the first frame update
        void Start()
        {
             AddLanguageButtons();
        }

        private void AddLanguageButtons()
        {
            _bibleBrain.GetAllLanguages();
            //_bibleBrain.GetEnglishLanguages();
        }

        public void SetCurrentBibleVersionOT(string id)
        {
            CurrentBibleVersionOT = id;
        }
        
        public void SetCurrentBibleVersionNT(string id)
        {
            CurrentBibleVersionNT = id;
        }
        
    }
}
