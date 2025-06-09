<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                version="1.0">
    <xsl:output omit-xml-declaration="yes" indent="yes"/>

    <!-- Identity template to copy everything else unchanged -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

    <!-- 🔹 Distinct Male Name Groups with 10 names each, defined as named templates -->

    <!-- Finnic: mythic and nature inspired, no repeats -->
    <xsl:template name="sharedFinnicMaleNames">
        <name name="{{=Finn001}}Kaleva"/>
        <name name="{{=Finn002}}Ahti"/>
        <name name="{{=Finn003}}Sampo"/>
        <name name="{{=Finn004}}Ilmarinen"/>
        <name name="{{=Finn005}}Toivo"/>
        <name name="{{=Finn006}}Otso"/>
        <name name="{{=Finn007}}Väinär"/> <!-- variation from Väinö -->
        <name name="{{=Finn008}}Tapio"/>
        <name name="{{=Finn009}}Eerik"/> <!-- variation of Eero -->
        <name name="{{=Finn010}}Lauri"/>
    </xsl:template>

    <!-- Karelian: mostly traditional, plus small Russian-influenced flavor -->
    <xsl:template name="sharedKarelianMaleNames">
        <name name="{{=Kare001}}Arvo"/>
        <name name="{{=Kare002}}Eelis"/>
        <name name="{{=Kare003}}Ilvo"/>
        <name name="{{=Kare004}}Laurin"/> <!-- variation of Lauri -->
        <name name="{{=Kare005}}Simo"/>
        <name name="{{=Kare006}}Mikael"/>
        <name name="{{=Kare007}}Veniamin"/> <!-- Russian loan -->
        <name name="{{=Kare008}}Niko"/>
        <name name="{{=Kare009}}Risto"/>
        <name name="{{=Kare010}}Antero"/>
    </xsl:template>

    <!-- Savonian: rustic, earthy -->
    <xsl:template name="sharedSavonianMaleNames">
        <name name="{{=Savo001}}Veikko"/>
        <name name="{{=Savo002}}Onni"/>
        <name name="{{=Savo003}}Armas"/>
        <name name="{{=Savo004}}Eino"/>
        <name name="{{=Savo005}}Kalevin"/> <!-- variation of Kalevi -->
        <name name="{{=Savo006}}Aapo"/>
        <name name="{{=Savo007}}Eerik"/>
        <name name="{{=Savo008}}Tapani"/>
        <name name="{{=Savo009}}Lassi"/>
        <name name="{{=Savo010}}Pekka"/>
    </xsl:template>

    <!-- Ingrian: Nordic base with minor Russian loan names -->
    <xsl:template name="sharedIngrianMaleNames">
        <name name="{{=Ingr001}}Juhani"/>
        <name name="{{=Ingr002}}Erik"/>
        <name name="{{=Ingr003}}Olof"/>
        <name name="{{=Ingr004}}Niilo"/>
        <name name="{{=Ingr005}}Sakari"/>
        <name name="{{=Ingr006}}Mikhail"/> <!-- Russian loan -->
        <name name="{{=Ingr007}}Andrei"/>  <!-- Russian loan -->
        <name name="{{=Ingr008}}Jari"/>
        <name name="{{=Ingr009}}Aleksis"/>
        <name name="{{=Ingr010}}Petteri"/>
    </xsl:template>

    <!-- Lappish: Sami-like names but without accents -->
    <xsl:template name="sharedLappishMaleNames">
        <name name="{{=Lapp001}}Aslak"/>
        <name name="{{=Lapp002}}Biera"/>
        <name name="{{=Lapp003}}Ivar"/> <!-- no accent -->
        <name name="{{=Lapp004}}Ande"/>
        <name name="{{=Lapp005}}Njuolla"/>
        <name name="{{=Lapp006}}Kari"/>
        <name name="{{=Lapp007}}Olli"/>
        <name name="{{=Lapp008}}Seppo"/>
        <name name="{{=Lapp009}}Tauno"/>
        <name name="{{=Lapp010}}Viljo"/>
    </xsl:template>

    <!-- Sami: keep accents for authenticity -->
    <xsl:template name="sharedSamiMaleNames">
        <name name="{{=Sami001}}Mikkel"/>
        <name name="{{=Sami002}}Jussa"/>
        <name name="{{=Sami003}}Vuolabbán"/>
        <name name="{{=Sami004}}Beaska"/>
        <name name="{{=Sami005}}Nilla"/>
        <name name="{{=Sami006}}Áile"/>
        <name name="{{=Sami007}}Guttorm"/>
        <name name="{{=Sami008}}Niillas"/>
        <name name="{{=Sami009}}Lemet"/>
        <name name="{{=Sami010}}Máze"/>
    </xsl:template>

    <!-- Ostrobothnian: mix of Härmä Finnish and coastal Swedish flavor -->
    <xsl:template name="sharedOstrobothnianMaleNames">
        <name name="{{=Ostro001}}Kustaa"/>      <!-- Finnish Härmä -->
        <name name="{{=Ostro002}}Antti"/>       <!-- Finnish Härmä -->
        <name name="{{=Ostro003}}Jaakko"/>      <!-- Finnish Härmä -->
        <name name="{{=Ostro004}}Seppo"/>       <!-- Finnish Härmä -->
        <name name="{{=Ostro005}}Eero"/>        <!-- Finnish Härmä -->
        <name name="{{=Ostro006}}Axel"/>        <!-- Coastal Swedish -->
        <name name="{{=Ostro007}}Bengt"/>       <!-- Coastal Swedish -->
        <name name="{{=Ostro008}}Lars"/>        <!-- Coastal Swedish -->
        <name name="{{=Ostro009}}Mikael"/>      <!-- Coastal Swedish -->
        <name name="{{=Ostro010}}Sven"/>        <!-- Coastal Swedish -->
    </xsl:template>

    <!-- Häme (Tavastia): mostly Finnish-sounding but noble/old style, less Swedish -->
    <xsl:template name="sharedHameMaleNames">
        <name name="{{=Hame001}}Arvi"/>
        <name name="{{=Hame002}}Eero"/>
        <name name="{{=Hame003}}Juhani"/>
        <name name="{{=Hame004}}Lassi"/>
        <name name="{{=Hame005}}Paavo"/>
        <name name="{{=Hame006}}Raimo"/>
        <name name="{{=Hame007}}Kalevi"/>
        <name name="{{=Hame008}}Simo"/>
        <name name="{{=Hame009}}Ilmari"/>
        <name name="{{=Hame010}}Onni"/>
    </xsl:template>

    <!-- Estonian: mix of native and Baltic German -->
    <xsl:template name="sharedEstonianMaleNames">
        <name name="{{=Est001}}Jaan"/>
        <name name="{{=Est002}}Peeter"/>
        <name name="{{=Est003}}Mati"/>
        <name name="{{=Est004}}Tõnis"/>
        <name name="{{=Est005}}Uku"/>
        <name name="{{=Est006}}Karl"/>
        <name name="{{=Est007}}Juhan"/>
        <name name="{{=Est008}}Vello"/>
        <name name="{{=Est009}}Mikk"/>
        <name name="{{=Est010}}Aivar"/>
    </xsl:template>

    <!-- Kven: Norwegian style with Finnish flavor, 10 names -->
    <xsl:template name="sharedKvenMaleNames">
        <name name="{{=Kven001}}Haraldi"/>    <!-- from Harald -->
        <name name="{{=Kven002}}Eelisi"/>     <!-- variation from Eelis -->
        <name name="{{=Kven003}}Vaeno"/>      <!-- variation Väinö -->
        <name name="{{=Kven004}}Raaknar"/>    <!-- from Ragnar -->
        <name name="{{=Kven005}}Sakari"/>
        <name name="{{=Kven006}}Olli"/>
        <name name="{{=Kven007}}Kaarlo"/>
        <name name="{{=Kven008}}Tomi"/>
        <name name="{{=Kven009}}Jori"/>
        <name name="{{=Kven010}}Veikko"/>
    </xsl:template>


    <!-- 🔸 Culture-Specific Injections using named templates -->

    <!-- Finnish culture: add Finnic + Ostrobothnian + Häme + Lappish + Savonian -->
    <xsl:template match="Culture[@id='finnish']/male_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedFinnicMaleNames"/>
            <xsl:call-template name="sharedOstrobothnianMaleNames"/>
            <xsl:call-template name="sharedHameMaleNames"/>
            <xsl:call-template name="sharedLappishMaleNames"/>
            <xsl:call-template name="sharedSavonianMaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Karelian culture: Karelian + Savonian + Ingrian -->
    <xsl:template match="Culture[@id='karelian']/male_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedKarelianMaleNames"/>
            <xsl:call-template name="sharedSavonianMaleNames"/>
            <xsl:call-template name="sharedIngrianMaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Aestian culture: Finnic + Ingrian + Karelian + Estonian + Kven -->
    <xsl:template match="Culture[@id='aestian']/male_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedFinnicMaleNames"/>
            <xsl:call-template name="sharedIngrianMaleNames"/>
            <xsl:call-template name="sharedKarelianMaleNames"/>
            <xsl:call-template name="sharedEstonianMaleNames"/>
            <xsl:call-template name="sharedKvenMaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Sami culture: Sami + Lappish -->
    <xsl:template match="Culture[@id='sami']/male_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedSamiMaleNames"/>
            <xsl:call-template name="sharedLappishMaleNames"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
