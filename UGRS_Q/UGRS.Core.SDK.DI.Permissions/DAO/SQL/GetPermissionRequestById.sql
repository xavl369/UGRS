SELECT

	PE.U_Date AS Date,
	PE.U_CrossingDate AS CrossingDate,
	PE.U_MobilizationTypeId AS MobilizationTypeId,
	PE.U_MobilizationType AS MobilizationType,
	PE.U_UgrsRequest AS UgrsRequest,
	PE.U_UgrsFolio AS UgrsFolio,
	PE.U_Producer AS Producer,
	PE.U_ProducerTelephone AS ProducerTelephone,

	ISNULL
	((
		PE.U_Location1

	),'') AS Location1,

	ISNULL
	((
		PE.U_Location2

	),'') AS Location2,

	ISNULL
	((
		PE.U_OriginState
	),'') AS OriginState,

	ISNULL
	((
		PE.U_OriginCity
	),'') AS OriginCity,

	ISNULL
	((
		SELECT 
			TOP 1 EN.Name 
		FROM [@UG_PE_WS_PORE] PO
			INNER JOIN [@UG_PE_ENRY] EN ON PO.U_PortId = EN.Code
		WHERE 
			PO.U_RequestId = PE.U_RequestId AND 
			PO.U_PortType = 1

	),'') AS Entry,

	ISNULL
	((
		SELECT 
			TOP 1 EN.Name 
		FROM [@UG_PE_WS_PORE] PO
			INNER JOIN [@UG_PE_ENRY] EN ON PO.U_PortId = EN.Code
		WHERE 
			PO.U_RequestId = PE.U_RequestId AND 
			PO.U_PortType = 2

	),'') AS Departure,

	PE.U_Transport AS TransportId,
	TR.Name AS Transport,

	(
		CASE WHEN
				PE.U_MobilizationType = 'Exportación'
			THEN
			(
				CONCAT(PE.U_Location1, ', ', PE.U_OriginCity, ', ', PE.U_OriginState)
			)
			ELSE
			(
				CONCAT(PE.U_OriginCity, ', ', PE.U_OriginState, ', ', PE.U_Location1)
			)
		END

	) AS Origin,

	(
		CASE WHEN
				PE.U_MobilizationType = 'Exportación'
			THEN
			(
				'Estados Unidos de Norte America'
			)
			ELSE
			(
				DE.U_Location
			)
		END

	) AS Destination,

	PE.U_Customs AS Customs1,

	ISNULL
	((
		SELECT 
			TOP 1 EN.Code 
		FROM [@UG_PE_WS_PORE] PO
			INNER JOIN [@UG_PE_ENRY] EN ON PO.U_PortId = EN.Code
		WHERE 
			PO.U_RequestId = PE.U_RequestId AND 
			PO.U_PortType = 2 AND
			EN.Name <> CS.Name
		ORDER BY PO.Code DESC

	),'') AS Customs2,

	ISNULL
	((
		SELECT TOP 1
			BP.CardCode 
		FROM OCRD BP
		WHERE
			BP.U_PE_CustomerCode = PE.U_CustomerCode

	),'') AS CardCode,

	ISNULL
	((
		SELECT TOP 1
			BP.U_PE_Observations 
		FROM OCRD BP
		WHERE
			BP.U_PE_CustomerCode = PE.U_CustomerCode

	),'') AS CustomerLocation

FROM [@UG_PE_WS_PERE] PE
INNER JOIN [@UG_PE_PETY] TY ON TY.Code = PE.U_MobilizationTypeId
INNER JOIN [@UG_PE_CUMS] CS ON CS.Code =
(
	CASE WHEN 
			ISNUMERIC(PE.U_Customs) = 1
		THEN
			CAST(PE.U_Customs AS INT)
		ELSE	
		(
			SELECT
				TOP 1 CU.Code 
			FROM 
				[@UG_PE_CUMS] CU
			WHERE
				CU.Name LIKE '%'+PE.U_Customs+'%'
		)
	END
)
	INNER JOIN [@UG_PE_WS_DERE] DE ON PE.U_RequestId = DE.U_RequestId
	INNER JOIN [@UG_PE_TRTY] TR ON PE.U_Transport = TR.Code
WHERE 
	PE.U_ProductiveGroup = 'UNION GANADERA REGIONAL DE SONORA' AND
	PE.U_RequestId = '{RequestId}'
	 