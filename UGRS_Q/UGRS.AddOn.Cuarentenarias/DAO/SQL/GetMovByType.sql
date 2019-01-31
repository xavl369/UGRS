SELECT
code as Code,
Name as Name,
U_Tipo as Tipo
FROM [@UG_CU_MVTY]
where U_Tipo = '{Type}' and Name = '{Name}'