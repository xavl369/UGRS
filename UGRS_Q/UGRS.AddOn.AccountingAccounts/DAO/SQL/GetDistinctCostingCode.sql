EXEC sp_addlinkedsrvlogin '{ServerName}', 'false', null, '{User}', '{Password}'; 

SELECT 
	 distinct(CUENTA2)
	 FROM 
		(SELECT 
			IPYEAR
			, IPMES
			, IPTIPO
			, IPNO
			, ATRAB
			, ACONC
			, ACCTO
			, ADEP
			, ACCTO_HN = CASE (LEN(ACCTO)) WHEN 1 THEN CAST(0 as varchar) + CAST(ACCTO as varchar) ELSE CAST(ACCTO as varchar) END
			, ADEP_HN = CASE (LEN(ADEP)) WHEN 1 THEN CAST(0 as varchar) + CAST(ADEP as varchar) ELSE CAST(ADEP as varchar) END
		 FROM [{ServerName}].[{DBName}].[dbo].[Historico de Nominas] 
		 WHERE ACONC BETWEEN 1 AND 100) AS TMP_HN  
	LEFT JOIN (SELECT  
					CCTO_ID
					, CUENTA2
					, ACCTO_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,1,2) WHEN 2 THEN SUBSTRING(CCTO_ID,1,2) END
					, ADEP_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,4,5) WHEN 2 THEN CAST('00' as varchar) END
					FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Centros de Costos]) AS TMP_CC on TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC
	WHERE  
		((TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC) and (TMP_HN.ADEP_HN = TMP_CC.ADEP_CC)) 		
		AND (TMP_HN.IPYEAR = '{Year}')
		AND (TMP_HN.IPTIPO = '{Tipo}')
		AND (TMP_HN.IPNO = '{Ipno}')