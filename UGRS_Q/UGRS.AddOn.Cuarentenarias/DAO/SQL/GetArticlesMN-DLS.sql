select U_Value,U_Comentario,

case 
when Name like '%CU_GLO_INV_PESOS%'
then 'MX'
when Name like '%CU_GLO_INV_DLLS%'
then 'USD'
end as Tipo

from
[@UG_CONFIG]

where Name like '%CU_GLO_INV_PESOS%' or Name like '%CU_GLO_INV_DLLS%'