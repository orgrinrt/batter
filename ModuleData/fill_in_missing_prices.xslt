<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:exsl="http://exslt.org/common"
  extension-element-prefixes="exsl"
>
  <xsl:import href="utils.xslt"/>

  <!-- ================================================================= -->
  <!--  PARAMETERS (tweak here!)                                      -->
  <!-- ================================================================= -->
  <!-- overrideable global price bounds -->
  <xsl:param name="targetMinPrice" select="-150"/>
  <xsl:param name="targetMaxPrice" select="75000"/>

  <!-- material multipliers -->
  <xsl:param name="mul-Cloth"   select="1.075"/>
  <xsl:param name="mul-Leather" select="1.2"/>
  <xsl:param name="mul-Plate"   select="1.375"/>
  <xsl:param name="mul-Chain"   select="1.225"/>
  <xsl:param name="mul-Default" select="1.0"/>

  <!-- weights for statSum components -->
  <xsl:param name="w-headArmor"   select="1.0"/>
  <xsl:param name="w-bodyArmor"   select="1.0"/>
  <xsl:param name="w-legArmor"    select="1.0"/>
  <xsl:param name="w-armArmor"    select="1.0"/>
  <xsl:param name="w-swingDmg"    select="1.0"/>
  <xsl:param name="w-thrustDmg"   select="1.0"/>
  <xsl:param name="w-missileDmg"  select="1.0"/>
  <xsl:param name="w-handling"    select="1.0"/>

  <!-- weights for difficulty & appearance -->
  <xsl:param name="w-difficulty"   select="1.0"/>
  <xsl:param name="w-appearance"   select="1.0"/>

  <!-- ================================================================= -->
  <!-- ITEM-TYPE COST MULTIPLIERS (per @Type)         -->
  <!-- ================================================================= -->

  <!-- These get multiplied into your “power” calc.   -->
  <xsl:param name="mul-HeadArmor"    select="1.00"/>
  <xsl:param name="mul-BodyArmor"    select="1.00"/>
  <xsl:param name="mul-HandArmor"    select="0.80"/>
  <xsl:param name="mul-LegArmor"     select="0.85"/>
  <xsl:param name="mul-Boots"        select="0.75"/>
  <xsl:param name="mul-Shield"       select="0.90"/>
  <xsl:param name="mul-OneHanded"    select="1.00"/>
  <xsl:param name="mul-TwoHanded"    select="1.10"/>
  <xsl:param name="mul-Polearm"      select="1.05"/>
  <xsl:param name="mul-Bow"          select="1.05"/>
  <xsl:param name="mul-Cape"         select="0.50"/>
  <xsl:param name="mul-Horse"        select="1.20"/>
  <xsl:param name="mul-DefaultType"  select="1.00"/>

  <!-- modifier_group multipliers -->
  <xsl:param name="mul-modifier-cloth_unarmoured" select="0.8"/>
  <xsl:param name="mul-modifier-leather"          select="1.1"/>
  <xsl:param name="mul-modifier-mail"             select="1.3"/>
  <xsl:param name="mul-modifier-plate"            select="1.5"/>
  <xsl:param name="mul-modifier-shield"           select="1.1"/>
  <xsl:param name="mul-modifier-bow"              select="1.2"/>
  <xsl:param name="mul-modifier-default"          select="1.0"/>

  <!-- subtype multipliers -->
  <xsl:param name="mul-subtype-helmet"     select="1.125"/>
  <xsl:param name="mul-subtype-body_armor" select="1.375"/>
  <xsl:param name="mul-subtype-leg_armor"  select="0.85"/>
  <xsl:param name="mul-subtype-hand_armor" select="0.8"/>
  <xsl:param name="mul-subtype-default"    select="1.0"/>

  <!-- ================================================================= -->
  <!--  END PARAMETERS                                                 -->
  <!-- ================================================================= -->

  <!-- key to group items by @Type -->
  <xsl:key name="items-by-type" match="Item" use="@Type"/>

  <!-- identity copy -->
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- build stats with min/max power & price per Type -->
  <xsl:variable name="stats">
    <stats>
      <xsl:for-each select="//Item[generate-id() = generate-id(key('items-by-type', @Type)[1])]">
        <stat type="{@Type}">
          <xsl:variable name="grp" select="key('items-by-type', @Type)"/>
          <xsl:variable name="priced" select="$grp"/>
          <xsl:if test="count($grp) &gt; 0">
            <!-- compute each priced item's power -->
            <xsl:variable name="powers">
              <xsl:for-each select="$grp">
                <pwr>
                  <!-- 1) weighted statSum -->
                  <xsl:variable name="statSum" select="
                    number(ItemComponent/*[1]/@head_armor or 0)   * $w-headArmor
                  + number(ItemComponent/*[1]/@body_armor or 0)   * $w-bodyArmor
                  + number(ItemComponent/*[1]/@leg_armor or 0)    * $w-legArmor
                  + number(ItemComponent/*[1]/@arm_armor or 0)    * $w-armArmor
                  + number(ItemComponent/*[1]/@swing_damage or 0)* $w-swingDmg
                  + number(ItemComponent/*[1]/@thrust_damage or 0)* $w-thrustDmg
                  + number(ItemComponent/*[1]/@missile_damage or 0)*$w-missileDmg
                  + number(ItemComponent/*[1]/@speed_rating or 0)* $w-handling
                  "/>
                  <!-- 2) weighted difficulty & appearance -->
                  <xsl:variable name="diff" select="number(@difficulty or 1.25) * $w-difficulty"/>
                  <xsl:variable name="app"  select="number(@appearance or 0.75) * $w-appearance"/>
                  <!-- 3) material multiplier -->
                  <xsl:variable name="matType" select="ItemComponent/*[1]/@material_type"/>
                  <xsl:variable name="matMul">
                    <xsl:choose>
                      <xsl:when test="$matType = 'Cloth'">{$mul-Cloth}</xsl:when>
                      <xsl:when test="$matType = 'Leather'">{$mul-Leather}</xsl:when>
                      <xsl:when test="$matType = 'Plate'">{$mul-Plate}</xsl:when>
                      <xsl:when test="$matType = 'Chainmail'">{$mul-Chain}</xsl:when>
                      <xsl:when test="$matType = 'Chain'">{$mul-Chain}</xsl:when>
                      <xsl:otherwise>{$mul-Default}</xsl:otherwise>
                    </xsl:choose>
                  </xsl:variable>
                  <!-- 4) final power -->
                  <xsl:value-of select="round($statSum * $matMul + $diff + $app)"/>
                </pwr>
              </xsl:for-each>
            </xsl:variable>

            <!-- Call custom min -->
            <xsl:variable name="minPwr">
              <xsl:call-template name="min-node-set">
                <xsl:with-param name="nodes" select="exsl:node-set($powers)/pwr"/>
              </xsl:call-template>
            </xsl:variable>

            <!-- Call custom max -->
            <xsl:variable name="maxPwr">
              <xsl:call-template name="max-node-set">
                <xsl:with-param name="nodes" select="exsl:node-set($powers)/pwr"/>
              </xsl:call-template>
            </xsl:variable>

            <minPwr><xsl:value-of select="$minPwr"/></minPwr>
            <maxPwr><xsl:value-of select="$maxPwr"/></maxPwr>

          </xsl:if>
        </stat>
      </xsl:for-each>
    </stats>
  </xsl:variable>

  <!-- interpolation curve factor -->
  <xsl:variable name="interpolation-power" select="1.0"/>

  <!-- cached stats set -->
  <xsl:variable name="statsSet" select="exsl:node-set($stats)/stats"/>

  <!-- for each Item missing Price, interpolate and insert one -->
  <xsl:template match="Item[not(Price) or normalize-space(Price)='']">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>

      <xsl:variable name="type" select="@Type"/>
      <xsl:variable name="st" select="$statsSet/stat[@type=$type]"/>


      <xsl:if test="$st">
        <!-- extended weighted statSum -->
        <xsl:variable name="statSum" select="
          number(ItemComponent/*[1]/@head_armor or 0 )     * $w-headArmor
        + number(ItemComponent/*[1]/@body_armor or 0 )     * $w-bodyArmor
        + number(ItemComponent/*[1]/@leg_armor or 0 )      * $w-legArmor
        + number(ItemComponent/*[1]/@arm_armor or 0 )      * $w-armArmor
        + number(ItemComponent/*[1]/@swing_damage or 0 )   * $w-swingDmg
        + number(ItemComponent/*[1]/@thrust_damage or 0 )  * $w-thrustDmg
        + number(ItemComponent/*[1]/@missile_damage or 0 ) * $w-missileDmg
        + number(ItemComponent/*[1]/@speed_rating or 0 )   * $w-handling
        + number(ItemComponent/*[1]/@hit_points or 0 )     * 0.005
        + number(ItemComponent/*[1]/@weapon_length or 0 )  * 0.01
        + number(ItemComponent/*[1]/@accuracy or 0 )       * 0.05
        "/>

        <xsl:variable name="diff" select="number(@difficulty) or 1  * $w-difficulty"/>

        <!-- appearance scaled via sqrt to prevent over-weighting -->
        <xsl:variable name="rawApp" select="number(@appearance) or 1.25"/>
        <xsl:variable name="sqrtApp">
          <xsl:call-template name="sqrt">
            <xsl:with-param name="number" select="$rawApp"/>
          </xsl:call-template>
        </xsl:variable>

        <xsl:variable name="app" select="$sqrtApp * $w-appearance"/>


        <xsl:variable name="matType" select="ItemComponent/*[1]/@material_type"/>
        <xsl:variable name="matMul">
          <xsl:choose>
            <xsl:when test="$matType = 'Cloth'">{$mul-Cloth}</xsl:when>
            <xsl:when test="$matType = 'Leather'">{$mul-Leather}</xsl:when>
            <xsl:when test="$matType = 'Plate'">{$mul-Plate}</xsl:when>
            <xsl:when test="$matType = 'Chainmail'">{$mul-Chain}</xsl:when>
            <xsl:when test="$matType = 'Chain'">{$mul-Chain}</xsl:when>
            <xsl:otherwise>{$mul-Default}</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <xsl:variable name="typeMul">
          <xsl:choose>
            <xsl:when test="@Type='HeadArmor'">{$mul-HeadArmor}</xsl:when>
            <xsl:when test="@Type='BodyArmor'">{$mul-BodyArmor}</xsl:when>
            <xsl:when test="@Type='HandArmor'">{$mul-HandArmor}</xsl:when>
            <xsl:when test="@Type='LegArmor'">{$mul-LegArmor}</xsl:when>
            <xsl:when test="@Type='Boots'">{$mul-Boots}</xsl:when>
            <xsl:when test="@Type='Shield'">{$mul-Shield}</xsl:when>
            <xsl:when test="@Type='OneHandedWeapon'">{$mul-OneHanded}</xsl:when>
            <xsl:when test="@Type='TwoHandedWeapon'">{$mul-TwoHanded}</xsl:when>
            <xsl:when test="@Type='Polearm'">{$mul-Polearm}</xsl:when>
            <xsl:when test="@Type='Bow'">{$mul-Bow}</xsl:when>
            <xsl:when test="@Type='Cape'">{$mul-Cape}</xsl:when>
            <xsl:when test="@Type='Horse'">{$mul-Horse}</xsl:when>
            <xsl:otherwise>{$mul-DefaultType}</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <!-- Lookup modifier_group multiplier -->
        <xsl:variable name="modGroup" select="ItemComponent/*[1]/@modifier_group"/>
        <xsl:variable name="modMul">
          <xsl:choose>
            <xsl:when test="$modGroup = 'cloth_unarmoured'">{$mul-modifier-cloth_unarmoured}</xsl:when>
            <xsl:when test="$modGroup = 'leather'">{$mul-modifier-leather}</xsl:when>
            <xsl:when test="$modGroup = 'mail'">{$mul-modifier-mail}</xsl:when>
            <xsl:when test="$modGroup = 'plate'">{$mul-modifier-plate}</xsl:when>
            <xsl:when test="$modGroup = 'shield'">{$mul-modifier-shield}</xsl:when>
            <xsl:when test="$modGroup = 'bow'">{$mul-modifier-bow}</xsl:when>
            <xsl:otherwise>{$mul-modifier-default}</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>

        <!-- Lookup subtype multiplier -->
        <xsl:variable name="subtype" select="@subtype"/>
        <xsl:variable name="subMul">
          <xsl:choose>
            <xsl:when test="$subtype = 'helmet'">{$mul-subtype-helmet}</xsl:when>
            <xsl:when test="$subtype = 'body_armor'">{$mul-subtype-body_armor}</xsl:when>
            <xsl:when test="$subtype = 'leg_armor'">{$mul-subtype-leg_armor}</xsl:when>
            <xsl:when test="$subtype = 'hand_armor'">{$mul-subtype-hand_armor}</xsl:when>
            <xsl:otherwise>{$mul-subtype-default}</xsl:otherwise>
          </xsl:choose>
        </xsl:variable>


        <!-- now final power -->
        <xsl:variable name="pwr" select="
          (
            ( $statSum * $matMul
              + $diff + $app
            ) * $typeMul * $modMul * $subMul
          )
        "/>


        <xsl:variable name="minpwr" select="number($st/minPwr)"/>
        <xsl:variable name="maxpwr" select="number($st/maxPwr)"/>
        <xsl:variable name="minpr"  select="$targetMinPrice"/>
        <xsl:variable name="maxpr"  select="$targetMaxPrice"/>


        <!-- interpolate with curve (power factor) -->
        <xsl:variable name="computed">
          <xsl:choose>
            <xsl:when test="$maxpwr &gt; $minpwr">
              <!-- Normalize input -->
              <xsl:variable name="normalized" select="($pwr - $minpwr) div ($maxpwr - $minpwr)"/>

              <!-- Apply power curve -->
              <xsl:variable name="curved">
                <xsl:call-template name="pow">
                  <xsl:with-param name="base" select="$normalized"/>
                  <xsl:with-param name="exp" select="$interpolation-power"/>
                </xsl:call-template>
              </xsl:variable>

              <!-- Final computed result -->
              <xsl:value-of select="round($curved * ($maxpr - $minpr) + $minpr)"/>
            </xsl:when>

            <xsl:otherwise>
              <xsl:value-of select="$minpr"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>



        <ComputedPrice><xsl:value-of select="$computed"/></ComputedPrice>
      </xsl:if>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
