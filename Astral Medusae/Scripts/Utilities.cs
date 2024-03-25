using System;
using XRL.Rules;

namespace SnakefangoxAstralMedusae
{
    public class Utilities
    {
        public static Random GameRandom() {
            return Stat.GetSeededRandomGenerator("Snakefangox_AstralMedusae");
        }
    }
}