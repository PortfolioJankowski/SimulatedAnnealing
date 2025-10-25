using System.ComponentModel;

namespace SimulatedAnnealing.Server.Models.Enums
{
    public enum PoliticalCommittee
    {
        [Description("KOALICYJNY KOMITET WYBORCZY KOALICJA OBYWATELSKA PO .N IPL ZIELONI")]
        CivicPlatform = 2023_01,

        [Description("KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ")]
        LawAndJustice = 2023_02,

        [Description("KOALICYJNY KOMITET WYBORCZY KOALICJA OBYWATELSKA PO .N IPL ZIELONI - ZPOW-601-6/19")]
        CivicPlatform2019 = 2019_01,

        [Description("KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ - ZPOW-601-9/19")]
        LawAndJustice2019 = 2019_02
    }
}
