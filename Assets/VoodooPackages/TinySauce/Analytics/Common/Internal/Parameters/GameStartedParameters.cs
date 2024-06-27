using System;
using System.Collections.Generic;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    [Serializable]
    public class GameStartedParameters
    {
        /// <summary>
        /// The level identifier, ideally a number.
        /// </summary>
        public string levelDimension1 = "";

        public string levelDimension2 = null;
        
        public string levelDimension3 = null;
        
        /// <summary>
        /// Ordinal position in the progression. Example: 1 for the first level.
        /// </summary>
        public int ordinal = 0;
        
        /// <summary>
        /// Loop number of levels are repeated when players run out of content.
        /// </summary>
        public int loop = 1;
        
        /// <summary>
        /// The level definition ID.
        /// </summary>
        public string levelDefinitionID = null;
        
        /// <summary>
        /// Name of the progression in case there are levels in separate progressions. Must be a game-specific enum.
        /// </summary>
        public Enum progression = null;
        
        /// <summary>
        /// How many moves does the level have by default.
        /// </summary>
        public int levelMoves = 0;
        
        /// <summary>
        /// Number of additional moved granted, e.g. by an additional booster.
        /// </summary>
        public int additionalMovesGranted = 0;
        
        /// <summary>
        /// Total number of objectives to win the level.
        /// </summary>
        public float? numberOfObjectives = null;
        
        /// <summary>
        /// Game mode of the level. Must be a game-specific enum.
        /// </summary>
        public Enum gameMode = null;
        
        /// <summary>
        /// Random seed used to generate the level. Can be NULL if not relevant.
        /// </summary>
        public string seed = null;
        
        /// <summary>
        /// Additional custom parameters, if a key does not exist in eventContextProperties merge into it
        /// (Deprecated, use eventContextProperties instead)
        /// </summary>
        public Dictionary<string, object> eventProperties = new Dictionary<string, object>();
        
        /// <summary>
        /// Context parameters (different from `eventProperties`).
        /// </summary>
        public Dictionary<string, object> eventContextProperties = new Dictionary<string, object>();
    }
}
