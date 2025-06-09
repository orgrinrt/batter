<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:exsl="http://exslt.org/common"
                extension-element-prefixes="exsl">
  <xsl:output method="xml" indent="yes"/>

  <!-- Copy everything by default -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>

  <!-- ===========================================
       1) Armor & Weapon Items: split into culture-variants
     =========================================== -->
  <xsl:template match="
     Item[
         @type='Horse'
      or @type='HorseHarness'
      or @type='OneHandedWeapon'
      or @type='TwoHandedWeapon'
      or @type='Polearm'
      or @type='Arrows'
      or @type='Bolts'
      or @type='Shield'
      or @type='Bow'
      or @type='Crossbow'
      or @type='Thrown'
      or @type='Pistol'
      or @type='Musket'
      or @type='Bullets'
      or @type='HeadArmor'
      or @type='BodyArmor'
      or @type='LegArmor'
      or @type='HandArmor'
      or @type='ChestArmor'
      or @type='Cape'
      or @type='Banner'
     ]" priority="2">
     <!-- stash the whole original Item -->
  <xsl:variable name="orig" select="."/>

    <!-- 1a) gather all Variant nodes -->
    <xsl:variable name="allVariantsRtf">
      <xsl:call-template name="determine-culture-ids">
        <xsl:with-param name="name" select="@name"/>
        <xsl:with-param name="id"   select="@id"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="allVariants" select="exsl:node-set($allVariantsRtf)/Variant"/>

    <!-- 1b) emit one <Item> per matching culture -->
    <xsl:for-each select="$allVariants">
      <xsl:variable name="culture" select="normalize-space(culture)"/>
      <xsl:variable name="origid"   select="normalize-space(id)"/>

      <Item>
        <!-- copy everything except id & culture -->
        <xsl:apply-templates select="@*[local-name()!='culture' and local-name()!='id']"/>

        <!-- override id & culture, and explicitly turn ON merchant-flag on these variants -->
        <xsl:attribute name="culture">
          <xsl:value-of select="concat('Culture.',$culture)"/>
        </xsl:attribute>
        <xsl:attribute name="id">
          <xsl:value-of select="concat($origid,'_',$culture)"/>
        </xsl:attribute>
        <xsl:attribute name="isMerchantItem">true</xsl:attribute>

        <!-- decorate children -->
        <xsl:apply-templates select="$orig/node()" mode="decorate">
          <xsl:with-param name="culture"  select="$culture"/>
          <xsl:with-param name="itemType" select="@type"/>
        </xsl:apply-templates>
      </Item>
    </xsl:for-each>

    <!-- 1c) if none matched, emit a single neutral‐culture variant -->
    <xsl:if test="not($allVariants)">
      <Item>
        <xsl:apply-templates select="@*[local-name()!='culture' and local-name()!='id']"/>
        <xsl:attribute name="culture">neutral_culture</xsl:attribute>
        <xsl:attribute name="id">
          <xsl:value-of select="concat(@id,'_neutral')"/>
        </xsl:attribute>
        <xsl:attribute name="isMerchantItem">false</xsl:attribute>
        <xsl:apply-templates select="$orig/node()" mode="decorate">
          <xsl:with-param name="culture"  select="'neutral_culture'"/>
          <xsl:with-param name="itemType" select="@type"/>
        </xsl:apply-templates>
      </Item>
    </xsl:if>


  </xsl:template>


<!-- fallback in decorate mode: copy everything else -->
  <xsl:template match="@*|node()" mode="decorate">
   <xsl:param name="culture"/>
    <xsl:param name="itemType"/>
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" mode="decorate">
        <xsl:with-param name="culture"  select="$culture"/>
        <xsl:with-param name="itemType" select="$itemType"/>
      </xsl:apply-templates>
        </xsl:copy>
    </xsl:template>



    <!-- 3) your giant contains() -> list-of-ids logic -->
  <xsl:template name="determine-culture-ids">
    <xsl:param name="name"/>
    <xsl:param name="id"/>
    <xsl:variable name="ln"  select="translate($name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>
    <xsl:variable name="lid" select="translate($id,  'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')"/>

        <!-- everywhere in northern hemisphere -->
			<xsl:if test="contains($ln, 'fur') or contains($lid, 'fur') or
					        contains($ln, 'warm') or contains($lid, 'warm') or
							contains($ln, 'winter') or contains($lid, 'winter') or
							contains($ln, 'snow') or contains($lid, 'snow')">
