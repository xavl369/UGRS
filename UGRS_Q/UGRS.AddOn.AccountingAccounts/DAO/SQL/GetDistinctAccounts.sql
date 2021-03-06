EXEC [sp_addlinkedsrvlogin] '{ServerName}', 'false', null, '{User}', '{Password}'; 
	
SELECT 
	CUENTA = CASE WHEN ACONC>=1 AND ACONC<=100 THEN CAST (SUBSTRING(RTRIM(CUENTA1),1,13) AS varchar) + RTRIM(ICCTAC) + SPACE(1) + CAST (SUBSTRING(RTRIM(CUENTA1),18,3) AS varchar) END
	FROM 
	(SELECT 
		ACONS
		, IPYEAR
		, IPMES
		, IPTIPO
		, IPNO
		, ATRAB
		, ACONC
		, AIMPO
		, ACCTO
		, ADEP
		, ACCTO_HN = CASE (LEN(ACCTO)) WHEN 1 THEN CAST(0 as varchar) + CAST(ACCTO as varchar) ELSE CAST(ACCTO as varchar) END
		, ADEP_HN = CASE (LEN(ADEP)) WHEN 1 THEN CAST(0 as varchar) + CAST(ADEP as varchar) ELSE CAST(ADEP as varchar) END
		FROM [{ServerName}].[{DBName}].[dbo].[Historico de Nominas] 
		WHERE ACONC BETWEEN 1 AND 100) AS TMP_HN  
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Catalogo Conceptos] on TMP_HN.ACONC = [Catalogo Conceptos].IDCONC 
LEFT JOIN (SELECT  
				ICDNOM
				, NIVEL
				, CUENTA1
				, CCTO_ID
				, CUENTA2
				, ACCTO_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,1,2) WHEN 2 THEN SUBSTRING(CCTO_ID,1,2) END
				, ADEP_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,4,5) WHEN 2 THEN CAST('00' as varchar) END
				FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Centros de Costos]) AS TMP_CC on TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Historico de Recibos Electronicos] TMP_HRE 
	on CAST(TMP_HN.IPYEAR AS varchar) + CAST(TMP_HN.IPMES AS VARCHAR) + CAST(TMP_HN.IPTIPO AS VARCHAR) + CAST(TMP_HN.IPNO AS varchar) + CAST(TMP_HN.ATRAB AS varchar) 
	= CAST(TMP_HRE.IPYEAR AS varchar) + CAST(TMP_HRE.IPMES AS varchar) + CAST(TMP_HRE.IPTIPO AS varchar) + CAST(TMP_HRE.IPNO AS varchar) + CAST(TMP_HRE.ATRAB AS varchar) 
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Catalogo Empleados] TMP_EMPL
	on TMP_HN.ATRAB = TMP_EMPL.NTRAB
WHERE  
	((TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC) and (TMP_HN.ADEP_HN = TMP_CC.ADEP_CC)) 		
	AND (TMP_HN.IPYEAR = '{Year}')
	AND (TMP_HN.IPTIPO = '{Tipo}')
	AND (TMP_HN.IPNO = '{Ipno}')

 
UNION

SELECT 		
		CUENTA = CASE WHEN ACONC>=101 AND ACONC<=200 THEN ICCTAA END
FROM (SELECT 
		ACONS,
		IPYEAR,
		IPMES,
		IPTIPO,
		IPNO,
		ATRAB,
		ACONC,
		AIMPO,
		ACCTO,
		ADEP, 
		'00' AS ACCTO_HN, 
		'00' AS ADEP_HN
		FROM [{ServerName}].[{DBName}].[dbo].[Historico de Nominas] WHERE ACONC BETWEEN 101 AND 200) AS TMP_HN  
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Catalogo Conceptos] on TMP_HN.ACONC = [Catalogo Conceptos].IDCONC 
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Historico de Recibos Electronicos] TMP_HRE 
	on CAST(TMP_HN.IPYEAR AS varchar) + CAST(TMP_HN.IPMES AS VARCHAR) + CAST(TMP_HN.IPTIPO AS VARCHAR) + CAST(TMP_HN.IPNO AS varchar) + CAST(TMP_HN.ATRAB AS varchar) 
	= CAST(TMP_HRE.IPYEAR AS varchar) + CAST(TMP_HRE.IPMES AS varchar) + CAST(TMP_HRE.IPTIPO AS varchar) + CAST(TMP_HRE.IPNO AS varchar) + CAST(TMP_HRE.ATRAB AS varchar) 
LEFT JOIN [{ServerName}].[{DBName}].[dbo].[Catalogo Empleados] TMP_EMPL
	on TMP_HN.ATRAB = TMP_EMPL.NTRAB
WHERE
	(TMP_HN.IPYEAR = '{Year}')
	AND (TMP_HN.IPTIPO = '{Tipo}')
	AND (TMP_HN.IPNO = '{Ipno}')