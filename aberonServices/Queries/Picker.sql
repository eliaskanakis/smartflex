SELECT 
	EMP.EMPLOYEEID "EmlpoyeeId",
	RTRIM(EMP.NAME1) "Name1"
FROM EMPLOYEES EMP
WHERE EMP.NAMESHORT=CAST(:pNameShort AS CHAR(10))