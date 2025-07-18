<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template mode="game" match="LevelReports">
		<div class="expandableSection">
			<h2 class="expandableSectionHeader">Levels</h2>
			<div class="expandableSectionContent">
				<xsl:apply-templates mode="level" />
			</div>
		</div>
	</xsl:template>

	<xsl:template mode="level" match="LevelReport">
		<div class="expandableSection">
			<span class="expandableSectionHeader">
				<b>
					<xsl:value-of select="@LevelName"/>
				</b>
			</span>
			<div class="expandableSectionContent">
				<xsl:apply-templates mode="level" />
			</div>
		</div>
	</xsl:template>

	<!-- Scripting -->

	<xsl:template mode="level" match="Scripting">
		<div class="expandableSection">
			<span class="expandableSectionHeader">
				Scripting
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="scripting" />
				</ul>
			</div>
		</div>
	</xsl:template>

	<xsl:template mode="scripting" match="Folder">
		<div class="expandableSection">
			<span class="expandableSectionHeader">
				<xsl:value-of select="@Name"/>
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="scripting" />
				</ul>
			</div>
		</div>
	</xsl:template>

	<xsl:template mode="scripting" match="Entrypoint">
		<li>
			<span style="color: orange; margin-right: 0.9em;                    ">Entrypoint</span>
			<span style="               margin-right: 0.2em; margin-left: 0.9em;">
				<xsl:value-of select="@Name" />
			</span>
			<span style="color: grey;   margin-left: 0.2em;                     ">
				<xsl:value-of select="node()"/>
			</span>
			<xsl:element name="a">
				<xsl:attribute name="href">
					#<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="node()" />
				</xsl:attribute>
				<xsl:attribute name="style">
					text-decoration: none; color: cyan;
				</xsl:attribute>
				(Jump)
			</xsl:element>
		</li>
	</xsl:template>

	<xsl:template mode="scripting" match="Composite">
		<li>
			<xsl:element name="div">
				<xsl:attribute name="class">expandableSection</xsl:attribute>
				<xsl:attribute name="id">
					<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="@GUID" />
				</xsl:attribute>
				<span class="expandableSectionHeader">
					<span style="             margin-right: 0.2em;                    ">
						<xsl:value-of select="@Name"/>
					</span>
					<span style="color: grey;                      margin-left: 0.2em;">
						<xsl:value-of select="@GUID"/>
					</span>
					<xsl:if test="@Entrypoint">
						<span style="color: orange; margin-left: 0.4em;">
							Entrypoint
						</span>
					</xsl:if>
				</span>
				<div class="expandableSectionContent">
					<ul>
						<xsl:apply-templates mode="composite" />
					</ul>
				</div>
			</xsl:element>
		</li>
	</xsl:template>

	<xsl:template mode="composite" match="Function">
		<li>
		<xsl:element name="div">
			<xsl:attribute name="class">expandableSection</xsl:attribute>
			<xsl:attribute name="id">
				<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@GUID" />
			</xsl:attribute>
			<span class="expandableSectionHeader">
				<span style="color: yellow; margin-right: 0.9em;                    ">Function</span>
				<span style="               margin-right: 0.2em; margin-left: 0.9em;">
					<xsl:value-of select="@Name" />
				</span>
				<span style="color: yellow; margin-right: 0.2em; margin-left: 0.2em;">
					<xsl:value-of select="@Type"/>
				</span>
				<span style="color: grey;   margin-left: 0.2em;                     ">
					<xsl:value-of select="@GUID"/>
				</span>
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="entity" />
				</ul>
			</div>
		</xsl:element>
		</li>
	</xsl:template>

	<xsl:template mode="composite" match="Variable">
		<li>
		<xsl:element name="div">
			<xsl:attribute name="class">expandableSection</xsl:attribute>
			<xsl:attribute name="id">
				<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@GUID" />
			</xsl:attribute>
			<span class="expandableSectionHeader">
				<span style="color: orange; margin-right: 0.9em;                    ">Variable</span>
				<span style="               margin-right: 0.2em; margin-left: 0.9em;">
					<xsl:value-of select="@Name" />
				</span>
				<span style="color: yellow; margin-right: 0.2em; margin-left: 0.2em;">
					<xsl:value-of select="@Type"/>
				</span>
				<span style="color: grey;   margin-left: 0.2em;                     ">
					<xsl:value-of select="@GUID"/>
				</span>
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="entity" />
				</ul>
			</div>
		</xsl:element>
		</li>
	</xsl:template>

	<xsl:template mode="composite" match="Parameter">
		<xsl:element name="div">
			<xsl:attribute name="class">expandableSection</xsl:attribute>
			<xsl:attribute name="id">
				<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@GUID" />
			</xsl:attribute>
			<span class="expandableSectionHeader">
				<span style="color: lime;   margin-right: 0.9em;                    ">Parameter</span>
				<span style="               margin-right: 0.2em; margin-left: 0.9em;">
					<xsl:value-of select="@Name" />
				</span>
				<span style="color: yellow; margin-right: 0.2em; margin-left: 0.2em;">
					<xsl:value-of select="@Type"/>
				</span>
				<span style="color: grey;   margin-left: 0.2em;                     ">
					<xsl:value-of select="@Variant"/>
				</span>
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="entity" />
				</ul>
			</div>
		</xsl:element>
	</xsl:template>

	<xsl:template mode="composite" match="Instance">
		<li>
		<xsl:element name="div">
			<xsl:attribute name="class">expandableSection</xsl:attribute>
			<xsl:attribute name="id">
				<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@GUID" />
			</xsl:attribute>
			<span class="expandableSectionHeader">
				<span style="color: cyan; margin-right: 0.9em;"                    >Instance</span>
				<span style="             margin-right: 0.2em; margin-left: 0.9em;">
					<xsl:value-of select="@Name" />
				</span>
				<span style="color: grey; margin-right: 0.5em; margin-left: 0.2em;">
					<xsl:value-of select="@GUID"/>
				</span>
				<span style="             margin-right: 0.5em; margin-left: 0.5em;"> => </span>
				<xsl:element name="a">
					<xsl:attribute name="href">
						#<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="@CompositeGUID" />
					</xsl:attribute>
					<xsl:attribute name="style">
						text-decoration: none;
					</xsl:attribute>
					<span style="color: white; margin-right: 0.2em; margin-left: 0.5em;">
						<xsl:value-of select="@CompositeName" />
					</span>
					<span style="color: grey;                      margin-left: 0.2em;">
						<xsl:value-of select="@CompositeGUID"/>
					</span>
				</xsl:element>
			</span>
			<div class="expandableSectionContent">
				<ul>
					<xsl:apply-templates mode="entity" />
				</ul>
			</div>
		</xsl:element>
		</li>
	</xsl:template>

	<xsl:template mode="entity" match="InLink">
		<xsl:element name="li">
			<xsl:attribute name="id"><xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="parent::*/@GUID" />/in-<xsl:value-of select="@TargetParam" />-<xsl:value-of select="@SourceID" />-<xsl:value-of select="@SourceParam" /></xsl:attribute>
			<span>
				<span style="color: red; margin-right: 0.2em;">In Link</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@TargetParam" />
				</span>
				<span style="color: red; margin-left: 0.2em; margin-right: 0.2em;">&lt;</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@SourceParam" />
				</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@SourceName" />
				</span>
				<span style="color: grey; margin-left: 0.2em;">
					<xsl:value-of select="@SourceID" />
				</span>
				<xsl:element name="a">
					<xsl:attribute name="href">#<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@SourceID" />/out-<xsl:value-of select="@SourceParam" />-<xsl:value-of select="parent::*/@GUID" />-<xsl:value-of select="@TargetParam" /></xsl:attribute>
					<xsl:attribute name="style">
						text-decoration: none; color: cyan;
					</xsl:attribute>
					(Jump to Out Link)
				</xsl:element>
			</span>
		</xsl:element>
	</xsl:template>

	<xsl:template mode="entity" match="OutLink">
		<xsl:element name="li">
			<xsl:attribute name="id"><xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="parent::*/@GUID" />/out-<xsl:value-of select="@SourceParam" />-<xsl:value-of select="@TargetID" />-<xsl:value-of select="@TargetParam" /></xsl:attribute>
			<span>
				<span style="color: lime; margin-right: 0.8em;">Out Link</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@SourceParam" />
				</span>
				<span style="color: lime; margin-left: 0.2em; margin-right: 0.2em;">&gt;</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@TargetParam" />
				</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@TargetName" />
				</span>
				<span style="color: grey; margin-left: 0.2em;">
					<xsl:value-of select="@TargetID" />
				</span>
				<xsl:element name="a">
					<xsl:attribute name="href">#<xsl:value-of select="ancestor::GameReport[1]/@SafePath"/><xsl:value-of select="ancestor::LevelReport[1]/@SafePath"/>/<xsl:value-of select="ancestor::LevelReport[1]/@LevelName"/>/<xsl:value-of select="ancestor::Composite[1]/@GUID" />/<xsl:value-of select="@TargetID" />/in-<xsl:value-of select="@TargetParam" />-<xsl:value-of select="parent::*/@GUID" />-<xsl:value-of select="@SourceParam" /></xsl:attribute>
					<xsl:attribute name="style">
						text-decoration: none; color: cyan;
					</xsl:attribute>
					(Jump to In Link)
				</xsl:element>
			</span>
		</xsl:element>
	</xsl:template>

	<xsl:template mode="entity" match="Option">
		<li>
			<span>
				<span style="color: cyan; margin-right: 0.8em;">Parameter</span>
				<span style="margin-left: 0.2em; margin-right: 0.2em;">
					<xsl:value-of select="@Name" />
				</span>
				<span style="color: yellow; margin-right: 0.2em; margin-left: 0.2em;">
					<xsl:value-of select="@Type"/>
				</span>
				<xsl:if test="@Content!='CATHODE.Scripting.cResource'">
					<span style="color: cyan; margin-left: 0.2em; margin-right: 0.2em;">=</span>
					<span style="margin-left: 0.2em; margin-right: 0.2em;">
						<xsl:value-of select="@Content" />
					</span>
				</xsl:if>
			</span>
		</li>
	</xsl:template>
</xsl:stylesheet>