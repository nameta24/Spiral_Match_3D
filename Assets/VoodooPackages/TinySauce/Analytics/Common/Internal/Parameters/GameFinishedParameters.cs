using System;
using System.Collections.Generic;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    [Serializable]
    public class GameFinishedParameters
    {
        /// <summary>
        /// Do not modify this parameter. It is updated automatically by the VoodooSauce.
        /// </summary>
        public int gameDuration = 0;
        
        /// <summary>
        /// The level identifier, ideally a number.
        /// </summary>
        public string levelDimension1 = "";

        public string levelDimension2 = null;
        
        public string levelDimension3 = null;

        /// <summary>
        /// Outcome of the game, True if won, else False.
        /// </summary>
        public bool status = false;
        
        /// <summary>
        /// Score obtained for this attempts.
        /// </summary>
        public float score = 0;
        
        /// <summary>
        /// The level definition ID.
        /// </summary>
        public string levelDefinitionID = "level";
        
        /// <summary>
        /// If level is lost, reason it was lost. Game-specific enum, default is ‘other’.
        /// </summary>
        public Enum gameEndReason = null;
        
        /// <summary>
        /// Number of stars gained from a level. Can be NULL if not relevant.
        /// </summary>
        public int? nbStars = null;
        
        /// <summary>
        /// How many moves were used during the level. Can be NULL if not relevant.
        /// </summary>
        public int? movesUsed = null;
        
        /// <summary>
        /// How many moves were left when the level ended. Can be NULL if not relevant.
        /// </summary>
        public int? movesLeft = null;
        
        /// <summary>
        /// Number of objectives left when the level ended. Can be NULL if not relevant.
        /// </summary>
        public int? objectivesLeft = null;
        
        /// <summary>
        /// Amount of soft currency used in this attempt.
        /// </summary>
        public float softCurrencyUsed = 0;
        
        /// <summary>
        /// Amount of hard currency used in this attempt.
        /// </summary>
        public float hardCurrencyUsed = 0;
        
        /// <summary>
        /// Number of End of Game Purchase used on this attempt (all sources).
        /// </summary>
        public int egpsUsed = 0;
        
        /// <summary>
        /// Number of End of Game Purchase used on this attempt (RV only).
        /// </summary>
        public int egpsRvUsed = 0;
        
        /// <summary>
        /// How many moves does the level have by default (might be different from the number of moves the player ends up having)
        /// </summary>
        public int? levelMoves = null;
        
        /// <summary>
        /// Number of additional moves granted at the start of the level, e.g. by an additional booster.
        /// </summary>
        public int? additionalMovesGranted = null;
        
        /// <summary>
        /// Number of additional moves that player got from EGPs.
        /// </summary>
        public int? egpMoves = null;
        
        /// <summary>
        /// Number of in-game boosters used during the attempt.
        /// </summary>
        public int? nbInGameBoostersUsed = null;
        
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
