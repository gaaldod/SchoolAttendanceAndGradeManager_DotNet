using System;
using System.Collections.Generic;
using System.Linq;
using ULogic.Models;

/*
 * ============================================================================
 * 
 * 1. Helyezzétek el ezt a fájlt a projektben (érdemes '.cs' kiterjesztéssel elmenteni, pl. HianyzasKiertekelo.cs).
 * 2. Az adatbázis lekérdezésnél feltétlenül "Include"-oljátok be a Jelenletek táblát is, 
 *    mivel az algoritmus ezekből számol!
 *    Példa Entity Framework Core-ral:
 *       var diakok = context.Hallgatok.Include(h => h.Jelenletek).ToList();
 * 
 * 3. Hívjátok meg az algoritmust, és adjátok át neki a betöltött listát, illetve a figyelmeztetési küszöböt (pl. 25%):
 *    Példa:
 *       var eredmeny = HianyzasKiertekelo.Kiertekeles(diakok, 25.0);
 * 
 * Az eredmény egy olyan lista lesz, ami már tartalmazza a kiszámolt százalékot,
 * a figyelmeztetést (ha átlépte a küszöböt), és a legproblémásabbakkal kezdődik a lista.
 * ============================================================================
 */

namespace HianyzasKiertekelo
{
    /// <summary>
    /// Segédosztály, ami tartalmazza a diákot és a kiszámolt hiányzási statisztikáit.
    
    public class HianyzasEredmeny
    {
        public Hallgato Diak { get; set; }
        public int OsszesOra { get; set; }
        public int HianyzasokSzama { get; set; }
        public double HianyzasArany { get; set; } // Százalékos érték (pl. 25.5)
        public bool Figyelmeztetes { get; set; } // True, ha átlépte a küszöböt
    }

    public class HianyzasKiertekelo 
    {
        /// <summary>
        /// Kiszámolja a hiányzási arányt, figyelmeztetést generál a küszöb alapján, 
        /// és a legproblémásabb diákok szerint rendezi a listát.
        /// </summary>
        /// <param name="hallgatok">A vizsgált diákok listája</param>
        /// <param name="kuszobSzazalek">A figyelmeztetési küszöb százalékban (pl. 20.0)</param>
        /// <returns>A statisztikákkal és figyelmeztetésekkel kiegészített, rendezett lista</returns>
        public static List<HianyzasEredmeny> Kiertekeles(IEnumerable<Hallgato> hallgatok, double kuszobSzazalek = 20.0)
        {
            if (hallgatok == null || !hallgatok.Any())
            {
                return new List<HianyzasEredmeny>();
            }

            var eredmenyek = new List<HianyzasEredmeny>();

            foreach (var h in hallgatok)
            {
                int osszesOra = h.Jelenletek?.Count ?? 0;
                // Csak az "Absent" státuszokat számoljuk
                int hianyzasokSzama = h.Jelenletek?.Count(j => j.Statusz.Equals("Absent", StringComparison.OrdinalIgnoreCase)) ?? 0;
                
                // Arány kiszámítása: (hiányzások / összes) * 100
                double arany = osszesOra > 0 ? ((double)hianyzasokSzama / osszesOra) * 100.0 : 0.0;
                
                eredmenyek.Add(new HianyzasEredmeny
                {
                    Diak = h,
                    OsszesOra = osszesOra,
                    HianyzasokSzama = hianyzasokSzama,
                    HianyzasArany = arany,
                    Figyelmeztetes = arany >= kuszobSzazalek
                });
            }

            // Rendezés: 
            // 1. Akik elérték a küszöböt (Figyelmeztetes == true kerül előre)
            // 2. Azokon belül a hiányzási arány alapján csökkenő sorrendbe (a legnagyobb arányú van legelöl)
            return eredmenyek
                .OrderByDescending(e => e.Figyelmeztetes)
                .ThenByDescending(e => e.HianyzasArany)
                .ToList();
        }
    }
}
