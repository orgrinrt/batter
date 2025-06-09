<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output omit-xml-declaration="yes" indent="yes"/>

    <!-- Identity template to copy everything else unchanged -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

    <!-- 🔹 Named templates for Shared Female Name Groups -->

    <!-- Finnic: mythic and nature inspired -->
    <xsl:template name="sharedFinnicFemaleNames">
        <name name="{{=FinnF001}}Tellervo"/>
        <name name="{{=FinnF002}}Louhi"/>
        <name name="{{=FinnF003}}Kielo"/>
        <name name="{{=FinnF004}}Aamu"/>
        <name name="{{=FinnF005}}Ilta"/>
        <name name="{{=FinnF006}}Toive"/>
        <name name="{{=FinnF007}}Lempi"/>
        <name name="{{=FinnF008}}Mielikki"/>
        <name name="{{=FinnF009}}Kaste"/>
        <name name="{{=FinnF010}}Säde"/>
    </xsl:template>

    <!-- Karelian: traditional + Russian-influenced -->
    <xsl:template name="sharedKarelianFemaleNames">
        <name name="{{=KareF001}}Vieno"/>
        <name name="{{=KareF002}}Hellä"/>
        <name name="{{=KareF003}}Ilmoja"/>
        <name name="{{=KareF004}}Marjatta"/>
        <name name="{{=KareF005}}Aino"/>
        <name name="{{=KareF006}}Ljudmila"/>
        <name name="{{=KareF007}}Raisa"/>
        <name name="{{=KareF008}}Irina"/>
        <name name="{{=KareF009}}Tatjana"/>
        <name name="{{=KareF010}}Anja"/>
    </xsl:template>

    <!-- Savonian: rustic, earthy -->
    <xsl:template name="sharedSavonianFemaleNames">
        <name name="{{=SavoF001}}Talvi"/>
        <name name="{{=SavoF002}}Sini"/>
        <name name="{{=SavoF003}}Nuppu"/>
        <name name="{{=SavoF004}}Usva"/>
        <name name="{{=SavoF005}}Valkea"/>
    </xsl:template>

    <!-- Ingrian: Finnish base + minor Russian loans -->
    <xsl:template name="sharedIngrianFemaleNames">
        <name name="{{=IngrF001}}Suvetar"/>
        <name name="{{=IngrF002}}Helmi"/>
        <name name="{{=IngrF003}}Kuura"/>
        <name name="{{=IngrF004}}Aava"/>
        <name name="{{=IngrF005}}Rauha"/>
        <name name="{{=IngrF006}}Olga"/>
        <name name="{{=IngrF007}}Nina"/>
        <name name="{{=IngrF008}}Vera"/>
        <name name="{{=IngrF009}}Tanja"/>
        <name name="{{=IngrF010}}Lena"/>
    </xsl:template>

    <!-- Lappish: Sami-like names without accents -->
    <xsl:template name="sharedLappishFemaleNames">
        <name name="{{=LappF001}}Elle"/>
        <name name="{{=LappF002}}Sunna"/>
        <name name="{{=LappF003}}Máret"/>
        <name name="{{=LappF004}}Biret"/>
        <name name="{{=LappF005}}Ánne"/>
    </xsl:template>

    <!-- Sami: authentic accents retained -->
    <xsl:template name="sharedSamiFemaleNames">
        <name name="{{=SamiF001}}Risten"/>
        <name name="{{=SamiF002}}Sire"/>
        <name name="{{=SamiF003}}Láilá"/>
        <name name="{{=SamiF004}}Heaika"/>
        <name name="{{=SamiF005}}Njálla"/>
    </xsl:template>

    <!-- Ostrobothnian: Härmä Finnish + coastal Swedish -->
    <xsl:template name="sharedOstrobothnianFemaleNames">
        <name name="{{=OstroF001}}Kaija"/>
        <name name="{{=OstroF002}}Eerika"/>
        <name name="{{=OstroF003}}Aila"/>
        <name name="{{=OstroF004}}Kyllikki"/>
        <name name="{{=OstroF005}}Pihla"/>
        <name name="{{=OstroF006}}Tuulikki"/>
        <name name="{{=OstroF007}}Raita"/>
        <name name="{{=OstroF008}}Veera"/>
        <name name="{{=OstroF009}}Sanni"/>
        <name name="{{=OstroF010}}Hilja"/>
    </xsl:template>

    <!-- Häme/Tavastia: traditional Finnish -->
    <xsl:template name="sharedHameFemaleNames">
        <name name="{{=HameF001}}Liisa"/>
        <name name="{{=HameF002}}Kaarina"/>
        <name name="{{=HameF003}}Marja"/>
        <name name="{{=HameF004}}Annikki"/>
        <name name="{{=HameF005}}Saara"/>
        <name name="{{=HameF006}}Elina"/>
        <name name="{{=HameF007}}Hilma"/>
        <name name="{{=HameF008}}Aino"/>
        <name name="{{=HameF009}}Sisko"/>
        <name name="{{=HameF010}}Leena"/>
    </xsl:template>

    <!-- Estonian: melodic, Baltic Finnic roots -->
    <xsl:template name="sharedEstonianFemaleNames">
        <name name="{{=EstF001}}Maarja"/>
        <name name="{{=EstF002}}Leelo"/>
        <name name="{{=EstF003}}Kaisa"/>
        <name name="{{=EstF004}}Eha"/>
        <name name="{{=EstF005}}Tuuli"/>
        <name name="{{=EstF006}}Liina"/>
        <name name="{{=EstF007}}Sirje"/>
        <name name="{{=EstF008}}Helmi"/>
        <name name="{{=EstF009}}Pille"/>
        <name name="{{=EstF010}}Raili"/>
    </xsl:template>

    <!-- Kvenn (Northern Norway): blend of Finnish + Sami -->
    <xsl:template name="sharedKvennFemaleNames">
        <name name="{{=KvenF001}}Sanni"/>
        <name name="{{=KvenF002}}Inga"/>
        <name name="{{=KvenF003}}Eira"/>
        <name name="{{=KvenF004}}Kaisa"/>
        <name name="{{=KvenF005}}Aila"/>
        <name name="{{=KvenF006}}Annika"/>
        <name name="{{=KvenF007}}Mette"/>
        <name name="{{=KvenF008}}Marit"/>
        <name name="{{=KvenF009}}Raisa"/>
        <name name="{{=KvenF010}}Tuija"/>
    </xsl:template>


    <!-- 🔸 Culture-Specific Female Name Injection -->

    <!-- Finnish culture: combine Finnic + Sami + Lappish + Savonian + Ingrian + Ostrobothnian + Häme -->
    <xsl:template match="Culture[@id='finnish']/female_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedFinnicFemaleNames"/>
            <xsl:call-template name="sharedSamiFemaleNames"/>
            <xsl:call-template name="sharedLappishFemaleNames"/>
            <xsl:call-template name="sharedSavonianFemaleNames"/>
            <xsl:call-template name="sharedIngrianFemaleNames"/>
            <xsl:call-template name="sharedOstrobothnianFemaleNames"/>
            <xsl:call-template name="sharedHameFemaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Karelian culture: Karelian + Savonian + Ostrobothnian -->
    <xsl:template match="Culture[@id='karelian']/female_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedKarelianFemaleNames"/>
            <xsl:call-template name="sharedSavonianFemaleNames"/>
            <xsl:call-template name="sharedOstrobothnianFemaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Aestian culture: Finnic + Ingrian + Karelian + Estonian -->
    <xsl:template match="Culture[@id='aestian']/female_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedFinnicFemaleNames"/>
            <xsl:call-template name="sharedIngrianFemaleNames"/>
            <xsl:call-template name="sharedKarelianFemaleNames"/>
            <xsl:call-template name="sharedEstonianFemaleNames"/>
        </xsl:copy>
    </xsl:template>

    <!-- Sami culture: Sami + Lappish + Kvenn -->
    <xsl:template match="Culture[@id='sami']/female_names">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
            <xsl:call-template name="sharedSamiFemaleNames"/>
            <xsl:call-template name="sharedLappishFemaleNames"/>
            <xsl:call-template name="sharedKvennFemaleNames"/>
        </xsl:copy>
    </xsl:template>


</xsl:stylesheet>
