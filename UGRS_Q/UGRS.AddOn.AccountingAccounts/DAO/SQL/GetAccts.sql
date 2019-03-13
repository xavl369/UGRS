EXEC sp_addlinkedsrvlogin '{ServerName}', 'false', null, '{User}', '{Password}'; 

SELECT  
	--  [CC].ICNOM
	--, CONVERT(VARCHAR(250), TMP_HN.AIMPO) AIMPO	
	-- , TMP_HN.ACCTO_HN,
	  TMP_CC.ICDNOM
	 , CONVERT(VARCHAR(250), TMP_HN.AIMPO) AIMPO	
	 --, TMP_HN.ADEP_HN
	 --, [CC].ICCTAC
	 --, [CC].ICCTAA
	 ,Convert(varchar(250),TMP_HN.ACONC) ACONC
	 ,Convert(varchar(250),TMP_HN.ATRAB) ATRAB
	 , CONVERT(VARCHAR(250), TMP_CC.NIVEL) NIVEL
	 , replace(left(CASE WHEN ACONC>=1 AND ACONC<=100 THEN CUENTA1 END,16),' ','') CUE
	 , ltrim(rtrim(case when charindex(' ',CUENTAMOD)>0 then SUBSTRING(cuentamod,0,charindex(' ',CUENTAMOD)) else CUENTAMOD end)) CC
	 , ltrim(rtrim(case when charindex(' ',CUENTAMOD)>0 then SUBSTRING(cuentamod,charindex(' ',CUENTAMOD), len(CUENTAMOD))  else '' end)) PROY
	 --, CONVERT(VARCHAR(250), TMP_HN.ACCTO) ACCTO
	 --, CONVERT(VARCHAR(250), TMP_HN.ADEP) ADEP
	 , TMP_CC.CUENTA1
	 --, TMP_CC.CCTO_ID
	 --, TMP_HRE.UUID
	 --, TMP_CC.CUENTA2
	 , TMP_EMPL.NNOM
	 , TMP_EMPL.NRFC	   
	 ,TMP_HRE.UUID
	 FROM 
		(SELECT 
		    CAST(IPYEAR AS varchar(10)) + CAST(IPMES AS VARCHAR(10)) + CAST(IPTIPO AS VARCHAR(10)) + CAST(IPNO AS varchar(10)) + CAST(ATRAB AS varchar(10)) Cod,
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
		 WHERE ACONC BETWEEN 1 AND 100 and IPYEAR = '{Year}' and IPTIPO = '{Tipo}' and IPNO = '{Ipno}') AS TMP_HN  
	LEFT JOIN (SELECT ICNOM, ICCTAC, ICCTAA, IDCONC FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Conceptos]) [CC] on TMP_HN.ACONC = [CC].IDCONC 
	LEFT JOIN (SELECT  
					ICDNOM
					, NIVEL
					, CUENTA1
					, ltrim(rtrim(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(CUENTA1,'0',''),'1',''),'2',''),'3',''),'4',''),'5',''),'6',''),'7',''),'8',''),'9',''))) CUENTAMOD
					, CCTO_ID
					, CUENTA2
					, ACCTO_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,1,2) WHEN 2 THEN SUBSTRING(CCTO_ID,1,2) END
					, ADEP_CC = CASE LEN(RTRIM(CCTO_ID)) WHEN 5 THEN SUBSTRING(CCTO_ID,4,5) WHEN 2 THEN CAST('00' as varchar) END
					FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Centros de Costos]) AS TMP_CC on TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC
	LEFT JOIN (SELECT CAST(IPYEAR AS varchar) + CAST(IPMES AS varchar(10)) + CAST(IPTIPO AS varchar(10)) + CAST(IPNO AS varchar(10)) + CAST(ATRAB AS varchar(10)) Cod,
	     IPYEAR, IPMES, IPTIPO, IPNO, ATRAB, UUID FROM [{ServerName}].[{DBName}].[dbo].[Historico de Recibos Electronicos] where  IPYEAR = '{Year}' and IPTIPO = '{Tipo}' and IPNO = '{Ipno}') TMP_HRE 
		on TMP_HN.cod = TMP_HRE.Cod
	LEFT JOIN ( SELECT NTRAB , NRFC, NNOM FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Empleados]) TMP_EMPL
		on TMP_HN.ATRAB = TMP_EMPL.NTRAB
	WHERE   
		((TMP_HN.ACCTO_HN = TMP_CC.ACCTO_CC) AND (TMP_HN.ADEP_HN = TMP_CC.ADEP_CC)) 
	union all
	SELECT 
	--TMP_HN.ACONS
	--	, TMP_HN.Cod
	--	, TMP_HN.ACONC,		
		 [Catalogo Conceptos].ICNOM
		, CONVERT(VARCHAR(250), TMP_HN.AIMPO) AIMPO	
		,Convert(varchar(250),TMP_HN.ACONC) ACONC
		,Convert(varchar(250),TMP_HN.ATRAB) ATRAB
		--, '00' AS ACCTO_HN
		--, '-' AS ICDNOM
		--, '00' AS ADEP_HN
		--, [Catalogo Conceptos].ICCTAC
		--, [Catalogo Conceptos].ICCTAA
		, '0' AS NIVEL
		, replace(left(CASE WHEN ACONC>=101 AND ACONC<=200 THEN ICCTAA END,16),' ','')  CUE
		 , ltrim(rtrim(case when charindex(' ',ICCTAAMOD)>0 then SUBSTRING(ICCTAAMOD,0,charindex(' ',ICCTAAMOD)) else ICCTAAMOD end)) CC
		 , ltrim(rtrim(case when charindex(' ',ICCTAAMOD)>0 then SUBSTRING(ICCTAAMOD,charindex(' ',ICCTAAMOD), len(ICCTAAMOD))  else '' end)) PROY
		--, CONVERT(VARCHAR(250), TMP_HN.ACCTO) ACCTO
		--, CONVERT(VARCHAR(250), TMP_HN.ADEP) ADEP	
		, ICCTAA AS CUENTA1
		--, '00' AS CCTO_ID
		--, TMP_HRE.UUID
		--, '' AS CUENTA2
		, TMP_EMPL.NNOM
		, TMP_EMPL.NRFC	 
		,TMP_HRE.UUID		
	FROM (SELECT 
	        CAST(IPYEAR AS varchar(10)) + CAST(IPMES AS VARCHAR(10)) + CAST(IPTIPO AS VARCHAR(10)) + CAST(IPNO AS varchar(10)) + CAST(ATRAB AS varchar(10)) Cod,
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
			FROM [{ServerName}].[{DBName}].[dbo].[Historico de Nominas] WHERE ACONC BETWEEN 101 AND 200 and IPYEAR = '{Year}' and IPTIPO = '{Tipo}' and IPNO = '{Ipno}') AS TMP_HN  
	LEFT JOIN (SELECT ICNOM, ICCTAC, ICCTAA
	       , ltrim(rtrim(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(ICCTAA,'0',''),'1',''),'2',''),'3',''),'4',''),'5',''),'6',''),'7',''),'8',''),'9',''))) ICCTAAMOD
	       ,IDCONC FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Conceptos])  [Catalogo Conceptos] on TMP_HN.ACONC = [Catalogo Conceptos].IDCONC 
	LEFT JOIN (SELECT CAST(IPYEAR AS varchar(10)) + CAST(IPMES AS VARCHAR(10)) + CAST(IPTIPO AS VARCHAR(10)) + CAST(IPNO AS varchar(10)) + CAST(ATRAB AS varchar(10)) Cod,IPYEAR, IPMES, IPTIPO, IPNO, ATRAB, UUID 
	           FROM  [{ServerName}].[{DBName}].[dbo].[Historico de Recibos Electronicos]  where  IPYEAR = '{Year}' and IPTIPO = '{Tipo}' and IPNO = '{Ipno}') TMP_HRE 
		on TMP_HN.Cod 	= TMP_HRE.Cod 
	LEFT JOIN ( SELECT NTRAB, NNOM , NRFC FROM [{ServerName}].[{DBName}].[dbo].[Catalogo Empleados] ) TMP_EMPL
		on TMP_HN.ATRAB = TMP_EMPL.NTRAB