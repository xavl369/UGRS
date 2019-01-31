SELECT 
(
	ISNULL(MAX(U_UgrsFolio), 0) + 1
)
AS UgrsFolio 
FROM [@UG_PE_WS_PERE] 
WHERE U_UgrsRequest LIKE '{Prefix}'