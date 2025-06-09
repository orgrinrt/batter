<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:exsl="http://exslt.org/common"
        version="1.0"
        extension-element-prefixes="exsl"
>

    <!-- Integer-only power function -->
    <xsl:template name="pow">
        <xsl:param name="base"/>
        <xsl:param name="exp"/>
        <xsl:param name="acc" select="1"/>
        <xsl:choose>
            <xsl:when test="$exp &lt;= 0">
                <xsl:value-of select="$acc"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="pow">
                    <xsl:with-param name="base" select="$base"/>
                    <xsl:with-param name="exp" select="$exp - 1"/>
                    <xsl:with-param name="acc" select="$acc * $base"/>
                </xsl:call-template>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <!-- Template to find minimum value in a node-set -->
    <xsl:template name="min-node-set">
        <xsl:param name="nodes"/>
        <xsl:variable name="first" select="$nodes[1]"/>
        <xsl:for-each select="$nodes">
            <xsl:sort data-type="number" order="ascending"/>
            <xsl:if test="position() = 1">
                <xsl:value-of select="."/>
            </xsl:if>
        </xsl:for-each>
    </xsl:template>

    <!-- Template to find maximum value in a node-set -->
    <xsl:template name="max-node-set">
        <xsl:param name="nodes"/>
        <xsl:variable name="first" select="$nodes[1]"/>
        <xsl:for-each select="$nodes">
            <xsl:sort data-type="number" order="descending"/>
            <xsl:if test="position() = 1">
                <xsl:value-of select="."/>
            </xsl:if>
        </xsl:for-each>
    </xsl:template>

    <xsl:template name="abs">
        <xsl:param name="value"/>
        <xsl:choose>
            <xsl:when test="$value &lt; 0">
                <xsl:value-of select="0 - $value"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$value"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


    <!-- Approximate square root using Newton-Raphson -->
    <xsl:template name="sqrt">
        <xsl:param name="number"/>
        <xsl:param name="guess" select="1"/>
        <xsl:param name="prev" select="0"/>

        <xsl:variable name="delta" select="$guess - $prev"/>
        <xsl:variable name="absDelta">
            <xsl:call-template name="abs">
                <xsl:with-param name="value" select="$delta"/>
            </xsl:call-template>
        </xsl:variable>

        <xsl:choose>
            <xsl:when test="$absDelta &lt; 0.0001">
                <xsl:value-of select="$guess"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="sqrt">
                    <xsl:with-param name="number" select="$number"/>
                    <xsl:with-param name="guess" select="($guess + ($number div $guess)) div 2"/>
                    <xsl:with-param name="prev" select="$guess"/>
                </xsl:call-template>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


    <!-- Tokenize comma-separated string into <token> elements -->
    <xsl:template name="tokenize">
        <xsl:param name="input"/>
        <xsl:choose>
            <xsl:when test="contains($input, ',')">
                <token>
                    <xsl:value-of select="normalize-space(substring-before($input, ','))"/>
                </token>
                <xsl:call-template name="tokenize">
                    <xsl:with-param name="input" select="substring-after($input, ',')"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <token>
                    <xsl:value-of select="normalize-space($input)"/>
                </token>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


</xsl:stylesheet>
