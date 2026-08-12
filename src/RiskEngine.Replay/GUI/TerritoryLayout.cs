using System.Collections.Generic;

namespace RiskEngine.Replay.GUI;


public static class TerritoryLayout
{
    public static readonly TerritoryLayoutEntry[] Entries =
    [
        // Nordamerika
        new("Alaska", 0.065f, 0.1589f),
        new("Nordwest-Territorium",0.144F,0.148F),
        new("Alberta",0.158F,0.239F),
        new("Ontario",0.223F,0.256F),
        new("Ostkanada",0.298F,0.262F),
        new("Weststaaten",0.16F,0.354F),
        new("Oststaaten",0.244F,0.376F),
        new("Grönland",0.357F,0.098F),
        new("Mittelamerika",0.168F,0.499F),
        
        //Südamerika
        new("Venezuela",0.244F,0.581F),
        new("Peru",0.211F,0.693F),
        new("Brasilien",0.327F,0.68F),
        new("Argentinien",0.268F,0.809F),
        
        //Afrika
        new("Nordafrika",0.463F,0.631F),
        new("Ägypten",0.54F,0.596F),
        new("Zentralafrika",0.541F,0.747F),
        new("Ostafrika",0.618F,0.721F),
        new("Madagaskar",0.646F,0.889F),
        new("Südafrika",0.554F,0.867F),
        
        //Europa
        new("Westeuropa",0.435F,0.481F),
        new("Südeuropa",0.516F,0.448F),
        new("Nordeuropa",0.513F,0.357F),
        new("Skandinavien",0.543F,0.176F),
        new("Großbritannine",0.423F,0.351F),
        new("Island",0.44F,0.208F),
        new("Russland",0.616F,0.262F),
        
        //Asien
        new("Ural",0.706F,0.264F),
        new("Sibirien",0.759F,0.167F),
        new("Jakutien",0.838F,0.124F),
        new("Kamtschatka",0.922F,0.137F),
        new("Irkutsk",0.828F,0.26F),
        new("Mongolei",0.838F,0.364F),
        new("China",0.813F,0.468F),
        new("Südostasien",0.823F,0.592F),
        new("Indien",0.738F,0.526F),
        new("Afghanistan",0.69F,0.398F),
        new("Naher Osten",0.623F,0.504F),
        new("Japan",0.945F,0.38F),
        
        //Australien
        new("Indonesien",0.828F,0.752F),
        new("Neu Guinea",0.924F,0.699F),
        new("Westaustralien",0.862F,0.887F),
        new("Ostaustralien",0.927F,0.833F)
        
    ];
}
        
        
        