<Variant>
  <culture>germanic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>welch</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>polish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>latvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sturgia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>scottish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>aestian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>kryvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>jatvingian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volga_fin</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>rus</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- everywhere in southern hemisphere -->
			<xsl:if test="contains($ln, 'turban') or contains($lid, 'turban') or
					        contains($ln, 'desert') or contains($lid, 'desert') or
							contains($ln, 'sand') or contains($lid, 'sand') or
							contains($ln, 'camel') or contains($lid, 'camel')">
				<Variant>
  <culture>aserai</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>ayyubid</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>persian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>bedouin_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>desert_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>spanish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sicilian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- VLANDIA KEYWORDS (Western European) -->
			<xsl:if test="contains($ln, 'vlandian') or contains($lid, 'vlandian') or
					        contains($ln, 'vlandia') or contains($lid, 'vlandia') or
							contains($ln, '[eoe western') or contains($lid, '[eoe western') or
							contains($ln, 'eoe western') or contains($lid, 'eoe western') or
							contains($ln, '[eoe crusader') or contains($lid, '[eoe crusader') or
							contains($ln, 'eoe crusader') or contains($lid, 'eoe crusader') or
							contains($ln, '[eoe heraldic') or contains($lid, '[eoe heraldic') or
							contains($ln, 'eoe heraldic') or contains($lid, 'eoe heraldic') or
							contains($ln, 'norman') or contains($lid, 'norman') or
					        contains($ln, 'western') or contains($lid, 'western') or
					        contains($ln, 'haubergeon') or contains($lid, 'haubergeon') or
							contains($ln, 'frankish') or contains($lid, 'frankish') or
					        contains($ln, 'french') or contains($lid, 'french') or
					        contains($ln, 'german') or contains($lid, 'german') or
					        contains($ln, 'gothic') or contains($lid, 'gothic') or
					        contains($ln, 'churburg') or contains($lid, 'churburg') or
							contains($ln, 'knight') or contains($lid, 'knight') or
							contains($ln, 'crusader') or contains($lid, 'crusader') or
					        contains($ln, 'maciejowski') or contains($lid, 'maciejowski') or
                            contains($ln, 'english') or contains($lid, 'english') or
							contains($ln, 'templar') or contains($lid, 'templar') or
							contains($ln, 'hospitaller') or contains($lid, 'hospitaller') or
							contains($ln, 'teutonic') or contains($lid, 'teutonic') or
							contains($ln, 'surcoat') or contains($lid, 'surcoat') or
							contains($ln, 'great helm') or contains($lid, 'great_helm') or
							contains($ln, 'fleur') or contains($lid, 'fleur') or
							contains($ln, 'heraldic') or contains($lid, 'heraldic') or
							contains($ln, 'chivalric') or contains($lid, 'chivalric') or
							contains($ln, 'courtly') or contains($lid, 'courtly') or
							contains($ln, 'bayeux') or contains($lid, 'bayeux')">
				<Variant>
  <culture>germanic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>brigands</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>armenia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sicilian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>roman</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>vlandia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>polish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>spain</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>empire</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>english</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>welch</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>bulgarian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>latvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- EMPIRE KEYWORDS (Roman/Byzantine) -->
			<xsl:if test="contains($ln, 'empire') or contains($lid, 'empire') or
							contains($ln, '[eoe byzantine') or contains($lid, '[eoe byzantine') or
							contains($ln, 'eoe byzantine') or contains($lid, 'eoe byzantine') or
							contains($ln, 'imperial') or contains($lid, 'imperial') or
							contains($ln, 'roman') or contains($lid, 'roman') or
					        contains($ln, 'greek') or contains($lid, 'greek') or
					        contains($ln, 'chamoson') or contains($lid, 'chamoson') or
					        contains($ln, 'olmutz') or contains($lid, 'olmutz') or
					        contains($ln, 'olmut') or contains($lid, 'olmut') or
							contains($ln, 'byzantine') or contains($lid, 'byzantine') or
							contains($ln, 'legion') or contains($lid, 'legion') or
					        contains($ln, 'sinj') or contains($lid, 'sinj') or
					        contains($ln, 'phrygian') or contains($lid, 'phrygian') or
					        contains($ln, 'legionary') or contains($lid, 'legionary') or
					        contains($ln, 'varangian') or contains($lid, 'varangian') or
                            contains($ln, 'varangoi') or contains($lid, 'varangoi') or
                            contains($ln, 'varang') or contains($lid, 'varang') or
							contains($ln, 'centurion') or contains($lid, 'centurion') or
							contains($ln, 'praetorian') or contains($lid, 'praetorian') or
							contains($ln, 'palatine') or contains($lid, 'palatine') or
							contains($ln, 'cataphract') or contains($lid, 'cataphract') or
							contains($ln, 'klivanion') or contains($lid, 'klivanion') or
							contains($ln, 'kavadion') or contains($lid, 'kavadion') or
							contains($ln, 'scaramangion') or contains($lid, 'scaramangion') or
							contains($ln, 'varangian') or contains($lid, 'varangian') or
							contains($ln, 'scholae') or contains($lid, 'scholae') or
							contains($ln, 'lorica') or contains($lid, 'lorica') or
							contains($ln, 'squamata') or contains($lid, 'squamata')">
				<Variant>
  <culture>rus</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>brigands</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>bulgarian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>armenia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sicilian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>roman</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>georgia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>byzantine</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>portuguese</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- STURGIA KEYWORDS (Nordic/Slavic) -->
			<xsl:if test="contains($ln, 'sturgian') or contains($lid, 'sturgian') or
					        contains($ln, 'sturgia') or contains($lid, 'sturgia') or
							contains($ln, '[eoe nordic') or contains($lid, '[eoe nordic') or
							contains($ln, 'eoe nordic') or contains($lid, 'eoe nordic') or
					        contains($ln, 'gjermundbu') or contains($lid, 'gjermundbu') or
					        contains($ln, 'lokrume') or contains($lid, 'lokrume') or
					        contains($ln, 'tjele') or contains($lid, 'tjele') or
							contains($ln, '[eoe northern') or contains($lid, '[eoe northern') or
							contains($ln, 'eoe northern') or contains($lid, 'eoe northern') or
							contains($ln, '[eoe rus') or contains($lid, '[eoe rus') or
							contains($ln, 'eoe rus') or contains($lid, 'eoe rus') or
							contains($ln, '[eoe east europe') or contains($lid, '[eoe east europe') or
							contains($ln, 'eoe east europe') or contains($lid, 'eoe east europe') or
							contains($ln, 'nordic') or contains($lid, 'nordic') or
					        contains($ln, 'pecs') or contains($lid, 'pecs') or
					        contains($ln, 'klappenrock') or contains($lid, 'klappenrock') or
					        contains($ln, 'birka') or contains($lid, 'birka') or
					        contains($ln, 'broe') or contains($lid, 'broe') or
							contains($ln, 'northern') or contains($lid, 'northern') or
					        contains($ln, 'gnezdovo') or contains($lid, 'gnezdovo') or
							contains($ln, 'nord') or contains($lid, 'nord') or
					        contains($ln, 'boyar') or contains($lid, 'boyar') or
					        contains($ln, 'druzhina') or contains($lid, 'druzhina') or
					        contains($ln, 'orchowski') or contains($lid, 'orchowski') or
					        contains($ln, 'yarm') or contains($lid, 'yarm') or
					        contains($ln, 'verden') or contains($lid, 'verden') or
					        contains($ln, 'kiev') or contains($lid, 'kiev') or
							contains($ln, 'norse') or contains($lid, 'norse') or
					        contains($ln, 'byrnie') or contains($lid, 'byrnie') or
							contains($ln, 'viking') or contains($lid, 'viking') or
							contains($ln, 'vaegir') or contains($lid, 'vaegir') or
							contains($ln, 'huscarl') or contains($lid, 'huscarl') or
							contains($ln, 'berserker') or contains($lid, 'berserker') or
							contains($ln, 'ulfhednar') or contains($lid, 'ulfhednar') or
							contains($ln, 'lithuanian') or contains($lid, 'lithuanian') or
							contains($ln, 'gjermundbu') or contains($lid, 'gjermundbu') or
							contains($ln, 'vendel') or contains($lid, 'vendel') or
							contains($ln, 'spectacle') or contains($lid, 'spectacle') or
							contains($ln, 'wolf') or contains($lid, 'wolf') or
							contains($ln, 'bear') or contains($lid, 'bear') or
							contains($ln, 'winter') or contains($lid, 'winter') or
							contains($ln, 'rus') or contains($lid, 'rus') or
							contains($ln, 'slavic') or contains($lid, 'slavic') or
							contains($ln, 'sloven') or contains($lid, 'sloven')">
				<Variant>
  <culture>hungary</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volga_fin</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sturgia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volhynian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>scottish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>aestian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>germanic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>pirates</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>brigands</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>nordic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sea_raiders</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>armenia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>jatvingian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>latvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sicilian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- BATTANIA KEYWORDS (Celtic/Highland) -->
			<xsl:if test="contains($ln, 'battanian') or contains($lid, 'battanian') or
					        contains($ln, 'battania') or contains($lid, 'battania') or
							contains($ln, 'highland') or contains($lid, 'highland') or
							contains($ln, 'celtic') or contains($lid, 'celtic') or
							contains($ln, 'gaelic') or contains($lid, 'gaelic') or
					        contains($ln, 'druid') or contains($lid, 'druid') or
					        contains($ln, 'fianna') or contains($lid, 'fianna') or
							contains($ln, 'pictish') or contains($lid, 'pictish') or
					        contains($ln, 'pict') or contains($lid, 'pict') or
					        contains($ln, 'sash') or contains($lid, 'sash') or
					        contains($ln, 'ridge') or contains($lid, 'ridge') or
					        contains($ln, 'roughscale') or contains($lid, 'roughscale') or
					        contains($ln, 'pioneer') or contains($lid, 'pioneer') or
							contains($ln, 'irish') or contains($lid, 'irish') or
							contains($ln, 'scottish') or contains($lid, 'scottish') or
							contains($ln, 'welsh') or contains($lid, 'welsh') or
							contains($ln, 'forest') or contains($lid, 'forest') or
							contains($ln, 'woodland') or contains($lid, 'woodland') or
					        contains($ln, 'forester') or contains($lid, 'forester') or
					        contains($ln, 'anglo') or contains($lid, 'anglo') or
					        contains($ln, 'saxon') or contains($lid, 'saxon') or
							contains($ln, 'tartan') or contains($lid, 'tartan') or
							contains($ln, 'plaid') or contains($lid, 'plaid') or
							contains($ln, 'coppergate') or contains($lid, 'coppergate')">
				<Variant>
  <culture>english</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>empire</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>hungary</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volga_fin</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>battania</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sturgia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>polish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>welch</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volhynian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>scottish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>serbian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>latvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>goldenhorde</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>jatvingian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>aestian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>germanic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>forest_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>nordic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>rus</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>

			<!-- KHUZAIT KEYWORDS (Mongol/Turkic/Steppe) -->
			<xsl:if test="contains($ln, 'khuzait') or contains($lid, 'khuzait') or
							contains($ln, 'mongol') or contains($lid, 'mongol') or
							contains($ln, 'turkic') or contains($lid, 'turkic') or
					        contains($ln, 'samurai') or contains($lid, 'samurai') or
					        contains($ln, 'chinese') or contains($lid, 'chinese') or
					        contains($ln, 'tartar') or contains($lid, 'tartar') or
					        contains($ln, 'cuman') or contains($lid, 'cuman') or
					        contains($ln, 'eastern') or contains($lid, 'eastern') or
					        contains($ln, 'khan') or contains($lid, 'khan') or
							contains($ln, 'steppe') or contains($lid, 'steppe') or
							contains($ln, 'nomad') or contains($lid, 'nomad') or
                            contains($ln, 'nomadic') or contains($lid, 'nomadic')">
                <Variant>
  <culture>khuzait</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>persian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volga_fin</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>volhynian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>serbian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>georgia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>hungary</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>armenia</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>dregovian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>kryvian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>turkic_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>steppe_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>mountain_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>bedouin_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>forest_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>rus</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>goldenhorde</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
            </xsl:if>

			<!-- ASERAI KEYWORDS (Arab/MENA/Desert) -->
			<xsl:if test="contains($ln, 'aserai') or contains($lid, 'aserai') or
							contains($ln, 'sarranid') or contains($lid, 'sarranid') or
							contains($ln, '[eoe mena') or contains($lid, '[eoe mena') or
							contains($ln, 'eoe mena') or contains($lid, 'eoe mena') or
							contains($ln, '[eoe iberia') or contains($lid, '[eoe iberia') or
							contains($ln, 'eoe iberia') or contains($lid, 'eoe iberia') or
							contains($ln, 'arab') or contains($lid, 'arab') or
					        contains($ln, 'arabic') or contains($lid, 'arabic') or
							contains($ln, 'arabian') or contains($lid, 'arabian') or
					        contains($ln, 'moor') or contains($lid, 'moor') or
					        contains($ln, 'moorish') or contains($lid, 'moorish') or
					        contains($ln, 'southern') or contains($lid, 'southern') or
							contains($ln, 'bedouin') or contains($lid, 'bedouin') or
							contains($ln, 'desert') or contains($lid, 'desert') or
					        contains($ln, 'pillbox') or contains($lid, 'pillbox') or
							contains($ln, 'mena') or contains($lid, 'mena') or
					        contains($ln, 'hijab') or contains($lid, 'hijab') or
					        contains($ln, 'keffiyeh') or contains($lid, 'keffiyeh') or
					        contains($ln, 'andalusian') or contains($lid, 'andalusian') or
					        contains($ln, 'spanish') or contains($lid, 'spanish') or
							contains($ln, 'muslim') or contains($lid, 'muslim') or
							contains($ln, 'islamic') or contains($lid, 'islamic') or
							contains($ln, 'ayyubid') or contains($lid, 'ayyubid') or
							contains($ln, 'mamluke') or contains($lid, 'mamluke') or
							contains($ln, 'berber') or contains($lid, 'berber') or
							contains($ln, 'saracen') or contains($lid, 'saracen') or
							contains($ln, 'seljuk') or contains($lid, 'seljuk') or
							contains($ln, 'iberian') or contains($lid, 'iberian') or
							contains($ln, 'iberia') or contains($lid, 'iberia') or
							contains($ln, 'tagelmust') or contains($lid, 'tagelmust') or
							contains($ln, 'kairouan') or contains($lid, 'kairouan') or
							contains($ln, 'turban') or contains($lid, 'turban')">
				<Variant>
  <culture>khuzait</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>aserai</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>ayyubid</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>persian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>bedouin_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>desert_bandits</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>pirates</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>spanish</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>sicilian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
<Variant>
  <culture>italian</culture>
  <id><xsl:value-of select="$id"/></id>
</Variant>
			</xsl:if>
    </xsl:template>

</xsl:stylesheet>
