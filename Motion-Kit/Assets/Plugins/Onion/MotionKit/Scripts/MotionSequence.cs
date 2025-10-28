using System.Collections.Generic;
using UnityEngine;

namespace Onion.MotionKit {
    [CreateAssetMenu(menuName = "Motion Kit/Sequence")]
    public class MotionSequence : ScriptableObject {
        [SerializeReference]
        public List<MotionTrack> tracks;

        
    }
}
