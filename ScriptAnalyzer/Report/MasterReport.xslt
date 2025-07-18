<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="Report.css" />
				<script defer="true" src="Report.js"></script>
			</head>
			<body>
				<xsl:apply-templates mode="root" />
				<span>
					<i>
						Not an official tool of OpenCAGE.<br />
						<a target="_blank" href="https://opencage.co.uk/">Consider supporting the OpenCAGE and CATHODELib toolset.</a>
					</i>
				</span>
			</body>
		</html>
	</xsl:template>

	<xsl:template mode="root" match="GameReport">
		<div class="expandableSection gameReportContainer">
			<span class="expandableSectionAdditionalClick">
				<span class="expandableSectionHeader" style="font-size: 2em; margin-right: 5px;">
					<b>OpenCAGE Game Report</b>
				</span>
				<span>
					<xsl:value-of select="@Path"/>
				</span>
			</span>
			<div class="expandableSectionContent">
				<xsl:apply-templates mode="game" />
			</div>
		</div>
	</xsl:template>

	<xsl:include href="./Report.xslt"/>
</xsl:stylesheet>
