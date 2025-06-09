<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output omit-xml-declaration="yes" indent="yes"/>

    <!-- Identity template to preserve all existing XML -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

    <!-- 🔹 Named templates for Distinct Clan Name Groups -->

    <!-- Finnic: ancient Finnish surnames (mythic/nature/archaic) -->
    <xsl:template name="sharedFinnicClanNames">
        <name name="{{=ClanFinn001}}Kalevala"/>
        <name name="{{=ClanFinn002}}Hiidenmaa"/>
        <name name="{{=ClanFinn003}}Ilmari"/>
        <name name="{{=ClanFinn004}}Toivonen"/>
        <name name="{{=ClanFinn005}}Koskenniemi"/>
        <name name="{{=ClanFinn006}}Vettenranta"/>
        <name name="{{=ClanFinn007}}Lehtovaara"/>
        <name name="{{=ClanFinn008}}Kivinen"/>
        <name name="{{=ClanFinn009}}Aaltola"/>
        <name name="{{=ClanFinn010}}Pohjanheimo"/>
    </xsl:template>

    <!-- Karelian: mix of Finnish, archaic, and Slavic-touched names -->
    <xsl:template name="sharedKarelianClanNames">
        <name name="{{=ClanKare001}}Rautio"/>
        <name name="{{=ClanKare002}}Niemelä"/>
        <name name="{{=ClanKare003}}Jääskeläinen"/>
        <name name="{{=ClanKare004}}Korpela"/>
        <name name="{{=ClanKare005}}Petrovich"/>
        <name name="{{=ClanKare006}}Vuorinen"/>
        <name name="{{=ClanKare007}}Ahomäki"/>
        <name name="{{=ClanKare008}}Lohikoski"/>
        <name name="{{=ClanKare009}}Sorjonen"/>
        <name name="{{=ClanKare010}}Terentjev"/>
    </xsl:template>

    <!-- Savonian: earthy and rural -->
    <xsl:template name="sharedSavonianClanNames">
        <name name="{{=ClanSavo001}}Pöllänen"/>
        <name name="{{=ClanSavo002}}Huttunen"/>
        <name name="{{=ClanSavo003}}Räsänen"/>
        <name name="{{=ClanSavo004}}Tolonen"/>
        <name name="{{=ClanSavo005}}Tanskanen"/>
        <name name="{{=ClanSavo006}}Nissinen"/>
        <name name="{{=ClanSavo007}}Kettunen"/>
        <name name="{{=ClanSavo008}}Partanen"/>
        <name name="{{=ClanSavo009}}Väänänen"/>
        <name name="{{=ClanSavo010}}Moilanen"/>
    </xsl:template>

    <!-- Ingrian: Nordic + Russian borrowings -->
    <xsl:template name="sharedIngrianClanNames">
        <name name="{{=ClanIngr001}}Järvinen"/>
        <name name="{{=ClanIngr002}}Leppänen"/>
        <name name="{{=ClanIngr003}}Makarov"/>
        <name name="{{=ClanIngr004}}Klementjev"/>
        <name name="{{=ClanIngr005}}Haapakoski"/>
        <name name="{{=ClanIngr006}}Mustonen"/>
        <name name="{{=ClanIngr007}}Ollikainen"/>
        <name name="{{=ClanIngr008}}Andreev"/>
        <name name="{{=ClanIngr009}}Kurki"/>
        <name name="{{=ClanIngr010}}Suomalainen"/>
    </xsl:template>

    <!-- Lappish: Finnish–Sami borderlands -->
    <xsl:template name="sharedLappishClanNames">
        <name name="{{=ClanLapp001}}Palojärvi"/>
        <name name="{{=ClanLapp002}}Vuotso"/>
        <name name="{{=ClanLapp003}}Rantanen"/>
        <name name="{{=ClanLapp004}}Kalliojärvi"/>
        <name name="{{=ClanLapp005}}Pekkarinen"/>
        <name name="{{=ClanLapp006}}Metsäranta"/>
        <name name="{{=ClanLapp007}}Luosma"/>
        <name name="{{=ClanLapp008}}Tunturila"/>
        <name name="{{=ClanLapp009}}Aikioinen"/>
        <name name="{{=ClanLapp010}}Saarela"/>
    </xsl:template>

    <!-- Sami: real historic surnames, some with minor accent removed -->
    <xsl:template name="sharedSamiClanNames">
        <name name="{{=ClanSami001}}Utsi"/>
        <name name="{{=ClanSami002}}Valkeapää"/>
        <name name="{{=ClanSami003}}Gaup"/>
        <name name="{{=ClanSami004}}Bals"/>
        <name name="{{=ClanSami005}}Labba"/>
        <name name="{{=ClanSami006}}Hætta"/>
        <name name="{{=ClanSami007}}Nutti"/>
        <name name="{{=ClanSami008}}Jomppanen"/>
        <name name="{{=ClanSami009}}Magga"/>
        <name name="{{=ClanSami010}}Pieski"/>
    </xsl:template>

    <!-- Estonian: older surnames with Baltic and Germanic roots -->
    <xsl:template name="sharedEstonianClanNames">
        <name name="{{=ClanEst001}}Tamm"/>
        <name name="{{=ClanEst002}}Saar"/>
        <name name="{{=ClanEst003}}Kask"/>
        <name name="{{=ClanEst004}}Rebane"/>
        <name name="{{=ClanEst005}}Pärn"/>
        <name name="{{=ClanEst006}}Lill"/>
        <name name="{{=ClanEst007}}Koppel"/>
        <name name="{{=ClanEst008}}Aun"/>
        <name name="{{=ClanEst009}}Mets"/>
        <name name="{{=ClanEst010}}Vaher"/>
    </xsl:template>

    <!-- Ostrobothnian: Finnish + coastal Swedish -->
    <xsl:template name="sharedOstrobothnianClanNames">
        <name name="{{=ClanOstro001}}Isokangas"/>
        <name name="{{=ClanOstro002}}Laajalahti"/>
        <name name="{{=ClanOstro003}}Rintala"/>
        <name name="{{=ClanOstro004}}Vaajakoski"/>
        <name name="{{=ClanOstro005}}Sundström"/>
        <name name="{{=ClanOstro006}}Strandberg"/>
        <name name="{{=ClanOstro007}}Järvenpää"/>
        <name name="{{=ClanOstro008}}Niemitalo"/>
        <name name="{{=ClanOstro009}}Peltokangas"/>
        <name name="{{=ClanOstro010}}Laihonen"/>
    </xsl:template>

    <!-- Häme (Tavastia): traditional Finnish -->
    <xsl:template name="sharedHameClanNames">
        <name name="{{=ClanHame001}}Somerjoki"/>
        <name name="{{=ClanHame002}}Härkälä"/>
        <name name="{{=ClanHame003}}Tuomola"/>
        <name name="{{=ClanHame004}}Koskelin"/>
        <name name="{{=ClanHame005}}Hakola"/>
        <name name="{{=ClanHame006}}Ilvesmäki"/>
        <name name="{{=ClanHame007}}Vehkamäki"/>
        <name name="{{=ClanHame008}}Tavastila"/>
        <name name="{{=ClanHame009}}Heinonen"/>
        <name name="{{=ClanHame010}}Rautamäki"/>
    </xsl:template>

    <!-- Kven: Arctic Finnic with Savonian & Lappish influence -->
    <xsl:template name="sharedKvenClanNames">
        <name name="{{=ClanKven001}}Kivikangas"/>
        <name name="{{=ClanKven002}}Orajärvi"/>
        <name name="{{=ClanKven003}}Halvari"/>
        <name name="{{=ClanKven004}}Kaikkonen"/>
        <name name="{{=ClanKven005}}Vuolajärvi"/>
        <name name="{{=ClanKven006}}Saari"/>
        <name name="{{=ClanKven007}}Ketola"/>
        <name name="{{=ClanKven008}}Kurkela"/>
        <name name="{{=ClanKven009}}Rautio"/>
        <name name="{{=ClanKven010}}Ylitalo"/>
    </xsl:template>


    <!-- 🔸 Clan Name Injection by Culture (calling named templates) -->

    <!-- Finnish: mixed with Savonian, Lappish, Häme, Ostrobothnian -->
    <xsl:template match="Culture[@id='finnish']/clan_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedFinnicClanNames"/>
            <xsl:call-template name="sharedSavonianClanNames"/>
            <xsl:call-template name="sharedLappishClanNames"/>
            <xsl:call-template name="sharedHameClanNames"/>
            <xsl:call-template name="sharedOstrobothnianClanNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Karelian: mixed with Savonian and Ingrian -->
    <xsl:template match="Culture[@id='karelian']/clan_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedKarelianClanNames"/>
            <xsl:call-template name="sharedSavonianClanNames"/>
            <xsl:call-template name="sharedIngrianClanNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Aestian: Estonian, Ingrian, Karelian, Ostrobothnian -->
    <xsl:template match="Culture[@id='aestian']/clan_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedEstonianClanNames"/>
            <xsl:call-template name="sharedIngrianClanNames"/>
            <xsl:call-template name="sharedKarelianClanNames"/>
            <xsl:call-template name="sharedOstrobothnianClanNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Sami: pure Lappish + Sami -->
    <xsl:template match="Culture[@id='sami']/clan_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedSamiClanNames"/>
            <xsl:call-template name="sharedLappishClanNames"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
