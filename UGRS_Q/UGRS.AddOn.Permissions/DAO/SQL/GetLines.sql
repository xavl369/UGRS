 select 
U_Prefix+U_EarringFrom as Desde,
U_Prefix+U_EarringTo as Hasta,
Code
from [@UG_PE_ERNK] where  U_BaseEntry = '{BaseEntry}' and U_Cancelled = 'N'