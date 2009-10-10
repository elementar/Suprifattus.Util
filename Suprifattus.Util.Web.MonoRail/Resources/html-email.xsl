<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns:h="http://www.w3.org/1999/xhtml"
								xmlns="http://www.w3.org/1999/xhtml">
	
	<xsl:output omit-xml-declaration="yes" method="html" encoding="utf-8" indent="no"/>
	<xsl:strip-space elements="h:*"/>
	<xsl:preserve-space elements="h:html h:head h:body"/>
	<xsl:namespace-alias stylesheet-prefix="h" result-prefix="#default"/>
	
	<xsl:param name="fontSize" select="2" />
	<xsl:param name="fontFace" select="'Verdana, sans-serif'" />

	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="node()">
		<xsl:call-template name="clone"/>
	</xsl:template>
	
	<xsl:template match="h:body/*//text()">
		<xsl:value-of select="' '"/>
		<xsl:value-of select="normalize-space(.)"/>
		<xsl:value-of select="' '"/>
	</xsl:template>

	<xsl:template match="h:td/* | h:td/text() | h:th/* | h:th/text() | h:h1/text() | h:h2/text() | h:body/*">
		<font size="{$fontSize}" face="{$fontFace}">
			<xsl:call-template name="clone"/>
		</font>
	</xsl:template>

	<xsl:template name="clone">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>
