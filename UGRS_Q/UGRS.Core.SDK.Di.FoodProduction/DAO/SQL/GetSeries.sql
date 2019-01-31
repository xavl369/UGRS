select distinct T1.Series, T1.SeriesName, T0.U_GLO_CostCenter
from OUSR T0 inner join NNM1 T1 on T0.DfltsGroup = T1.SeriesName where (T0.USERID = '{UserID}' and T1.ObjectCode='{ObjCode}')