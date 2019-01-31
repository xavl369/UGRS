SELECT COUNT(*) FROM ORDR SO
INNER JOIN [@UG_PE_WS_PERE] PE 
ON PE.U_UgrsFolio = SO.U_PE_FolioUGRS 
AND PE.U_UgrsRequest = SO.U_PE_RequestCodeUGRS
WHERE PE.U_RequestId = '{RequestId}'