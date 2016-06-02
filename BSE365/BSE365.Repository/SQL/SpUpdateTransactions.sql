CREATE PROCEDURE UpdateTransactions @Time DATETIME
AS
BEGIN
	EXEC UpdateNotTransferedTransactions @Time;

	EXEC UpdateNotConfirmedTransactions @Time;
END
