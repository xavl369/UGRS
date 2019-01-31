SELECT
case when t1.PymntGroup like'%Contado%' then 10
else t0.GroupNum
end as GroupNumber

FROM OCRD T0
INNER JOIN OCTG T1 on T1.GroupNum = t0.GroupNum
WHERE CardCode = '{CardCode}'
