<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:exsl="http://exslt.org/common"
        version="1.0"
        extension-element-prefixes="exsl"
>
    <xsl:import href="utils.xslt"/>
    <xsl:output method="xml" indent="yes"/>


    <!-- 1) Identity -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

    <!-- 2) Main loop over Items -->
    <xsl:template match="Items">
        <xsl:copy>
            <xsl:apply-templates select="@*"/>
            <xsl:for-each select="Item">
                <!-- Define friendly variables -->
                <xsl:variable name="id" select="@id"/>
                <xsl:variable name="name" select="@name"/>
                <xsl:variable name="subtype" select="@subtype"/>
                <xsl:variable name="type" select="@Type"/>
                <xsl:variable name="weight" select="number(@weight)"/>
                <xsl:variable name="difficulty" select="number(@difficulty)"/>
                <xsl:variable name="appearance" select="number(@appearance)"/>
                <xsl:variable name="culture" select="@culture"/>
                <xsl:variable name="mesh" select="@mesh"/>
                <!-- Armor stats -->
                <xsl:variable name="head_armor" select="number(ItemComponent/*/@head_armor)"/>
                <xsl:variable name="body_armor" select="number(ItemComponent/*/@body_armor)"/>
                <xsl:variable name="leg_armor" select="number(ItemComponent/*/@leg_armor)"/>
                <xsl:variable name="arm_armor" select="number(ItemComponent/*/@arm_armor)"/>
                <!-- Weapon stats -->
                <xsl:variable name="slash_damage" select="number(ItemComponent/*/@swing_damage)"/>
                <xsl:variable name="thrust_damage" select="number(ItemComponent/*/@thrust_damage)"/>
                <xsl:variable name="missile_damage" select="number(ItemComponent/*/@missile_damage)"/>
                <xsl:variable name="handling" select="number(ItemComponent/*/@speed_rating)"/>
                <!-- Shield / other -->
                <xsl:variable name="hit_points" select="number(ItemComponent/*/@hit_points)"/>
                <!-- misc -->
                <xsl:variable name="is_civilian" select="Flags/@Civilian = 'true'"/>
                <xsl:variable name="modifier_group" select="ItemComponent/*/@modifier_group"/>
                <xsl:variable name="material_type" select="ItemComponent/*/@material_type"/>
                <!-- override the default matchField to concat(name,id,mesh) -->
                <xsl:variable name="defaultMatchField"
                              select="concat(@name,' ',@id,' ',@mesh)"/>

                <xsl:copy>
                    <xsl:apply-templates select="@*|node()"/>

                    <!-- ========== your semantic rules here ========== -->

                    <!-- +5 head armor on anything named “plate” or “steel” -->
                    <xsl:call-template name="add-head-armor">
                        <xsl:with-param name="keywords" select="'plate,steel'"/>
                        <xsl:with-param name="amount" select="5"/>
                        <xsl:with-param name="input" select="$defaultMatchField"/>
                    </xsl:call-template>

                    <!-- *0.9 price on any id containing “cloth” -->
                    <xsl:call-template name="mul-price">
                        <xsl:with-param name="keywords" select="'cloth'"/>
                        <xsl:with-param name="amount" select="0.9"/>
                        <xsl:with-param name="input" select="$id"/>
                    </xsl:call-template>

                    <!-- ========== end your rules ========== -->

                </xsl:copy>
            </xsl:for-each>
        </xsl:copy>
    </xsl:template>

    <!-- ================= Semantic Helpers ================= -->

    <!-- HEAD ARMOR -->
    <xsl:template name="add-head-armor">
        <xsl:param name="keywords"/>
        <xsl:param name="amount"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Armor'"/>
            <xsl:with-param name="targetAttr" select="'head_armor'"/>
            <xsl:with-param name="mode" select="'sum'"/>
            <xsl:with-param name="value" select="$amount"/>
        </xsl:call-template>
    </xsl:template>

    <!-- BODY ARMOR -->
    <xsl:template name="add-body-armor">
        <xsl:param name="keywords"/>
        <xsl:param name="amount"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Armor'"/>
            <xsl:with-param name="targetAttr" select="'body_armor'"/>
            <xsl:with-param name="mode" select="'sum'"/>
            <xsl:with-param name="value" select="$amount"/>
        </xsl:call-template>
    </xsl:template>

    <!-- LEG ARMOR -->
    <xsl:template name="add-leg-armor">
        <xsl:param name="keywords"/>
        <xsl:param name="amount"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Armor'"/>
            <xsl:with-param name="targetAttr" select="'leg_armor'"/>
            <xsl:with-param name="mode" select="'sum'"/>
            <xsl:with-param name="value" select="$amount"/>
        </xsl:call-template>
    </xsl:template>

    <!-- ARM ARMOR -->
    <xsl:template name="add-arm-armor">
        <xsl:param name="keywords"/>
        <xsl:param name="amount"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Armor'"/>
            <xsl:with-param name="targetAttr" select="'arm_armor'"/>
            <xsl:with-param name="mode" select="'sum'"/>
            <xsl:with-param name="value" select="$amount"/>
        </xsl:call-template>
    </xsl:template>

    <!-- SWING DAMAGE -->
    <xsl:template name="mul-swing-damage">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Weapon'"/>
            <xsl:with-param name="targetAttr" select="'swing_damage'"/>
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- THRUST DAMAGE -->
    <xsl:template name="mul-thrust-damage">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Weapon'"/>
            <xsl:with-param name="targetAttr" select="'thrust_damage'"/>
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- MISSILE DAMAGE -->
    <xsl:template name="mul-missile-damage">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Weapon'"/>
            <xsl:with-param name="targetAttr" select="'missile_damage'"/>
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- HANDLING -->
    <xsl:template name="mul-handling">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Weapon'"/>
            <xsl:with-param name="targetAttr" select="'speed_rating'"/>
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- WEIGHT -->
    <xsl:template name="mul-weight">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'.'"/>       <!-- element text -->
            <xsl:with-param name="targetAttr" select="'@weight'"/> <!-- attribute weight -->
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- PRICE -->
    <xsl:template name="mul-price">
        <xsl:param name="keywords"/>
        <xsl:param name="factor"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'Price'"/>
            <xsl:with-param name="targetAttr" select="'.'"/>
            <xsl:with-param name="mode" select="'product'"/>
            <xsl:with-param name="value" select="$factor"/>
        </xsl:call-template>
    </xsl:template>

    <!-- TIER (difficulty) -->
    <xsl:template name="add-tier">
        <xsl:param name="keywords"/>
        <xsl:param name="amount"/>
        <xsl:param name="input"/>
        <xsl:variable name="matchField" select="$input"/>

        <xsl:call-template name="generic-augment">
            <xsl:with-param name="matchList" select="$keywords"/>
            <xsl:with-param name="matchField" select="$matchField"/>
            <xsl:with-param name="targetElement" select="'.'"/>          <!-- current element -->
            <xsl:with-param name="targetAttr" select="'@difficulty'"/>
            <xsl:with-param name="mode" select="'sum'"/>
            <xsl:with-param name="value" select="$amount"/>
        </xsl:call-template>
    </xsl:template>

    <!-- SET IS_CIVILIAN -->
    <xsl:template name="set-is-civilian">
        <xsl:param name="keywords"/>
        <xsl:param name="input"/>
        <xsl:param name="value" select="'true'"/>
        <xsl:variable name="matchField" select="$input"/>

        <!-- Create tokens from comma-separated string -->
        <xsl:variable name="tokens">
            <tokens>
                <xsl:call-template name="tokenize">
                    <xsl:with-param name="input" select="$keywords"/>
                </xsl:call-template>
            </tokens>
        </xsl:variable>

        <!-- Precompute list of matching tokens -->
        <xsl:variable name="matches" select="exsl:node-set($tokens)/token[contains($matchField, normalize-space(.))]"/>

        <!-- If there's at least one match, this will be 'true', else 'false' -->
        <xsl:variable name="doIt" select="boolean($matches)"/>


        <xsl:if test="$doIt">
            <xsl:for-each select="Flags">
                <xsl:attribute name="Civilian">
                    <xsl:value-of select="$value"/>
                </xsl:attribute>
            </xsl:for-each>
        </xsl:if>
    </xsl:template>


    <!-- ================ end semantic helpers ================ -->


    <!-- 4) Core generic augmenting (augmented) -->
    <xsl:template name="generic-augment">
        <xsl:param name="matchList"/>
        <xsl:param name="input"/>               <!-- optional override of matchField -->
        <xsl:param name="targetElement"/>       <!-- e.g. 'Price', 'Flags', 'Armor', 'Weapon' -->
        <xsl:param name="targetAttr"/>          <!-- e.g. '.', '@weight', 'head_armor' -->
        <xsl:param name="mode"/>                <!-- 'sum' or 'product' -->
        <xsl:param name="value"/>

        <!-- ❶ compute matchField once -->
        <xsl:variable name="matchField" select="$input"/>

        <!-- ❷ dispatch flags -->
        <xsl:variable name="isPrice" select="$targetElement = 'Price'"/>
        <xsl:variable name="isFlags" select="$targetElement = 'Flags'"/>
        <xsl:variable name="isItemAttribute" select="starts-with($targetAttr,'@') and not($isFlags)"/>
        <xsl:variable name="isComponent" select="not($isPrice or $isFlags or $isItemAttribute)"/>

        <!-- ❸ split & test keywords -->
        <!-- Create tokens from comma-separated string -->
        <xsl:variable name="tokens">
            <tokens>
                <xsl:call-template name="tokenize">
                    <xsl:with-param name="input" select="matchList"/>
                </xsl:call-template>
            </tokens>
        </xsl:variable>
        <xsl:variable name="matches" select="exsl:node-set($tokens)/token[contains($matchField, normalize-space(.))]"/>

        <!-- If there's at least one match, this will be 'true', else 'false' -->
        <xsl:variable name="doIt" select="boolean($matches)"/>

        <xsl:if test="$doIt">
            <xsl:choose>
                <!-- A) Price element under Item -->
                <xsl:when test="$isPrice">
                    <xsl:for-each select="../Price">
                        <xsl:variable name="orig" select="number(.)"/>
                        <xsl:choose>
                            <xsl:when test="$mode = 'sum'">
                                <xsl:value-of select="$orig + number($value)"/>
                            </xsl:when>
                            <xsl:when test="$mode = 'product'">
                                <xsl:value-of select="$orig * number($value)"/>
                            </xsl:when>
                        </xsl:choose>
                    </xsl:for-each>
                </xsl:when>

                <!-- B) Flags child of Item -->
                <xsl:when test="$isFlags">
                    <xsl:for-each select="../Flags">
                        <xsl:variable name="attrName" select="substring-after($targetAttr,'@')"/>
                        <xsl:attribute name="{$attrName}">
                            <xsl:value-of select="$value"/>
                        </xsl:attribute>
                    </xsl:for-each>
                </xsl:when>

                <!-- C) Attribute on Item itself -->
                <xsl:when test="$isItemAttribute">
                    <xsl:variable name="attrName" select="substring-after($targetAttr,'@')"/>
                    <xsl:attribute name="{$attrName}">
                        <xsl:variable name="orig" select="number(../@*[name()=$attrName])"/>
                        <xsl:choose>
                            <xsl:when test="$mode = 'sum'">
                                <xsl:value-of select="$orig + number($value)"/>
                            </xsl:when>
                            <xsl:when test="$mode = 'product'">
                                <xsl:value-of select="$orig * number($value)"/>
                            </xsl:when>
                        </xsl:choose>
                    </xsl:attribute>
                </xsl:when>

                <!-- D) Component under ItemComponent -->
                <xsl:when test="$isComponent">
                    <xsl:for-each select="ItemComponent/*[local-name() = $targetElement]">
                        <xsl:variable name="orig">
                            <xsl:choose>
                                <xsl:when test="$targetAttr = '.'">
                                    <xsl:value-of select="number(.)"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="number(@*[name() = $targetAttr])"/>
                                </xsl:otherwise>
                            </xsl:choose>
                        </xsl:variable>
                        <xsl:choose>
                            <xsl:when test="$mode = 'sum'">
                                <xsl:if test="$targetAttr = '.'">
                                    <xsl:value-of select="$orig + number($value)"/>
                                </xsl:if>
                                <xsl:if test="$targetAttr != '.'">
                                    <xsl:attribute name="{$targetAttr}">
                                        <xsl:value-of select="$orig + number($value)"/>
                                    </xsl:attribute>
                                </xsl:if>
                            </xsl:when>
                            <xsl:when test="$mode = 'product'">
                                <xsl:if test="$targetAttr = '.'">
                                    <xsl:value-of select="$orig * number($value)"/>
                                </xsl:if>
                                <xsl:if test="$targetAttr != '.'">
                                    <xsl:attribute name="{$targetAttr}">
                                        <xsl:value-of select="$orig * number($value)"/>
                                    </xsl:attribute>
                                </xsl:if>
                            </xsl:when>
                        </xsl:choose>
                    </xsl:for-each>
                </xsl:when>
            </xsl:choose>
        </xsl:if>
    </xsl:template>


</xsl:stylesheet>

